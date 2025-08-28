using UnityEngine;

public class UIFollowWorldObject : MonoBehaviour
{
    public Transform worldObject;    // 世界坐标中的物体
    public RectTransform uiElement;  // 跟随的 UI 元素
    public Camera mainCamera;        // 用于转换坐标的相机
    public bool followPlayer;        // 是否自动跟随 Player
    public Vector3 offset;           // 偏移量（单位：世界坐标）

    private void Start()
    {
        // 获取场景中的主相机
        mainCamera = Camera.main;

        if (followPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                worldObject = player.transform.Find("气泡挂载点");
            }
        }
    }

    void Update()
    {
        if (worldObject == null || uiElement == null || mainCamera == null)
            return;

        // 计算应用偏移后的世界坐标
        Vector3 adjustedWorldPosition = worldObject.position + offset;

        // 将世界坐标转换为屏幕坐标
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(adjustedWorldPosition);

        // 把屏幕坐标转换为 UI 坐标（因为 UI 元素在 Canvas 中）
        uiElement.position = screenPosition;
    }

}
