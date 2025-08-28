using UnityEngine;

public class MeteorController : MonoBehaviour
{
    public float speed = 5f; // ���ǵ��ƶ��ٶ�
    public Vector3 direction = Vector3.down; // ���ǵ��ƶ�����
    public float jitterFrequency = 1f; // ����Ƶ��
    public float jitterMagnitude = 0.1f; // ��������

    private Vector3 originalPosition;
    public float destroyHeight = -10f; // �������ٵ� Y ����

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
       // �ƶ�����
        transform.Translate(direction * speed * Time.deltaTime);

        // ������ǵ�����Ŀ���λ��������
        if (transform.position.y <= destroyHeight)
        {
            Destroy(gameObject);
        }
        // ����������
        ApplyJitter();
    }

    void ApplyJitter()
    {
        float jitterAmount = Mathf.PerlinNoise(Time.time * jitterFrequency, 0f) * jitterMagnitude;
        Vector3 jitter = new Vector3(Random.Range(-jitterAmount, jitterAmount),
                                     Random.Range(-jitterAmount, jitterAmount),
                                     Random.Range(-jitterAmount, jitterAmount));
        transform.position += jitter;
    }
}
