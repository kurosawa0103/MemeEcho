using UnityEngine;
using System.Collections.Generic;

public class UILeafController : MonoBehaviour
{
    [Header("UI设置")]
    public RectTransform uiTarget; // 目标UI物体
    public Canvas targetCanvas; // UI所在的Canvas

    [Header("推开设置")]
    public float clearPadding = 0.5f; // 清除区域内边距
    public float pushDistance = 3f; // 推开距离
    public float stabilityBuffer = 1f; // 稳定性缓冲区，防止频繁切换
    public LayerMask leafLayer = 1 << 8; // 叶子图层

    [Header("性能设置")]
    public float updateInterval = 0.1f; // 更新间隔

    [Header("调试设置")]
    public bool useAlternativeConversion = false; // 是否使用备用坐标转换方法
    public bool useSimpleConversion = true; // 是否使用简化坐标转换方法
    public bool enableDebugLog = false; // 启用调试日志

    private Camera mainCamera;
    private Vector3 worldPosition;
    private Vector2 uiSize; // UI矩形的实际尺寸
    private Rect clearRect; // 清除区域的矩形
    private Rect stabilityRect; // 稳定性检测区域（更大的矩形）
    private List<Leaf> allLeaves = new List<Leaf>();
    private HashSet<Leaf> pushedLeaves = new HashSet<Leaf>();
    private Dictionary<Leaf, Vector3> leafTargetPositions = new Dictionary<Leaf, Vector3>(); // 记录每个叶子的目标位置

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        mainCamera = Camera.main;

        // 如果没有指定Canvas，尝试从UI目标获取
        if (targetCanvas == null && uiTarget != null)
        {
            targetCanvas = uiTarget.GetComponentInParent<Canvas>();
        }

        // 找到所有叶子
        FindAllLeaves();

        // 开始定期更新
        InvokeRepeating(nameof(UpdateLeafPositions), 0f, updateInterval);
    }

    void FindAllLeaves()
    {
        allLeaves.Clear();

        // 通过图层查找所有叶子
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (((1 << obj.layer) & leafLayer) != 0)
            {
                Leaf leaf = obj.GetComponent<Leaf>();
                if (leaf != null)
                {
                    allLeaves.Add(leaf);
                }
            }
        }

        Debug.Log($"找到 {allLeaves.Count} 片叶子");
    }

    void UpdateLeafPositions()
    {
        if (uiTarget == null) return;

        // 获取UI的世界坐标和尺寸
        UpdateUIWorldPositionAndSize();

        if (enableDebugLog)
        {
            Debug.Log($"=== 坐标转换调试 ===");
            Debug.Log($"UI RectTransform.position: {uiTarget.position}");
            Debug.Log($"UI anchoredPosition: {uiTarget.anchoredPosition}");
            Debug.Log($"UI Rect Size: {uiTarget.rect.size}");
            Debug.Log($"UI World Size: {uiSize}");
            Debug.Log($"Canvas RenderMode: {targetCanvas.renderMode}");
            Debug.Log($"转换后世界坐标: {worldPosition}");
            Debug.Log($"清除矩形区域: {clearRect}");
        }

        // 检查每片叶子
        List<Leaf> leavesToReturn = new List<Leaf>(pushedLeaves);

        foreach (var leaf in allLeaves)
        {
            if (leaf == null) continue;

            Vector3 leafOriginalPos = leaf.GetOriginalPosition();

            // 检查叶子是否在矩形清除区域内
            if (IsPointInRect(leafOriginalPos, clearRect))
            {
                // 叶子在清除区域内，需要推开
                Vector3 pushTarget = CalculateRectangularPushTarget(leafOriginalPos, clearRect);

                leaf.PushTo(pushTarget);
                pushedLeaves.Add(leaf);
                leavesToReturn.Remove(leaf);

                if (enableDebugLog)
                {
                    Debug.Log($"推开叶子 {leaf.name}: 从 {leafOriginalPos} 推到 {pushTarget}");
                }
            }
        }

        // 让不再需要推开的叶子回到原位
        foreach (var leaf in leavesToReturn)
        {
            if (leaf != null)
            {
                leaf.ReturnToOriginal();
                pushedLeaves.Remove(leaf);
            }
        }
    }

    void UpdateUIWorldPositionAndSize()
    {
        // 获取UI的世界坐标
        if (useSimpleConversion)
        {
            worldPosition = UIToWorldPositionSimple(uiTarget);
        }
        else if (useAlternativeConversion && targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            worldPosition = UIToWorldPosition(uiTarget);
        }
        else
        {
            worldPosition = UIToWorldPosition(uiTarget);
        }

        // 计算UI在世界坐标中的实际尺寸
        uiSize = CalculateUIWorldSize();

        // 创建清除区域矩形（添加内边距）
        Vector2 clearSize = uiSize + Vector2.one * clearPadding * 2f;
        clearRect = new Rect(
            worldPosition.x - clearSize.x / 2f,
            worldPosition.y - clearSize.y / 2f,
            clearSize.x,
            clearSize.y
        );

        // 创建稳定性检测区域（更大的矩形，用于防止频繁切换）
        Vector2 stabilitySize = clearSize + Vector2.one * stabilityBuffer * 2f;
        stabilityRect = new Rect(
            worldPosition.x - stabilitySize.x / 2f,
            worldPosition.y - stabilitySize.y / 2f,
            stabilitySize.x,
            stabilitySize.y
        );
    }

    Vector2 CalculateUIWorldSize()
    {
        if (uiTarget == null || targetCanvas == null) return Vector2.zero;

        Vector2 rectSize = uiTarget.rect.size;

        if (targetCanvas.renderMode == RenderMode.WorldSpace)
        {
            // WorldSpace模式下，直接使用lossyScale
            Vector3 scale = uiTarget.lossyScale;
            return new Vector2(rectSize.x * scale.x, rectSize.y * scale.y);
        }
        else
        {
            // ScreenSpace模式下，需要通过屏幕坐标转换来计算世界尺寸
            Vector3[] corners = new Vector3[4];
            uiTarget.GetWorldCorners(corners);

            // 计算UI矩形在屏幕上的尺寸
            Vector2 screenSize = new Vector2(
                Mathf.Abs(corners[2].x - corners[0].x),
                Mathf.Abs(corners[2].y - corners[0].y)
            );

            // 将屏幕尺寸转换为世界尺寸
            Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 10f));
            Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(screenSize.x, screenSize.y, 10f));

            return new Vector2(
                Mathf.Abs(screenTopRight.x - screenBottomLeft.x),
                Mathf.Abs(screenTopRight.y - screenBottomLeft.y)
            );
        }
    }

    bool IsPointInRect(Vector3 point, Rect rect)
    {
        return point.x >= rect.xMin && point.x <= rect.xMax &&
               point.y >= rect.yMin && point.y <= rect.yMax;
    }

    Vector3 CalculateRectangularPushTarget(Vector3 leafPos, Rect clearRect)
    {
        Vector3 rectCenter = new Vector3(clearRect.center.x, clearRect.center.y, 0f);
        Vector3 direction = (leafPos - rectCenter).normalized;

        // 计算叶子相对于矩形中心的方向
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        Vector3 pushTarget;

        if (absX > absY)
        {
            // 主要在水平方向，推到左边或右边
            float targetX = direction.x > 0 ?
                clearRect.xMax + pushDistance :
                clearRect.xMin - pushDistance;
            pushTarget = new Vector3(targetX, leafPos.y, 0f);
        }
        else
        {
            // 主要在垂直方向，推到上边或下边
            float targetY = direction.y > 0 ?
                clearRect.yMax + pushDistance :
                clearRect.yMin - pushDistance;
            pushTarget = new Vector3(leafPos.x, targetY, 0f);
        }

        return pushTarget;
    }

    /// <summary>
    /// 将UI坐标转换为世界坐标 - 修复版本
    /// </summary>
    Vector3 UIToWorldPosition(RectTransform rectTransform)
    {
        if (targetCanvas == null || mainCamera == null)
        {
            Debug.LogError("Canvas或主相机未设置!");
            return Vector3.zero;
        }

        Vector3 worldPos = Vector3.zero;

        if (targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Screen Space - Overlay模式
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector3 centerScreen = (corners[0] + corners[2]) / 2f;

            Vector3 screenPoint = new Vector3(centerScreen.x, centerScreen.y, 20f);
            worldPos = mainCamera.ScreenToWorldPoint(screenPoint);

            if (enableDebugLog)
            {
                Debug.Log($"Overlay模式 - UI屏幕角点: {corners[0]}, 屏幕中心: {centerScreen}, 世界坐标: {worldPos}");
            }
        }
        else if (targetCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Camera canvasCamera = targetCanvas.worldCamera;
            if (canvasCamera == null)
            {
                Debug.LogError("Canvas的World Camera未设置!");
                return Vector3.zero;
            }

            Vector3 uiWorldPos = rectTransform.position;
            Vector3 screenPos = canvasCamera.WorldToScreenPoint(uiWorldPos);
            Vector3 screenPoint = new Vector3(screenPos.x, screenPos.y, 10f);
            worldPos = mainCamera.ScreenToWorldPoint(screenPoint);

            if (enableDebugLog)
            {
                Debug.Log($"Camera模式 - UI世界位置: {uiWorldPos}, 屏幕坐标: {screenPos}, 最终世界坐标: {worldPos}");
            }
        }
        else if (targetCanvas.renderMode == RenderMode.WorldSpace)
        {
            worldPos = rectTransform.position;

            if (enableDebugLog)
            {
                Debug.Log($"WorldSpace模式 - 直接世界坐标: {worldPos}");
            }
        }

        worldPos.z = 0f;
        return worldPos;
    }

    /// <summary>
    /// 简化版本的UI坐标转换
    /// </summary>
    Vector3 UIToWorldPositionSimple(RectTransform rectTransform)
    {
        Vector3 worldPos = Vector3.zero;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
            targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera,
            rectTransform.position
        );

        Vector3 screenPoint = new Vector3(screenPos.x, screenPos.y, 10f);
        worldPos = mainCamera.ScreenToWorldPoint(screenPoint);
        worldPos.z = 0f;

        if (enableDebugLog)
        {
            Debug.Log($"Simple转换 - UI位置: {rectTransform.position}, 屏幕坐标: {screenPos}, 世界坐标: {worldPos}");
        }

        return worldPos;
    }

    /// <summary>
    /// 手动刷新叶子列表
    /// </summary>
    [ContextMenu("刷新叶子列表")]
    public void RefreshLeaves()
    {
        FindAllLeaves();
        pushedLeaves.Clear();
        leafTargetPositions.Clear();
    }

    /// <summary>
    /// 设置新的UI目标
    /// </summary>
    public void SetUITarget(RectTransform newTarget)
    {
        uiTarget = newTarget;
        if (targetCanvas == null && uiTarget != null)
        {
            targetCanvas = uiTarget.GetComponentInParent<Canvas>();
        }
    }

    void OnDrawGizmos()
    {
        if (uiTarget != null && Application.isPlaying)
        {
            // 绘制世界坐标位置
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(worldPosition, 0.2f);

            // 绘制稳定性检测区域
            Gizmos.color = Color.cyan;
            if (Application.isPlaying)
            {
                Vector3 stabilityCenter = new Vector3(stabilityRect.center.x, stabilityRect.center.y, 0f);
                Vector3 stabilitySize = new Vector3(stabilityRect.width, stabilityRect.height, 0.1f);
                Gizmos.DrawWireCube(stabilityCenter, stabilitySize);
            }

            // 绘制清除矩形区域
            Gizmos.color = Color.yellow;
            Vector3 rectCenter = new Vector3(clearRect.center.x, clearRect.center.y, 0f);
            Vector3 rectSize = new Vector3(clearRect.width, clearRect.height, 0.1f);
            Gizmos.DrawWireCube(rectCenter, rectSize);

            // 绘制推开区域边界
            Gizmos.color = Color.green;
            Vector3 pushRectSize = new Vector3(clearRect.width + pushDistance * 2f, clearRect.height + pushDistance * 2f, 0.1f);
            Gizmos.DrawWireCube(rectCenter, pushRectSize);

            // 绘制UI原始尺寸区域
            Gizmos.color = Color.blue;
            Vector3 originalSize = new Vector3(uiSize.x, uiSize.y, 0.1f);
            Gizmos.DrawWireCube(worldPosition, originalSize);
        }
    }

    void OnValidate()
    {
        // 确保推开距离大于0
        if (pushDistance <= 0)
        {
            pushDistance = 1f;
        }

        // 确保稳定性缓冲区大小合理
        if (stabilityBuffer < 0)
        {
            stabilityBuffer = 0;
        }
    }
}