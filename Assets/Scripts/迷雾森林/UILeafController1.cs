using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UILeafController1 : MonoBehaviour
{
    [Header("UI设置")]
    public RectTransform uiTarget; // 目标UI物体
    public Canvas targetCanvas; // UI所在的Canvas

    [Header("渐隐设置")]
    public float clearPadding = 0.5f; // 清除区域内边距
    public float fadeBuffer = 1f; // 渐隐缓冲区，用于平滑过渡
    public float fadeSpeed = 3f; // 渐隐速度
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
    private Rect fadeRect; // 渐隐区域的矩形（更大的范围）
    private List<Leaf> allLeaves = new List<Leaf>();
    private Dictionary<Leaf, LeafFadeData> leafFadeStates = new Dictionary<Leaf, LeafFadeData>(); // 记录每个叶子的渐隐状态

    [System.Serializable]
    public class LeafFadeData
    {
        public SpriteRenderer spriteRenderer;
        public Color originalColor;
        public float targetAlpha;
        public float currentAlpha;
        public bool isInFadeZone;

        public LeafFadeData(SpriteRenderer sr)
        {
            spriteRenderer = sr;
            originalColor = sr.color;
            targetAlpha = 1f;
            currentAlpha = 1f;
            isInFadeZone = false;
        }
    }

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
        InvokeRepeating(nameof(UpdateLeafFade), 0f, updateInterval);

        // 开始渐隐协程
        StartCoroutine(FadeUpdateCoroutine());
    }

    void FindAllLeaves()
    {
        allLeaves.Clear();
        leafFadeStates.Clear();

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

                    // 初始化渐隐数据
                    SpriteRenderer sr = leaf.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        leafFadeStates[leaf] = new LeafFadeData(sr);
                    }
                }
            }
        }

        Debug.Log($"找到 {allLeaves.Count} 片叶子");
    }

    void UpdateLeafFade()
    {
        if (uiTarget == null) return;

        // 获取UI的世界坐标和尺寸
        UpdateUIWorldPositionAndSize();

        if (enableDebugLog)
        {
            Debug.Log($"=== 渐隐更新调试 ===");
            Debug.Log($"UI RectTransform.position: {uiTarget.position}");
            Debug.Log($"UI anchoredPosition: {uiTarget.anchoredPosition}");
            Debug.Log($"UI Rect Size: {uiTarget.rect.size}");
            Debug.Log($"UI World Size: {uiSize}");
            Debug.Log($"Canvas RenderMode: {targetCanvas.renderMode}");
            Debug.Log($"转换后世界坐标: {worldPosition}");
            Debug.Log($"清除矩形区域: {clearRect}");
            Debug.Log($"渐隐矩形区域: {fadeRect}");
        }

        // 检查每片叶子的渐隐状态
        foreach (var leaf in allLeaves)
        {
            if (leaf == null || !leafFadeStates.ContainsKey(leaf)) continue;

            Vector3 leafOriginalPos = leaf.GetOriginalPosition();
            LeafFadeData fadeData = leafFadeStates[leaf];

            bool isInClearRect = IsPointInRect(leafOriginalPos, clearRect);
            bool isInFadeRect = IsPointInRect(leafOriginalPos, fadeRect);

            if (isInClearRect)
            {
                // 叶子在清除区域内，完全透明
                fadeData.targetAlpha = 0f;
                fadeData.isInFadeZone = true;

                if (enableDebugLog)
                {
                    Debug.Log($"叶子 {leaf.name} 在清除区域内，目标透明度: 0");
                }
            }
            else if (isInFadeRect)
            {
                // 叶子在渐隐区域内，计算距离渐隐
                float fadeAlpha = CalculateFadeAlpha(leafOriginalPos, clearRect, fadeRect);
                fadeData.targetAlpha = fadeAlpha;
                fadeData.isInFadeZone = true;

                if (enableDebugLog)
                {
                    Debug.Log($"叶子 {leaf.name} 在渐隐区域内，目标透明度: {fadeAlpha:F2}");
                }
            }
            else
            {
                // 叶子在渐隐区域外，完全不透明
                fadeData.targetAlpha = 1f;
                fadeData.isInFadeZone = false;

                if (enableDebugLog)
                {
                    Debug.Log($"叶子 {leaf.name} 在区域外，目标透明度: 1");
                }
            }
        }
    }

    float CalculateFadeAlpha(Vector3 leafPos, Rect clearRect, Rect fadeRect)
    {
        // 计算叶子到清除区域边界的最短距离
        float distToClear = CalculateDistanceToRect(leafPos, clearRect);
        float maxFadeDistance = fadeBuffer;

        // 将距离映射到透明度 (0到fadeBuffer距离映射到0-1透明度)
        float alpha = Mathf.Clamp01(distToClear / maxFadeDistance);

        return alpha;
    }

    float CalculateDistanceToRect(Vector3 point, Rect rect)
    {
        // 计算点到矩形的最短距离
        float dx = Mathf.Max(0, Mathf.Max(rect.xMin - point.x, point.x - rect.xMax));
        float dy = Mathf.Max(0, Mathf.Max(rect.yMin - point.y, point.y - rect.yMax));
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    IEnumerator FadeUpdateCoroutine()
    {
        while (true)
        {
            // 更新所有叶子的透明度
            foreach (var kvp in leafFadeStates)
            {
                LeafFadeData fadeData = kvp.Value;
                if (fadeData.spriteRenderer == null) continue;

                // 平滑插值到目标透明度
                fadeData.currentAlpha = Mathf.Lerp(
                    fadeData.currentAlpha,
                    fadeData.targetAlpha,
                    fadeSpeed * Time.deltaTime
                );

                // 应用透明度
                Color currentColor = fadeData.originalColor;
                currentColor.a = fadeData.currentAlpha;
                fadeData.spriteRenderer.color = currentColor;
            }

            yield return null; // 等待下一帧
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

        // 创建渐隐区域矩形（更大的矩形，用于渐隐过渡）
        Vector2 fadeSize = clearSize + Vector2.one * fadeBuffer * 2f;
        fadeRect = new Rect(
            worldPosition.x - fadeSize.x / 2f,
            worldPosition.y - fadeSize.y / 2f,
            fadeSize.x,
            fadeSize.y
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
    }

    /// <summary>
    /// 重置所有叶子透明度
    /// </summary>
    [ContextMenu("重置叶子透明度")]
    public void ResetLeafAlpha()
    {
        foreach (var kvp in leafFadeStates)
        {
            LeafFadeData fadeData = kvp.Value;
            if (fadeData.spriteRenderer != null)
            {
                fadeData.spriteRenderer.color = fadeData.originalColor;
                fadeData.currentAlpha = 1f;
                fadeData.targetAlpha = 1f;
            }
        }
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

            // 绘制渐隐区域
            Gizmos.color = Color.cyan;
            Vector3 fadeCenter = new Vector3(fadeRect.center.x, fadeRect.center.y, 0f);
            Vector3 fadeSize = new Vector3(fadeRect.width, fadeRect.height, 0.1f);
            Gizmos.DrawWireCube(fadeCenter, fadeSize);

            // 绘制清除矩形区域
            Gizmos.color = Color.yellow;
            Vector3 rectCenter = new Vector3(clearRect.center.x, clearRect.center.y, 0f);
            Vector3 rectSize = new Vector3(clearRect.width, clearRect.height, 0.1f);
            Gizmos.DrawWireCube(rectCenter, rectSize);

            // 绘制UI原始尺寸区域
            Gizmos.color = Color.blue;
            Vector3 originalSize = new Vector3(uiSize.x, uiSize.y, 0.1f);
            Gizmos.DrawWireCube(worldPosition, originalSize);
        }
    }

    void OnValidate()
    {
        // 确保渐隐速度大于0
        if (fadeSpeed <= 0)
        {
            fadeSpeed = 1f;
        }

        // 确保渐隐缓冲区大小合理
        if (fadeBuffer < 0)
        {
            fadeBuffer = 0;
        }

        // 确保清除内边距不为负数
        if (clearPadding < 0)
        {
            clearPadding = 0;
        }
    }

    void OnDestroy()
    {
        // 清理协程
        StopAllCoroutines();
    }
}