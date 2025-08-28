using UnityEngine;

public class HeartBeat : MonoBehaviour
{
    [Header("心跳参数")]
    public float beatDuration = 1.0f;           // 一整个跳动周期的时间（秒）
    public float scaleAmount = 0.2f;            // 缩放幅度
    public AnimationCurve beatCurve;            // 控制跳动节奏和形状

    private Vector3 originalScale;
    private float timer;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (beatCurve == null || beatCurve.length == 0) return;

        // 时间在 [0, 1] 之间循环
        timer += Time.deltaTime / beatDuration;
        if (timer > 1f) timer -= 1f;

        float curveValue = beatCurve.Evaluate(timer); // 获取当前跳动强度
        transform.localScale = originalScale + Vector3.one * curveValue * scaleAmount;
    }
}