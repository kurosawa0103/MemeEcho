using Fungus;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CommandInfo("Custom",
             "Check Screenshot",
             "����ͼ�ű��Ƿ���ɽ�ͼ����������������")]
public class CheckScreenshotCommand : Command
{
    // Screenshot Capture Mode
    public enum ScreenshotCaptureMode
    {
        AnyImage,           // ��������ͼ
        ConditionBased      // ������������ͼ
    }

    [Tooltip("ѡ���ͼģʽ")]
    public ScreenshotCaptureMode captureMode = ScreenshotCaptureMode.AnyImage;

    [Tooltip("MaskedScreenshot �ű�������")]
    public MaskedScreenshot maskedScreenshot;

    [Tooltip("�������ã������� ConditionBased ģʽ")]
    public PhotoCaptureCondition[] conditionConfigs;

    private bool isCheckingCondition = false; // �Ƿ����ڼ������

    public override void OnEnter()
    {
        maskedScreenshot = GameObject.FindObjectOfType<MaskedScreenshot>();
        // ���Ľ�ͼ����¼�
        if (maskedScreenshot != null)
        {
            maskedScreenshot.OnScreenshotTaken += HandleScreenshotTaken;
        }

        // ����ѡ��Ľ�ͼģʽ��������߼�
        if (captureMode == ScreenshotCaptureMode.ConditionBased)
        {
            if (!isCheckingCondition)
            {
                isCheckingCondition = true;
                Flowchart.BroadcastFungusMessage("StartCheckingScreenshotCondition");
                StartCoroutine(CheckScreenshotBasedOnCondition());
            }
        }
        else if (captureMode == ScreenshotCaptureMode.AnyImage)
        {
            // ����Ƿ��Ѿ��н�ͼ���
            CheckIfScreenshotTaken();
        }
    }

    private void HandleScreenshotTaken(PhotoCaptureCondition condition)
    {
        Debug.Log($"�յ� ScreenshotTaken �¼���ƥ��������{condition.conditionID}���㲥�� Fungus");

        if (captureMode == ScreenshotCaptureMode.AnyImage ||
            (captureMode == ScreenshotCaptureMode.ConditionBased &&
             System.Array.Exists(conditionConfigs, c => c.conditionID == condition.conditionID)))
        {
            Flowchart.BroadcastFungusMessage("ScreenshotTaken");
            isCheckingCondition = false;
            Continue(); 
        }
    }

    /// <summary>
    /// ��������ͼ�����Ƿ�����
    /// </summary>
    private IEnumerator CheckScreenshotBasedOnCondition()
    {
        while (isCheckingCondition)
        {
            foreach (var config in conditionConfigs)
            {

                // �������
                if (maskedScreenshot.CheckCondition(config))
                {
                    // �������㣬����ͼ�Ƿ������
                    CheckIfScreenshotTaken();
                    yield break; // ���������㣬ֹͣ���
                }
            }

            yield return new WaitForSeconds(0.1f); // �ӳ�һ��ʱ����ټ�飬����Ƶ��
        }
    }

    /// <summary>
    /// ����ͼ�Ƿ�����ɣ��ɽ�ͼ�ű�������
    /// </summary>
    private void CheckIfScreenshotTaken()
    {
        // �жϽ�ͼ�Ƿ���ɣ���ͼ�ű���Ҫ֪ͨ����Ϣ
        if (maskedScreenshot != null)
        {
            // ��ͼ��ɣ�ִ������߼�
            Flowchart.BroadcastFungusMessage("ScreenshotTaken");

            // ���Ը�������ִ����������
            isCheckingCondition = false; // ֹͣ�������
        }
    }
    public override Color GetButtonColor()
    {
        return new Color32(200, 100, 150, 255);
    }
    public override string GetSummary()
    {
        return captureMode == ScreenshotCaptureMode.AnyImage ? "��������ͼ�Ƿ����" : "���������ͼ�Ƿ����";
    }
}
