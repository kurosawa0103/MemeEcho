using UnityEngine;

public class FollowUI : MonoBehaviour
{
    public RectTransform uiElement;    // UI 组件
    public Transform worldSprite;      // 世界坐标系的精灵
    public Canvas canvas;              // UI 的 Canvas
    public Camera uiCamera;            // UI 摄像机（如果是 Overlay 模式则不需要）
    public bool matchSize = true;      // 是否匹配大小
    public SpriteRenderer spriteRenderer; // 精灵渲染器，用于调整大小
    private Vector3 lastScale = Vector3.zero;
    [Header("附加控制项")]
    public bool followRotation = false; // 是否跟随旋转
    private const float sizeThreshold = 0.001f; // 可以调整精度

    // 新增：控制尺寸边缘内缩的比例
    [Range(0f, 1f)] public float shrinkFactor = 0.05f; // 0.05 表示5%的内缩

    void Start()
    {
        // 如果没有手动指定 SpriteRenderer，则尝试在同一对象上获取
        if (spriteRenderer == null && worldSprite != null)
        {
            spriteRenderer = worldSprite.GetComponent<SpriteRenderer>();
        }

        if (worldSprite != null)
        {
            lastScale = worldSprite.localScale; // ✅ 初始化lastScale，避免首次误判
        }
    }

    void LateUpdate()
    {
        if (uiElement == null || worldSprite == null || canvas == null) return;

        // 更新位置
        UpdatePosition();

        // 更新大小
        if (matchSize && spriteRenderer != null)
        {
            UpdateSize();
        }

        if (followRotation)
        {
            UpdateRotation();
        }
    }

    void UpdatePosition()
    {
        Vector3 worldPos;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // 1️⃣ Screen Space - Overlay 模式
            worldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(uiElement.position.x, uiElement.position.y, -Camera.main.transform.position.z)
            );
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // 2️⃣ Screen Space - Camera 模式
            worldPos = uiCamera.ScreenToWorldPoint(
                RectTransformUtility.WorldToScreenPoint(uiCamera, uiElement.position)
            );
        }
        else
        {
            // 3️⃣ World Space 模式
            worldPos = uiElement.position; // 直接使用 UI 的世界坐标
        }

        // 确保 Z 轴适配，防止精灵跑偏
        worldPos.z = worldSprite.position.z; // 保持原有的z值

        // 更新精灵的位置
        worldSprite.position = worldPos;
    }
    void UpdateRotation()
    {
        // UI元素的世界旋转
        Quaternion uiRotation = uiElement.rotation;

        // 应用到世界精灵上
        worldSprite.rotation = uiRotation;
    }
    void UpdateSize()
    {
        if (spriteRenderer == null) return;

        // 获取UI元素的实际大小（考虑缩放）
        Vector2 uiSize = new Vector2(
            uiElement.rect.width * uiElement.lossyScale.x,
            uiElement.rect.height * uiElement.lossyScale.y
        );

        // 计算世界空间中的大小
        Vector2 worldSize = Vector2.zero;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // 将屏幕大小转换为世界大小
            Vector3 worldPoint1 = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
            Vector3 worldPoint2 = Camera.main.ScreenToWorldPoint(new Vector3(uiSize.x, uiSize.y, -Camera.main.transform.position.z));
            worldSize = new Vector2(Mathf.Abs(worldPoint2.x - worldPoint1.x), Mathf.Abs(worldPoint2.y - worldPoint1.y));
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // 使用UI相机计算世界大小
            float distanceToCamera = Mathf.Abs(uiCamera.transform.position.z - worldSprite.position.z);
            worldSize.x = uiSize.x * distanceToCamera / uiCamera.orthographicSize / 2f;
            worldSize.y = uiSize.y * distanceToCamera / uiCamera.orthographicSize / 2f;
        }
        else
        {
            // World Space 模式下直接使用 UI 大小
            worldSize = uiSize;
        }

        // 新增：在计算大小时加入 shrinkFactor
        worldSize *= (1 - shrinkFactor); // 使尺寸缩小 shrinkFactor 百分比

        // 获取精灵的原始大小
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // 计算需要的缩放比例
        Vector3 newScale = new Vector3(
            worldSize.x / spriteSize.x,
            worldSize.y / spriteSize.y,
            worldSprite.localScale.z // 保持Z轴缩放不变
        );

        // 判断尺寸变化是否大于阈值
        if (Vector3.Distance(lastScale, newScale) > sizeThreshold)
        {
            worldSprite.localScale = newScale;
            lastScale = newScale;
        }
    }
}