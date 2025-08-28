using UnityEngine;
using UnityEngine.UI;
using Fungus;
using DG.Tweening;

[CommandInfo("Custom",
             "FadeTarget",
             "ʹ�� DoTween ����/����Ŀ�� UI Image�������� Alpha�����޸ļ���״̬��")]
public class FadeTarget : Command
{
    [Tooltip("Ŀ�� UI ��������� Image��")]
    public GameObject targetObject;

    [Tooltip("����/����Ŀ��͸���ȣ�0 = ������1 = ���룩")]
    [Range(0f, 1f)]
    public float targetAlpha = 0f;

    [Tooltip("��������ʱ�䣨�룩")]
    public float duration = 1f;

    public override void OnEnter()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("[FadeTarget] δָ��Ŀ�����");
            Continue();
            return;
        }

        Image image = targetObject.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogWarning("[FadeTarget] Ŀ�����û�� Image ���");
            Continue();
            return;
        }

        image.DOFade(targetAlpha, duration)
             .SetUpdate(true) // ��ѡ������ͣʱ���ܲ��ţ����� UI ������
             .OnComplete(() => Continue());
    }

    public override string GetSummary()
    {
        if (targetObject == null)
            return "δ����Ŀ�����";

        string action = targetAlpha <= 0f ? "����" : "����";
        return $"{action} {targetObject.name}������ {duration:F2} ��";
    }

    public override Color GetButtonColor()
    {
        return new Color32(180, 200, 255, 255); // ǳ��ɫ
    }
}
