using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public enum SailAffectType
{
    NoEffect,           // 不受航行速度影响
    SailEffect,       // 按比例影响（原来的 effectedBySail == true）
    SailAdd        // 只加速，不减速（你最新要的效果）
}
public class BgScroll : MonoBehaviour
{
    private BgCreat _bgCreat;
    public Sailor _sailor;

    [LabelText("背景滚动速率")]
    public float moveValue;

    private bool oneTime;

    [LabelText("是否循环生成")]
    public bool loopCreat;

    [LabelText("航行速度影响类型")]
    public SailAffectType sailAffectType;

    // 用于区分要生成的物体类型
    [LabelText("生成物体类型")]
    public ObjectType objectType;

    // 标记物体是否替换过预制体
    public bool isReplaced = false;

    [LabelText("是否是地图衔接点")]
    public bool isMapConnector;

    void Start()
    {
        _bgCreat = GameObject.FindGameObjectWithTag("Mgr").GetComponent<BgCreat>();
        _sailor = GameObject.FindGameObjectWithTag("Mgr").GetComponent<Sailor>();
    }

    void Update()
    {
        switch (sailAffectType)
        {
            case SailAffectType.NoEffect:
                transform.Translate(Vector3.left * Time.deltaTime * moveValue);
                break;
            case SailAffectType.SailEffect:
                transform.Translate(Vector3.left * Time.deltaTime * moveValue * _sailor.currentSailSpeed);
                break;
            case SailAffectType.SailAdd:
                transform.Translate(Vector3.left * Time.deltaTime * moveValue * (1 + _sailor.currentSailSpeed));
                break;

        }

        // 判断是否已到达中点，生成新背景或云朵
        if (transform.position.x <= _bgCreat.midPoint.position.x && !oneTime)
        {
            if (loopCreat)
            {
                _bgCreat.CreateObject(objectType); // 根据选择的类型生成物体
            }

            if (isMapConnector) // 如果是地图衔接点，那么就会抵达
            {
                //moveValue = 0;//不是循环的话抵达中点后，速度就会归零
                _sailor.isSailing = false;
               // _sailor.ArriveTargetLand();
            }
            oneTime = true;
        }

        if (transform.position.x <= _bgCreat.endPoint.position.x)
        {
            _bgCreat.ReturnObjectToPool(objectType, gameObject, isReplaced); // 否则正常回收到对象池
            oneTime = false;
        }
    }
}
