using UnityEngine;

public class DragSprite : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;

    public LimitArea limitArea;  // 引用 LimitArea 脚本来获取边界

    void OnMouseDown()
    {
        // 计算鼠标点击点与物体中心的偏移量
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
        offset = transform.parent.position - mouseWorldPos;
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // 计算新的位置
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
            Vector3 newPosition = mouseWorldPos + offset;

            // 计算限制区域的边界（使用 center 和 size）
            float minX = limitArea.center.x - limitArea.size.x / 2;
            float maxX = limitArea.center.x + limitArea.size.x / 2;
            float minY = limitArea.center.y - limitArea.size.y / 2;
            float maxY = limitArea.center.y + limitArea.size.y / 2;

            // 限制物体位置在边界内
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.parent.position = newPosition;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}
