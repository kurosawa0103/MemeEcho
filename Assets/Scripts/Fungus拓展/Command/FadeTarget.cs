using UnityEngine;
using UnityEngine.UI;
using Fungus;
using DG.Tweening;

[CommandInfo("Custom",
             "FadeTarget",
             "使用 DoTween 淡入/淡出目标 UI Image（仅更改 Alpha，不修改激活状态）")]
public class FadeTarget : Command
{
    [Tooltip("目标 UI 对象（需带有 Image）")]
    public GameObject targetObject;

    [Tooltip("淡入/淡出目标透明度（0 = 淡出，1 = 淡入）")]
    [Range(0f, 1f)]
    public float targetAlpha = 0f;

    [Tooltip("动画持续时间（秒）")]
    public float duration = 1f;

    public override void OnEnter()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("[FadeTarget] 未指定目标对象");
            Continue();
            return;
        }

        Image image = targetObject.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogWarning("[FadeTarget] 目标对象没有 Image 组件");
            Continue();
            return;
        }

        image.DOFade(targetAlpha, duration)
             .SetUpdate(true) // 可选：在暂停时仍能播放（例如 UI 动画）
             .OnComplete(() => Continue());
    }

    public override string GetSummary()
    {
        if (targetObject == null)
            return "未设置目标对象";

        string action = targetAlpha <= 0f ? "淡出" : "淡入";
        return $"{action} {targetObject.name}，持续 {duration:F2} 秒";
    }

    public override Color GetButtonColor()
    {
        return new Color32(180, 200, 255, 255); // 浅蓝色
    }
}
