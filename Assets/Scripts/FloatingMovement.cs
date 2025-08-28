using UnityEngine;

public class FloatingMovement : MonoBehaviour
{
    public Transform centerPoint; // 中心点（通常是挂载点）

    public float amplitudeX = 0.5f; // 左右浮动幅度
    public float amplitudeY = 0.3f; // 上下浮动幅度
    public float frequencyX = 1.5f; // 左右浮动频率
    public float frequencyY = 2.0f; // 上下浮动频率
    public float rotationAmount = 10f; // 旋转角度模拟振翅

    private Vector3 initialOffset;

    void Start()
    {
        if (centerPoint == null)
        {
            centerPoint = transform.parent;
        }
        initialOffset = transform.position - centerPoint.position;
    }

    void Update()
    {
        if (centerPoint == null) return;

        float offsetX = Mathf.Sin(Time.time * frequencyX) * amplitudeX;
        float offsetY = Mathf.Cos(Time.time * frequencyY) * amplitudeY;

        // 设置位置
        Vector3 floatPosition = centerPoint.position + initialOffset + new Vector3(offsetX, offsetY, 0f);
        transform.position = floatPosition;

        // 模拟悬浮时的轻微旋转（振翅感）
        float rotationZ = Mathf.Sin(Time.time * frequencyY) * rotationAmount;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }
}