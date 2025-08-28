using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // ���� DOTween �����ռ�

public class OpenCameraList : MonoBehaviour
{
    public GameObject targetObject; // ��Ҫ�ƶ�������
    public Vector3 openPosition; // �������Ŀ��λ��
    public Vector3 closePosition; // ��ʼλ�ã��ָ�����ʼλ�ã�

    private bool isAnimating = false; // ���ڷ�ֹ�ظ����
    public bool isOn = false; // ���ڷ�ֹ�ظ����

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
    // �л����չ��������
    void TogglePanel()
    {
        if (isAnimating) return; // ������ڶ����У�����

        isAnimating = true; // ����Ϊ���ڶ���

        if (!isOn)
        {
            // ��������ڳ�ʼλ�ã��򲥷���������
            targetObject.transform.DOLocalMove(openPosition, 0.5f).OnComplete(() =>
            {
                isAnimating = false; // ������ɺ�ָ����
                isOn = true;
            });
        }
        else
        {
            // ��������Ѿ�������λ�ã���ָ���ʼλ��
            targetObject.transform.DOLocalMove(closePosition, 0.5f).OnComplete(() =>
            {
                isAnimating = false; // ������ɺ�ָ����
                isOn = false;
            });
        }
    }
}
