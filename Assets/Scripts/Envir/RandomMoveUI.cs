using UnityEngine;

public class RandomMoveUI : MonoBehaviour
{
    public RectTransform rectTransform; // UI元素的RectTransform组件
    public RectTransform movementBounds; // 限制移动范围的RectTransform
    public float moveSpeed = 100f; // 移动速度
    private Vector2 moveDirection; // 移动方向

    private void Start()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        // 初始化随机移动方向
        moveDirection = GetRandomDirection();
    }

    private void Update()
    {
        MoveUIElement();
    }

    private void MoveUIElement()
    {
        // 移动UI元素
        rectTransform.anchoredPosition += moveDirection * moveSpeed * Time.deltaTime;

        // 检查是否撞到边界
        if (!IsWithinBounds())
        {
            // 如果撞到边界，改变方向
            moveDirection = GetRandomDirection();
            ClampPositionWithinBounds();
        }
    }

    private Vector2 GetRandomDirection()
    {
        // 获取一个随机方向
        float angle = Random.Range(0f, 360f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    private bool IsWithinBounds()
    {
        // 检查UI元素是否在指定范围内
        Vector2 pos = rectTransform.anchoredPosition;
        Vector2 minBounds = movementBounds.rect.min + rectTransform.rect.size / 2;
        Vector2 maxBounds = movementBounds.rect.max - rectTransform.rect.size / 2;

        return pos.x >= minBounds.x && pos.x <= maxBounds.x && pos.y >= minBounds.y && pos.y <= maxBounds.y;
    }

    private void ClampPositionWithinBounds()
    {
        // 将UI元素位置限制在指定范围内
        Vector2 pos = rectTransform.anchoredPosition;
        Vector2 minBounds = movementBounds.rect.min + rectTransform.rect.size / 2;
        Vector2 maxBounds = movementBounds.rect.max - rectTransform.rect.size / 2;

        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);

        rectTransform.anchoredPosition = pos;
    }

    private void OnDrawGizmos()
    {
        if (movementBounds != null)
        {
            Gizmos.color = Color.green;
            Vector3 boundsCenter = movementBounds.position;
            Vector3 boundsSize = movementBounds.rect.size;
            Gizmos.DrawWireCube(boundsCenter, boundsSize);
        }
    }
}
