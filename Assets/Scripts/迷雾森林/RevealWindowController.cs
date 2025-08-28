using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class RevealWindowController : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float moveSpeed = 10f;           // �ӿ�����ٶ�
    [Header("�ƿ�����")]
    public float pushForce = 4f;            // ÿƬҶ�ӵĳ�����С
    public float upwardSpin = 0.5f;         // ҶƬ����Ť��

    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        // ���������򴥿�λ�������ӿ�����滻�ɼ���/�ֱ�����
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

            // ������һ�μ��ɣ�����ÿ֡���ӣ�������ٶ��ж�
            if (Vector2.Dot(leafRb.velocity, dir) < 0.1f)
                leafRb.AddForce(dir * pushForce, ForceMode2D.Impulse);

            // ���С��ת��������Ȼ��
            leafRb.AddTorque(Random.Range(-1f, 1f) * upwardSpin, ForceMode2D.Impulse);
        }
    }
}