using UnityEngine;
using Fungus;
using DG.Tweening;

[CommandInfo("��չ",
             "ChangeScaleByTag",
             "�ı����ָ�� Tag ����������ţ�ʹ�� DOTween ����")]
public class ChangeScaleByTag : Command
{
    public enum ScaleMode
    {
        SetToValue,    // ����ΪĿ��ֵ
        AddToCurrent   // �ڵ�ǰֵ������
    }

    [Tooltip("Ŀ������� Tag")]
    [SerializeField] private string targetTag = "SubRoot";

    [Tooltip("����ֵ��Set: Ŀ��ֵ / Add: ����ֵ��")]
    [SerializeField] private Vector3 scaleValue = new Vector3(1, 1, 1);

    [Tooltip("����ʱ�䣨�룩")]
    [SerializeField] private float duration = 1f;

    [Tooltip("�Ƿ�ȴ���������ټ���ִ�� Flowchart")]
    [SerializeField] private bool waitUntilFinished = true;

    [Tooltip("����ģʽ")]
    [SerializeField] private ScaleMode scaleMode = ScaleMode.SetToValue;

    public override void OnEnter()
    {
        GameObject target = GameObject.FindGameObjectWithTag(targetTag);

        if (target == null)
        {
            Debug.LogWarning($"ChangeScaleByTag: �Ҳ��� Tag Ϊ [{targetTag}] �����塣");
            Continue();
            return;
        }

        Vector3 targetScale = scaleMode == ScaleMode.SetToValue
            ? scaleValue
            : target.transform.localScale + scaleValue;

        Tween tween = target.transform.DOScale(targetScale, duration);

        if (waitUntilFinished)
        {
            tween.OnComplete(() => Continue());
        }
        else
        {
            Continue();
        }
    }

    public override string GetSummary()
    {
        string modeDesc = scaleMode == ScaleMode.SetToValue ? "����Ϊ" : "����";
        return $"{modeDesc} [{targetTag}] �����ţ�{scaleValue}����ʱ {duration} ��";
    }

    public override Color GetButtonColor()
    {
        return new Color32(135, 206, 250, 255); // ����ɫ
    }
}
