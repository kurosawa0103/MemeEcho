using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementSystem : MonoBehaviour
{
    public float moveSpeed = 5f;  // �ƶ��ٶ�

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 moveVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ��ȡ���룺WASD �����
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;  // ���ֶԽ��߲������
        moveVelocity = moveInput * moveSpeed;
    }

    void FixedUpdate()
    {
        rb.velocity = moveVelocity;  // Ӧ���ٶ�����
    }
}
