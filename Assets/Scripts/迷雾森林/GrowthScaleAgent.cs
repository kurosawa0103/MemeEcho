using UnityEngine;

/// <summary>
/// 随 “时间” 把物体从 minScale → maxScale（0→1）
/// 把脚本挂到需要生长/缩小的物体，并在 TimeFlowController 的 targets 中注册
/// </summary>
public class GrowthScaleAgent : MonoBehaviour, ITimeAgent
{
    [Header("缩放范围")]
    [SerializeField] private Vector3 minScale = Vector3.zero;   // progress = 0
    [SerializeField] private Vector3 maxScale = Vector3.one;    // progress = 1

    [Header("完成 0→1 所需秒数")]
    [SerializeField] private float cycleSeconds = 5f;

    [Range(0f, 1f)]
    [Tooltip("当前进度（0~1），运行时由 ApplyTime() 推进/回退")]
    public float progress;

    /* ---------- ITimeAgent ---------- */
    public void ApplyTime(float timeDelta)
    {
        if (cycleSeconds <= 0f) return; // 防止除以 0

        // 把秒数映射到 progress，正向增长，负向回退
        progress = Mathf.Clamp01(progress + timeDelta / cycleSeconds);

        // 根据 progress 更新缩放
        UpdateScale();
    }

    void UpdateScale()
    {
        transform.localScale = Vector3.LerpUnclamped(minScale, maxScale, progress);
    }

#if UNITY_EDITOR
    // 在 Inspector 调整数值时即时更新
    void OnValidate() => UpdateScale();

    [ContextMenu("设为当前缩放值 → MaxScale")]
    void SetMaxScaleToCurrent()
    {
        maxScale = transform.localScale;
        Debug.Log($"[GrowthScaleAgent] 已将 MaxScale 设置为当前缩放值：{maxScale}", this);
    }

    [ContextMenu("设为当前缩放值 → MinScale")]
    void SetMinScaleToCurrent()
    {
        minScale = transform.localScale;
        Debug.Log($"[GrowthScaleAgent] 已将 MinScale 设置为当前缩放值：{minScale}", this);
    }
#endif
}
