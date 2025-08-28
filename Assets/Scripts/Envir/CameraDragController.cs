using UnityEngine;

public class CameraDragController : MonoBehaviour
{
    public float dragSpeed = 0.005f;
    public Vector2 xLimit = new Vector2(-3f, 3f);  // ������������
    public Vector2 yLimit = new Vector2(-3f, 3f);

    private Vector3 lastMousePosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            // �϶��������
            Vector3 move = new Vector3(-delta.x * dragSpeed, -delta.y * dragSpeed, 0f);
            Vector3 targetPos = transform.position + move;

            // �ھ����������������� X/Y ��Χ
            targetPos.x = Mathf.Clamp(targetPos.x, xLimit.x, xLimit.y);
            targetPos.y = Mathf.Clamp(targetPos.y, yLimit.x, yLimit.y);
            targetPos.z = transform.position.z;

            transform.position = targetPos;
        }
    }
}
