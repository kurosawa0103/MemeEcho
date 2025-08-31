using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementSystem : MonoBehaviour
{
    public float moveSpeed = 5f;  // 移动速度
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 moveVelocity;
    private bool movementEnabled = true; // 是否允许移动

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!movementEnabled)
        {
            moveVelocity = Vector2.zero;
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;
        moveVelocity = moveInput * moveSpeed;
    }

    void FixedUpdate()
    {
        rb.velocity = moveVelocity;
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        if (!enabled)
        {
            rb.velocity = Vector2.zero; // 停止移动
        }
    }
}
