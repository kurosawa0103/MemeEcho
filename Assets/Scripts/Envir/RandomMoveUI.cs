using UnityEngine;

public class RandomMoveUI : MonoBehaviour
{
    public RectTransform rectTransform; // UIԪ�ص�RectTransform���
    public RectTransform movementBounds; // �����ƶ���Χ��RectTransform
    public float moveSpeed = 100f; // �ƶ��ٶ�
    private Vector2 moveDirection; // �ƶ�����

    private void Start()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        // ��ʼ������ƶ�����
        moveDirection = GetRandomDirection();
    }

    private void Update()
    {
        MoveUIElement();
    }

    private void MoveUIElement()
    {
        // �ƶ�UIԪ��
        rectTransform.anchoredPosition += moveDirection * moveSpeed * Time.deltaTime;

        // ����Ƿ�ײ���߽�
        if (!IsWithinBounds())
        {
            // ���ײ���߽磬�ı䷽��
            moveDirection = GetRandomDirection();
            ClampPositionWithinBounds();
        }
    }

    private Vector2 GetRandomDirection()
    {
        // ��ȡһ���������
        float angle = Random.Range(0f, 360f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    private bool IsWithinBounds()
    {
        // ���UIԪ���Ƿ���ָ����Χ��
        Vector2 pos = rectTransform.anchoredPosition;
        Vector2 minBounds = movementBounds.rect.min + rectTransform.rect.size / 2;
        Vector2 maxBounds = movementBounds.rect.max - rectTransform.rect.size / 2;

        return pos.x >= minBounds.x && pos.x <= maxBounds.x && pos.y >= minBounds.y && pos.y <= maxBounds.y;
    }

    private void ClampPositionWithinBounds()
    {
        // ��UIԪ��λ��������ָ����Χ��
        Vector2 pos = rectTransform.anchoredPosition;
        Vector2 minBounds = movementBounds.rect.min + rectTransform.rect.size / 2;
        Vector2 maxBounds = movementBounds.rect.max - rectTransform.rect.size / 2;

        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);

        rectTransform.anchoredPosition = pos;
    }

    private void OnDrawGizmos()
    {
        if (movementBounds != null)
        {
            Gizmos.color = Color.green;
            Vector3 boundsCenter = movementBounds.position;
            Vector3 boundsSize = movementBounds.rect.size;
            Gizmos.DrawWireCube(boundsCenter, boundsSize);
        }
    }
}
