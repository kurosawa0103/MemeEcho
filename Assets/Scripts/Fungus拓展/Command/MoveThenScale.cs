using UnityEngine;
using Fungus;
using DG.Tweening;

[CommandInfo("Custom",
             "�ƶ�������",
             "���ƶ���Ŀ��λ�ã�Ȼ�����ŵ�Ŀ���С��ʹ��DOTween��")]
public class MoveThenScale : Command
{
    [Tooltip("Ŀ��������Ϊ�գ���ʹ��Tag����")]
    public GameObject targetObject;

    [Tooltip("Ŀ������Tag��������Ϊ��ʱ���ã�")]
    public string targetTag;

    [Tooltip("�ƶ�Ŀ��λ��")]
    public Vector3 targetPosition;

    [Tooltip("����Ŀ���С")]
    public Vector3 targetScale = Vector3.one;

    [Tooltip("�ƶ�ʱ��")]
    public float moveDuration = 1f;

    [Tooltip("����ʱ��")]
    public float scaleDuration = 1f;

    [Tooltip("�Ƿ������ź��������")]
    public bool waitUntilFinished = true;

    public override void OnEnter()
    {
        // �Զ�����Ŀ�����
        if (targetObject == null && !string.IsNullOrEmpty(targetTag))
        {
            targetObject = GameObject.FindWithTag(targetTag);
        }

        if (targetObject == null)
        {
            Debug.LogWarning("MoveThenScale��δ�ҵ�Ŀ�����");
            Continue();
            return;
        }

        targetObject.transform.DOLocalMove(targetPosition, moveDuration).OnComplete(() =>
        {
            Tween scaleTween = targetObject.transform.DOScale(targetScale, scaleDuration);
            if (waitUntilFinished)
            {
                scaleTween.OnComplete(() =>
                {
                    Continue();
                });
            }
            else
            {
                Continue();
            }
        });

        if (!waitUntilFinished)
        {
            Continue();
        }
    }

    public override string GetSummary()
    {
        if (targetObject != null)
            return $"{targetObject.name} �ƶ��� {targetPosition}��Ȼ�����ŵ� {targetScale}";
        if (!string.IsNullOrEmpty(targetTag))
            return $"Tag Ϊ \"{targetTag}\" �Ķ����ƶ��� {targetPosition}��Ȼ�����ŵ� {targetScale}";
        return "δ����Ŀ������Tag";
    }

    public override Color GetButtonColor()
    {
        return new Color32(200, 255, 200, 255);
    }
}
