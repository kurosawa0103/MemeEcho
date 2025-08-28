using UnityEngine;

public class WaveTargetParam : MonoBehaviour
{
    [Header("滑块目标值A")]
    [Range(0f, 1f)]
    public float targetSliderValueA = 0.5f;

    [Header("黑幕判定")]
    public float maxFadeDistance = 5f;      // 最远距离时黑幕全黑
    public float fullClearDistance = 1f;    // 距离足够近时黑幕全透明

    [Header("达成成功时上报调用 Fungus")]
    public string successKey = "WavePointClear";

    [Header("目标点可视化（调试）")]
    public Color gizmoColor = Color.green;
    public float gizmoRadius = 0.2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}
