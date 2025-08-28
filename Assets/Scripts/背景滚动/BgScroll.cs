using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public enum SailAffectType
{
    NoEffect,           // ���ܺ����ٶ�Ӱ��
    SailEffect,       // ������Ӱ�죨ԭ���� effectedBySail == true��
    SailAdd        // ֻ���٣������٣�������Ҫ��Ч����
}
public class BgScroll : MonoBehaviour
{
    private BgCreat _bgCreat;
    public Sailor _sailor;

    [LabelText("������������")]
    public float moveValue;

    private bool oneTime;

    [LabelText("�Ƿ�ѭ������")]
    public bool loopCreat;

    [LabelText("�����ٶ�Ӱ������")]
    public SailAffectType sailAffectType;

    // ��������Ҫ���ɵ���������
    [LabelText("������������")]
    public ObjectType objectType;

    // ��������Ƿ��滻��Ԥ����
    public bool isReplaced = false;

    [LabelText("�Ƿ��ǵ�ͼ�νӵ�")]
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

        // �ж��Ƿ��ѵ����е㣬�����±������ƶ�
        if (transform.position.x <= _bgCreat.midPoint.position.x && !oneTime)
        {
            if (loopCreat)
            {
                _bgCreat.CreateObject(objectType); // ����ѡ���������������
            }

            if (isMapConnector) // ����ǵ�ͼ�νӵ㣬��ô�ͻ�ִ�
            {
                //moveValue = 0;//����ѭ���Ļ��ִ��е���ٶȾͻ����
                _sailor.isSailing = false;
               // _sailor.ArriveTargetLand();
            }
            oneTime = true;
        }

        if (transform.position.x <= _bgCreat.endPoint.position.x)
        {
            _bgCreat.ReturnObjectToPool(objectType, gameObject, isReplaced); // �����������յ������
            oneTime = false;
        }
    }
}
