using UnityEngine;

public class MeteorController : MonoBehaviour
{
    public float speed = 5f; // 流星的移动速度
    public Vector3 direction = Vector3.down; // 流星的移动方向
    public float jitterFrequency = 1f; // 抖动频率
    public float jitterMagnitude = 0.1f; // 抖动幅度

    private Vector3 originalPosition;
    public float destroyHeight = -10f; // 流星销毁的 Y 坐标

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
       // 移动流星
        transform.Translate(direction * speed * Time.deltaTime);

        // 如果流星到达了目标点位，销毁它
        if (transform.position.y <= destroyHeight)
        {
            Destroy(gameObject);
        }
        // 添加随机抖动
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
