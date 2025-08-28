using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 挂在任意物体上，在 Update 中持续计算 targetRect 围绕屏幕中心的累积角度（顺时针为正）
/// </summary>
public class UIOrbitAngleTracker : MonoBehaviour
{
    [Header("必填 —— 被拖拽的 UI 物体")]
    [SerializeField] private RectTransform targetRect;

    [Header("可选 —— 如果 Canvas 不是 Screen Space - Overlay，填入 Canvas")]
    [SerializeField] private Canvas targetCanvas;

    [Header("可选 —— 用于在界面上显示数值")]
    [SerializeField] private Text debugText;

    /// <summary>顺时针累积角度（单位：°）</summary>
    public float CumulativeCWAngle { get; private set; }

    Vector2 screenCenter;      // 屏幕中心（像素坐标）
    float lastAngleCCW;        // 上一帧的 CCW（逆时针）角度
    bool hasLastAngle;         // 是否已经记录过上一帧

    Camera canvasCam;          // 非 Overlay Canvas 时需要提供相机
    /// <summary>角速度事件：参数 = 顺时针增量角度（°）。正 = 前进，负 = 倒退</summary>
    public event System.Action<float> OnAngleDeltaCW;
    void Start()
    {
        screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        if (targetCanvas && targetCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            canvasCam = targetCanvas.worldCamera;
        else
            canvasCam = null;  // Overlay 模式不需要相机
    }

    void Update()
    {
        if (!targetRect) return;

        // 将 UI 世界坐标转换到屏幕坐标
        Vector2 currentScreenPos = RectTransformUtility.WorldToScreenPoint(canvasCam, targetRect.position);
        Vector2 dirFromCenter = currentScreenPos - screenCenter;

        // 若刚好在中心则无法定义角度
        if (dirFromCenter.sqrMagnitude < 1e-6f) return;

        // 以 +X 轴为 0°，逆时针为正得到当前瞬时角
        float angleCCW = Mathf.Atan2(dirFromCenter.y, dirFromCenter.x) * Mathf.Rad2Deg;

        if (!hasLastAngle)
        {
            lastAngleCCW = angleCCW;
            hasLastAngle = true;
            return; // 需要有前一帧才能算增量
        }

        // Δ = 当前 - 上一帧（取最短路径，范围 [-180, 180]）
        float deltaCCW = Mathf.DeltaAngle(lastAngleCCW, angleCCW);
        lastAngleCCW = angleCCW;

        // 题目要求：顺时针为正，所以把逆时针正值改为负、顺时针负值改为正
        CumulativeCWAngle -= deltaCCW;
        float deltaCW = -deltaCCW;        // 转为顺时针角增量
        CumulativeCWAngle += deltaCW;

        // 广播给外部
        OnAngleDeltaCW?.Invoke(deltaCW);

        // —— Debug 输出 —— //
        Debug.Log($"CW Sum: {CumulativeCWAngle:F2}°   ΔFrame: {-deltaCCW:F2}°");

        if (debugText) debugText.text = $"{CumulativeCWAngle:F1}°";
    }

    /// <summary>可在拖拽开始时调用，用来清零累计值</summary>
    public void ResetAngle()
    {
        CumulativeCWAngle = 0;
        hasLastAngle = false;
    }
}
