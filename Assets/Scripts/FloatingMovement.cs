using UnityEngine;

public class FloatingMovement : MonoBehaviour
{
    public Transform centerPoint; // ���ĵ㣨ͨ���ǹ��ص㣩

    public float amplitudeX = 0.5f; // ���Ҹ�������
    public float amplitudeY = 0.3f; // ���¸�������
    public float frequencyX = 1.5f; // ���Ҹ���Ƶ��
    public float frequencyY = 2.0f; // ���¸���Ƶ��
    public float rotationAmount = 10f; // ��ת�Ƕ�ģ�����

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

        // ����λ��
        Vector3 floatPosition = centerPoint.position + initialOffset + new Vector3(offsetX, offsetY, 0f);
        transform.position = floatPosition;

        // ģ������ʱ����΢��ת�����У�
        float rotationZ = Mathf.Sin(Time.time * frequencyY) * rotationAmount;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }
}