using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Sailor : MonoBehaviour
{


    [LabelText("�Ƿ���")]
    public bool isSailing;  //�����Ƿ���
    [LabelText("��ǰ���о���"), ReadOnly]
    private float sailDistance; //���о���
    [LabelText("�����ٶ�")]
    public float sailSpeed;      //�����ٶ�
    [LabelText("��ǰ�����ٶ�"),ReadOnly]
    public float currentSailSpeed;


    [LabelText("�Ƿ�ִ�Ŀ���"), ReadOnly]
    public bool arriveTargetLand;

    private void Awake()
    {

    }
    void Start()
    {

    }

    void Update()
    {
        SailDistance();
    }
    public void SailDistance()
    {

        if (isSailing)
        {
            currentSailSpeed = sailSpeed;

            sailDistance += Time.deltaTime * currentSailSpeed; // ���о����ٶ�
        }
        else
        {
            currentSailSpeed = 0;
        }

    }


}
