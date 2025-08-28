using UnityEngine;
using Fungus;
using DG.Tweening;

[CommandInfo("拓展",
             "ChangeScaleByTag",
             "改变带有指定 Tag 的物体的缩放，使用 DOTween 动画")]
public class ChangeScaleByTag : Command
{
    public enum ScaleMode
    {
        SetToValue,    // 设置为目标值
        AddToCurrent   // 在当前值上增加
    }

    [Tooltip("目标物体的 Tag")]
    [SerializeField] private string targetTag = "SubRoot";

    [Tooltip("缩放值（Set: 目标值 / Add: 增加值）")]
    [SerializeField] private Vector3 scaleValue = new Vector3(1, 1, 1);

    [Tooltip("过渡时间（秒）")]
    [SerializeField] private float duration = 1f;

    [Tooltip("是否等待动画完成再继续执行 Flowchart")]
    [SerializeField] private bool waitUntilFinished = true;

    [Tooltip("缩放模式")]
    [SerializeField] private ScaleMode scaleMode = ScaleMode.SetToValue;

    public override void OnEnter()
    {
        GameObject target = GameObject.FindGameObjectWithTag(targetTag);

        if (target == null)
        {
            Debug.LogWarning($"ChangeScaleByTag: 找不到 Tag 为 [{targetTag}] 的物体。");
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
        string modeDesc = scaleMode == ScaleMode.SetToValue ? "设置为" : "增加";
        return $"{modeDesc} [{targetTag}] 的缩放：{scaleValue}，耗时 {duration} 秒";
    }

    public override Color GetButtonColor()
    {
        return new Color32(135, 206, 250, 255); // 天蓝色
    }
}
