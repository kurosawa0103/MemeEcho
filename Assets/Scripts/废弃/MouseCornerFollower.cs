using UnityEngine;
using UnityEngine.UI;

public class MouseCornerFollower : MonoBehaviour
{
    // ���������ڷŴ��ƶ���Ӱ��
    public float multiplier = 0.1f;

    // ��¼��ʼλ��
    private Vector3 initialPosition;

    // �����ƶ�����
    public Vector2 moveDirection = new Vector2(1, 1);

    void Start()
    {
        // ��¼��ʼλ��
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // ��ȡ��ǰ֡�����λ��
        Vector3 currentMousePosition = Input.mousePosition;

        // ����UIͼƬ��ƫ����
        Vector3 offset = currentMousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

        // ����ƫ�����ķ���
        offset = Vector3.ClampMagnitude(offset, 100f);

        // �������õ��ƶ��������ƫ����
        offset.x *= moveDirection.x;
        offset.y *= moveDirection.y;

        // ��������ƶ��ľ������UIͼƬ��ƫ����
        Vector3 finalOffset = offset * multiplier;

        // ���ϳ�ʼλ��
        finalOffset += initialPosition;

        // Ӧ��ƫ������UIͼƬλ��
        transform.localPosition = finalOffset;
    }
}
