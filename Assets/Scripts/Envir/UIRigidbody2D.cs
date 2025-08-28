using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIRigidbody2D : MonoBehaviour
{
    public float gravity = -1000f;
    public float bounceDamping = 0.7f;
    public float drag = 0.99f;
    public float mass = 1f;
    public Vector2 velocity;

    [Header("��������")]
    public float inertiaMultiplier = 1f; // ��ק�ٶȳ��������Ե��ڹ�������

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
            Debug.LogError("UIRigidbody2D ������� Screen Space �� Overlay ģʽ�� Canvas �У�");
            enabled = false;
            return;
        }

        canvasRect = canvas.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isPaused) return;

        float dt = Time.deltaTime;

        // ����
        velocity.y += gravity / mass * dt;

        // ����
        velocity *= drag;

        // λ�ø���
        Vector2 pos = rectTransform.anchoredPosition;
        pos += velocity * dt;

        // �߽���ײ
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
