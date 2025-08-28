using UnityEngine;

public class PerspectiveCameraZoom : MonoBehaviour
{
    public Camera mainCamera; // �����
    public Transform targetObject; // ָ����Ŀ������
    public float zoomSpeed = 5f; // �����ٶ�
    public float minZoom = 20f; // ��С��Ұ�Ƕ� (FOV)
    public float maxZoom = 60f; // �����Ұ�Ƕ� (FOV)
    public float lerpSpeed = 5f; // ƽ����ֵ�ٶ�
    public LayerMask zoomLayerMask; // ֻ����������㼶������
    public LimitArea limitArea; // ������������ı߽�

    public float targetFOV; // Ŀ����Ұ�Ƕ�
    public float percentage;
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (targetObject == null)
        {
            Debug.LogWarning("��ָ��Ŀ�����壡");
            enabled = false;
            return;
        }

        // ��ʼĿ�� FOV
        targetFOV = mainCamera.fieldOfView;
    }

    void Update()
    {
        // ʹ�������ֿ�������
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetFOV = Mathf.Clamp(targetFOV - scroll * zoomSpeed, minZoom, maxZoom);
        }

        // ƽ����ֵ FOV
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * lerpSpeed);

        // ȷ������ʱĿ�����屣������Ļ�ϵ���ͬλ��
        AdjustCameraPosition();

        // ���ݵ�ǰ�� FOV ���������Χ����
        SetLimitArea();

        // ȷ�������λ�ò������������򣨼�������λ�ã�
        ConstrainCameraPosition();
    }


    void AdjustCameraPosition()
    {
        // ��ȡ����ǰĿ���������Ļλ��
        Vector3 preZoomScreenPos = mainCamera.WorldToScreenPoint(targetObject.position);

        // ��ȡ���ź�Ŀ���������Ļλ��
        Vector3 postZoomScreenPos = mainCamera.WorldToScreenPoint(targetObject.position);

        // ������Ļ�����ƫ��
        Vector3 screenOffset = preZoomScreenPos - postZoomScreenPos;

        // ����Ļƫ��ת��Ϊ����ռ��е�ƫ��
        Vector3 worldOffset = mainCamera.ScreenToWorldPoint(new Vector3(screenOffset.x, screenOffset.y, postZoomScreenPos.z))
                             - mainCamera.ScreenToWorldPoint(new Vector3(0, 0, postZoomScreenPos.z));

        // ���������λ��
        mainCamera.transform.position -= worldOffset;
    }
    void SetLimitArea()
    {
        // ���ݵ�ǰ�� FOV ����ٷֱ�
        percentage = (targetFOV - minZoom) / (maxZoom - minZoom);

        // ���ݰٷֱȵ��� limitArea �� size����ֵ�� minSize �� maxSize ֮��
        limitArea.size = Vector2.Lerp(limitArea.minSize, limitArea.maxSize, percentage);
    }
    // ���������ָ��������
    void ConstrainCameraPosition()
    {
        Vector3 currentPos = targetObject.position;

        // ��ȡ��������ı߽磨�� center �� size �����㣩
        float minX = limitArea.center.x - limitArea.size.x / 2;
        float maxX = limitArea.center.x + limitArea.size.x / 2;
        float minY = limitArea.center.y - limitArea.size.y / 2;
        float maxY = limitArea.center.y + limitArea.size.y / 2;

        // �������λ��
        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
        currentPos.y = Mathf.Clamp(currentPos.y, minY, maxY);

        // ����������ƣ���������������ĺϷ�λ��
        targetObject.position = currentPos;
    }


}
