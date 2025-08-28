using UnityEngine;

public class DragSprite : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;

    public LimitArea limitArea;  // ���� LimitArea �ű�����ȡ�߽�

    void OnMouseDown()
    {
        // ��������������������ĵ�ƫ����
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
        offset = transform.parent.position - mouseWorldPos;
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // �����µ�λ��
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
            Vector3 newPosition = mouseWorldPos + offset;

            // ������������ı߽磨ʹ�� center �� size��
            float minX = limitArea.center.x - limitArea.size.x / 2;
            float maxX = limitArea.center.x + limitArea.size.x / 2;
            float minY = limitArea.center.y - limitArea.size.y / 2;
            float maxY = limitArea.center.y + limitArea.size.y / 2;

            // ��������λ���ڱ߽���
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.parent.position = newPosition;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}
