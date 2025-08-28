using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // 引入 DOTween 命名空间

public class OpenCameraList : MonoBehaviour
{
    public GameObject targetObject; // 需要移动的物体
    public Vector3 openPosition; // 下拉后的目标位置
    public Vector3 closePosition; // 初始位置（恢复的起始位置）

    private bool isAnimating = false; // 用于防止重复点击
    public bool isOn = false; // 用于防止重复点击

    public MouseEnter mouseEnter;
    void Start()
    {

    }

    void Update()
    {

    }
    private void OnMouseDown()
    {
        if(!mouseEnter.isDisabled&& !isAnimating)
        {
            TogglePanel();
        }
            
    }
    // 切换面板展开和收起
    void TogglePanel()
    {
        if (isAnimating) return; // 如果正在动画中，返回

        isAnimating = true; // 设置为正在动画

        if (!isOn)
        {
            // 如果物体在初始位置，则播放下拉动画
            targetObject.transform.DOLocalMove(openPosition, 0.5f).OnComplete(() =>
            {
                isAnimating = false; // 动画完成后恢复点击
                isOn = true;
            });
        }
        else
        {
            // 如果物体已经在下拉位置，则恢复初始位置
            targetObject.transform.DOLocalMove(closePosition, 0.5f).OnComplete(() =>
            {
                isAnimating = false; // 动画完成后恢复点击
                isOn = false;
            });
        }
    }
}
