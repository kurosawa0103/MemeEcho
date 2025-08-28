using UnityEngine;

public class TrailRendererMask : MonoBehaviour
{
    public TrailRenderer trailRenderer;  // TrailRenderer 组件
    public Vector3 minBounds;           // 范围的最小边界
    public Vector3 maxBounds;           // 范围的最大边界

    // 在编辑器中绘制生成范围的可视化区域
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue; // 设置颜色为蓝色

        // 计算范围的中心和大小
        Vector3 center = (minBounds + maxBounds) / 2;
        Vector3 size = maxBounds - minBounds;

        // 绘制一个线框立方体，表示范围
        Gizmos.DrawWireCube(center, size);
    }

    void Update()
    {
        if (trailRenderer != null)
        {
            // 获取 TrailRenderer 当前所有的轨迹点
            Vector3[] trailPositions = new Vector3[trailRenderer.positionCount];
            trailRenderer.GetPositions(trailPositions);

            for (int i = 0; i < trailPositions.Length; i++)
            {
                if (IsPointOutsideBounds(trailPositions[i]))
                {
                    // 如果点超出范围，清除该点
                    trailPositions[i] = Vector3.zero; // 你可以将点设置为无效位置，或者直接删除
                }
            }

            // 更新 TrailRenderer 的点
            trailRenderer.SetPositions(trailPositions);
        }
    }

    // 检查点是否在指定范围外
    bool IsPointOutsideBounds(Vector3 point)
    {
        return point.x < minBounds.x || point.x > maxBounds.x ||
               point.y < minBounds.y || point.y > maxBounds.y ||
               point.z < minBounds.z || point.z > maxBounds.z;
    }
}
