using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitArea : MonoBehaviour
{
    public Vector2 center = new Vector2(0, 0);  // 中心点
    public Vector2 size = new Vector2(10, 5);   // 宽高尺寸（X 和 Y）
    public Vector2 minSize = new Vector2(10, 5);   // 宽高尺寸（X 和 Y）
    public Vector2 maxSize = new Vector2(10, 5);   // 宽高尺寸（X 和 Y）
    private void OnDrawGizmos()
    {
        // 设置 Gizmo 的颜色
        Gizmos.color = Color.green;

        // 计算矩形的范围（以中心点和宽高为基础）
        Rect boundary = new Rect(center - size / 2, size);

        // 绘制矩形边界
        Gizmos.DrawWireCube(boundary.center, boundary.size);
    }
}
