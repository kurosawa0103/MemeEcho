using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementSystem : MonoBehaviour
{
    public float moveSpeed = 5f;  // 移动速度

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 moveVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 获取输入：WASD 或方向键
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;  // 保持对角线不会更快
        moveVelocity = moveInput * moveSpeed;
    }

    void FixedUpdate()
    {
        rb.velocity = moveVelocity;  // 应用速度向量
    }
}
