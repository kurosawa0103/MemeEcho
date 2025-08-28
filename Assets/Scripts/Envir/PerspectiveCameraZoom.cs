using UnityEngine;

public class PerspectiveCameraZoom : MonoBehaviour
{
    public Camera mainCamera; // 主相机
    public Transform targetObject; // 指定的目标物体
    public float zoomSpeed = 5f; // 缩放速度
    public float minZoom = 20f; // 最小视野角度 (FOV)
    public float maxZoom = 60f; // 最大视野角度 (FOV)
    public float lerpSpeed = 5f; // 平滑插值速度
    public LayerMask zoomLayerMask; // 只允许在这个层级上缩放
    public LimitArea limitArea; // 用于限制相机的边界

    public float targetFOV; // 目标视野角度
    public float percentage;
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (targetObject == null)
        {
            Debug.LogWarning("请指定目标物体！");
            enabled = false;
            return;
        }

        // 初始目标 FOV
        targetFOV = mainCamera.fieldOfView;
    }

    void Update()
    {
        // 使用鼠标滚轮控制缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetFOV = Mathf.Clamp(targetFOV - scroll * zoomSpeed, minZoom, maxZoom);
        }

        // 平滑插值 FOV
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * lerpSpeed);

        // 确保缩放时目标物体保持在屏幕上的相同位置
        AdjustCameraPosition();

        // 根据当前的 FOV 计算相机范围限制
        SetLimitArea();

        // 确保相机的位置不超出限制区域（即刻修正位置）
        ConstrainCameraPosition();
    }


    void AdjustCameraPosition()
    {
        // 获取缩放前目标物体的屏幕位置
        Vector3 preZoomScreenPos = mainCamera.WorldToScreenPoint(targetObject.position);

        // 获取缩放后目标物体的屏幕位置
        Vector3 postZoomScreenPos = mainCamera.WorldToScreenPoint(targetObject.position);

        // 计算屏幕坐标的偏移
        Vector3 screenOffset = preZoomScreenPos - postZoomScreenPos;

        // 将屏幕偏移转换为世界空间中的偏移
        Vector3 worldOffset = mainCamera.ScreenToWorldPoint(new Vector3(screenOffset.x, screenOffset.y, postZoomScreenPos.z))
                             - mainCamera.ScreenToWorldPoint(new Vector3(0, 0, postZoomScreenPos.z));

        // 调整相机的位置
        mainCamera.transform.position -= worldOffset;
    }
    void SetLimitArea()
    {
        // 根据当前的 FOV 计算百分比
        percentage = (targetFOV - minZoom) / (maxZoom - minZoom);

        // 根据百分比调整 limitArea 的 size，插值在 minSize 和 maxSize 之间
        limitArea.size = Vector2.Lerp(limitArea.minSize, limitArea.maxSize, percentage);
    }
    // 限制相机在指定区域内
    void ConstrainCameraPosition()
    {
        Vector3 currentPos = targetObject.position;

        // 获取限制区域的边界（以 center 和 size 来计算）
        float minX = limitArea.center.x - limitArea.size.x / 2;
        float maxX = limitArea.center.x + limitArea.size.x / 2;
        float minY = limitArea.center.y - limitArea.size.y / 2;
        float maxY = limitArea.center.y + limitArea.size.y / 2;

        // 限制相机位置
        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
        currentPos.y = Mathf.Clamp(currentPos.y, minY, maxY);

        // 如果超出限制，立刻修正回最近的合法位置
        targetObject.position = currentPos;
    }


}
