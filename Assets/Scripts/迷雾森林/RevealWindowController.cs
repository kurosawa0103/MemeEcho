using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class RevealWindowController : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 10f;           // 视框跟随速度
    [Header("推开参数")]
    public float pushForce = 4f;            // 每片叶子的冲量大小
    public float upwardSpin = 0.5f;         // 叶片附加扭矩

    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        // 例：用鼠标或触控位置驱动视框，亦可替换成键盘/手柄坐标
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rb.MovePosition(Vector2.Lerp(rb.position, target, moveSpeed * Time.deltaTime));
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.rigidbody || !collision.collider.CompareTag("Leaf")) return;

        foreach (ContactPoint2D cp in collision.contacts)
        {
            Rigidbody2D leafRb = collision.rigidbody;
            Vector2 dir = (leafRb.position - rb.position).normalized;

            // 冲量：一次即可，避免每帧叠加；用相对速度判断
            if (Vector2.Dot(leafRb.velocity, dir) < 0.1f)
                leafRb.AddForce(dir * pushForce, ForceMode2D.Impulse);

            // 随机小旋转，增加自然感
            leafRb.AddTorque(Random.Range(-1f, 1f) * upwardSpin, ForceMode2D.Impulse);
        }
    }
}