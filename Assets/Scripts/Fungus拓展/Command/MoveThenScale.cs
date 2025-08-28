using UnityEngine;
using Fungus;
using DG.Tweening;

[CommandInfo("Custom",
             "移动后缩放",
             "先移动到目标位置，然后缩放到目标大小（使用DOTween）")]
public class MoveThenScale : Command
{
    [Tooltip("目标对象。如果为空，将使用Tag查找")]
    public GameObject targetObject;

    [Tooltip("目标对象的Tag（当上面为空时启用）")]
    public string targetTag;

    [Tooltip("移动目标位置")]
    public Vector3 targetPosition;

    [Tooltip("缩放目标大小")]
    public Vector3 targetScale = Vector3.one;

    [Tooltip("移动时间")]
    public float moveDuration = 1f;

    [Tooltip("缩放时间")]
    public float scaleDuration = 1f;

    [Tooltip("是否在缩放后继续流程")]
    public bool waitUntilFinished = true;

    public override void OnEnter()
    {
        // 自动查找目标对象
        if (targetObject == null && !string.IsNullOrEmpty(targetTag))
        {
            targetObject = GameObject.FindWithTag(targetTag);
        }

        if (targetObject == null)
        {
            Debug.LogWarning("MoveThenScale：未找到目标对象");
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
            return $"{targetObject.name} 移动到 {targetPosition}，然后缩放到 {targetScale}";
        if (!string.IsNullOrEmpty(targetTag))
            return $"Tag 为 \"{targetTag}\" 的对象移动到 {targetPosition}，然后缩放到 {targetScale}";
        return "未设置目标对象或Tag";
    }

    public override Color GetButtonColor()
    {
        return new Color32(200, 255, 200, 255);
    }
}
