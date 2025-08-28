using UnityEngine;

public class UIFollowWorldObject : MonoBehaviour
{
    public Transform worldObject;    // ���������е�����
    public RectTransform uiElement;  // ����� UI Ԫ��
    public Camera mainCamera;        // ����ת����������
    public bool followPlayer;        // �Ƿ��Զ����� Player
    public Vector3 offset;           // ƫ��������λ���������꣩

    private void Start()
    {
        // ��ȡ�����е������
        mainCamera = Camera.main;

        if (followPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                worldObject = player.transform.Find("���ݹ��ص�");
            }
        }
    }

    void Update()
    {
        if (worldObject == null || uiElement == null || mainCamera == null)
            return;

        // ����Ӧ��ƫ�ƺ����������
        Vector3 adjustedWorldPosition = worldObject.position + offset;

        // ����������ת��Ϊ��Ļ����
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(adjustedWorldPosition);

        // ����Ļ����ת��Ϊ UI ���꣨��Ϊ UI Ԫ���� Canvas �У�
        uiElement.position = screenPosition;
    }

}
