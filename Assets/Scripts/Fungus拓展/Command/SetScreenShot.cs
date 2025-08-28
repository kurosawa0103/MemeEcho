using Fungus;
using UnityEngine;
using System.Collections.Generic;

[CommandInfo("Custom", "Set Screenshot Conditions", "���ý�ͼ����", 0)]
public class SetScreenShot : Command
{
    [Tooltip("�����б�")]
    public List<PhotoCaptureCondition> conditionConfigs;

    private MaskedScreenshot maskedScreenshot;

    public override void OnEnter()
    {
        // ��ȡ MaskedScreenshot �ű�ʵ��
        maskedScreenshot = FindObjectOfType<MaskedScreenshot>();

        if (maskedScreenshot != null)
        {
            // �����õ������б�ֵ�� MaskedScreenshot �ű�
            maskedScreenshot.conditionConfigs = conditionConfigs;
            Debug.Log("��ͼ�������������");
        }
        else
        {
            Debug.LogWarning("δ�ҵ� MaskedScreenshot �ű�");
        }

        Continue();
    }
}
