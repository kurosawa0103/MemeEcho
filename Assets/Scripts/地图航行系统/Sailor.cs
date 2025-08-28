using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Sailor : MonoBehaviour
{


    [LabelText("是否航行")]
    public bool isSailing;  //控制是否航行
    [LabelText("当前航行距离"), ReadOnly]
    private float sailDistance; //航行距离
    [LabelText("航行速度")]
    public float sailSpeed;      //航行速度
    [LabelText("当前航行速度"),ReadOnly]
    public float currentSailSpeed;


    [LabelText("是否抵达目标点"), ReadOnly]
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

            sailDistance += Time.deltaTime * currentSailSpeed; // 航行距离速度
        }
        else
        {
            currentSailSpeed = 0;
        }

    }


}
