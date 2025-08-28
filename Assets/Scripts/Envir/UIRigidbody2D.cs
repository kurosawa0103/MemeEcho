using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIRigidbody2D : MonoBehaviour
{
    public float gravity = -1000f;
    public float bounceDamping = 0.7f;
    public float drag = 0.99f;
    public float mass = 1f;
    public Vector2 velocity;

    [Header("惯性配置")]
    public float inertiaMultiplier = 1f; // 拖拽速度乘数，可以调节惯性力度

    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private bool isPaused = false;

    public void Pause() => isPaused = true;
    public void Resume() => isPaused = false;
    public void SetVelocity(Vector2 newVelocity) => velocity = newVelocity;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null || canvas.renderMode == RenderMode.WorldSpace)
        {
            Debug.LogError("UIRigidbody2D 必须放在 Screen Space 或 Overlay 模式的 Canvas 中！");
            enabled = false;
            return;
        }

        canvasRect = canvas.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isPaused) return;

        float dt = Time.deltaTime;

        // 重力
        velocity.y += gravity / mass * dt;

        // 阻力
        velocity *= drag;

        // 位置更新
        Vector2 pos = rectTransform.anchoredPosition;
        pos += velocity * dt;

        // 边界碰撞
        Vector2 halfSize = rectTransform.rect.size * 0.5f;
        Vector2 canvasSize = canvasRect.rect.size;

        if (pos.x - halfSize.x < -canvasSize.x * 0.5f)
        {
            pos.x = -canvasSize.x * 0.5f + halfSize.x;
            velocity.x *= -bounceDamping;
        }
        else if (pos.x + halfSize.x > canvasSize.x * 0.5f)
        {
            pos.x = canvasSize.x * 0.5f - halfSize.x;
            velocity.x *= -bounceDamping;
        }

        if (pos.y - halfSize.y < -canvasSize.y * 0.5f)
        {
            pos.y = -canvasSize.y * 0.5f + halfSize.y;
            velocity.y *= -bounceDamping;
        }
        else if (pos.y + halfSize.y > canvasSize.y * 0.5f)
        {
            pos.y = canvasSize.y * 0.5f - halfSize.y;
            velocity.y *= -bounceDamping;
        }

        rectTransform.anchoredPosition = pos;
    }
}
