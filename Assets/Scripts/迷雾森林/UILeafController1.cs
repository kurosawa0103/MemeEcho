using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UILeafController1 : MonoBehaviour
{
    [Header("UI����")]
    public RectTransform uiTarget; // Ŀ��UI����
    public Canvas targetCanvas; // UI���ڵ�Canvas

    [Header("��������")]
    public float clearPadding = 0.5f; // ��������ڱ߾�
    public float fadeBuffer = 1f; // ����������������ƽ������
    public float fadeSpeed = 3f; // �����ٶ�
    public LayerMask leafLayer = 1 << 8; // Ҷ��ͼ��

    [Header("��������")]
    public float updateInterval = 0.1f; // ���¼��

    [Header("��������")]
    public bool useAlternativeConversion = false; // �Ƿ�ʹ�ñ�������ת������
    public bool useSimpleConversion = true; // �Ƿ�ʹ�ü�����ת������
    public bool enableDebugLog = false; // ���õ�����־

    private Camera mainCamera;
    private Vector3 worldPosition;
    private Vector2 uiSize; // UI���ε�ʵ�ʳߴ�
    private Rect clearRect; // �������ľ���
    private Rect fadeRect; // ��������ľ��Σ�����ķ�Χ��
    private List<Leaf> allLeaves = new List<Leaf>();
    private Dictionary<Leaf, LeafFadeData> leafFadeStates = new Dictionary<Leaf, LeafFadeData>(); // ��¼ÿ��Ҷ�ӵĽ���״̬

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

        // ���û��ָ��Canvas�����Դ�UIĿ���ȡ
        if (targetCanvas == null && uiTarget != null)
        {
            targetCanvas = uiTarget.GetComponentInParent<Canvas>();
        }

        // �ҵ�����Ҷ��
        FindAllLeaves();

        // ��ʼ���ڸ���
        InvokeRepeating(nameof(UpdateLeafFade), 0f, updateInterval);

        // ��ʼ����Э��
        StartCoroutine(FadeUpdateCoroutine());
    }

    void FindAllLeaves()
    {
        allLeaves.Clear();
        leafFadeStates.Clear();

        // ͨ��ͼ���������Ҷ��
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (((1 << obj.layer) & leafLayer) != 0)
            {
                Leaf leaf = obj.GetComponent<Leaf>();
                if (leaf != null)
                {
                    allLeaves.Add(leaf);

                    // ��ʼ����������
                    SpriteRenderer sr = leaf.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        leafFadeStates[leaf] = new LeafFadeData(sr);
                    }
                }
            }
        }

        Debug.Log($"�ҵ� {allLeaves.Count} ƬҶ��");
    }

    void UpdateLeafFade()
    {
        if (uiTarget == null) return;

        // ��ȡUI����������ͳߴ�
        UpdateUIWorldPositionAndSize();

        if (enableDebugLog)
        {
            Debug.Log($"=== �������µ��� ===");
            Debug.Log($"UI RectTransform.position: {uiTarget.position}");
            Debug.Log($"UI anchoredPosition: {uiTarget.anchoredPosition}");
            Debug.Log($"UI Rect Size: {uiTarget.rect.size}");
            Debug.Log($"UI World Size: {uiSize}");
            Debug.Log($"Canvas RenderMode: {targetCanvas.renderMode}");
            Debug.Log($"ת������������: {worldPosition}");
            Debug.Log($"�����������: {clearRect}");
            Debug.Log($"������������: {fadeRect}");
        }

        // ���ÿƬҶ�ӵĽ���״̬
        foreach (var leaf in allLeaves)
        {
            if (leaf == null || !leafFadeStates.ContainsKey(leaf)) continue;

            Vector3 leafOriginalPos = leaf.GetOriginalPosition();
            LeafFadeData fadeData = leafFadeStates[leaf];

            bool isInClearRect = IsPointInRect(leafOriginalPos, clearRect);
            bool isInFadeRect = IsPointInRect(leafOriginalPos, fadeRect);

            if (isInClearRect)
            {
                // Ҷ������������ڣ���ȫ͸��
                fadeData.targetAlpha = 0f;
                fadeData.isInFadeZone = true;

                if (enableDebugLog)
                {
                    Debug.Log($"Ҷ�� {leaf.name} ����������ڣ�Ŀ��͸����: 0");
                }
            }
            else if (isInFadeRect)
            {
                // Ҷ���ڽ��������ڣ�������뽥��
                float fadeAlpha = CalculateFadeAlpha(leafOriginalPos, clearRect, fadeRect);
                fadeData.targetAlpha = fadeAlpha;
                fadeData.isInFadeZone = true;

                if (enableDebugLog)
                {
                    Debug.Log($"Ҷ�� {leaf.name} �ڽ��������ڣ�Ŀ��͸����: {fadeAlpha:F2}");
                }
            }
            else
            {
                // Ҷ���ڽ��������⣬��ȫ��͸��
                fadeData.targetAlpha = 1f;
                fadeData.isInFadeZone = false;

                if (enableDebugLog)
                {
                    Debug.Log($"Ҷ�� {leaf.name} �������⣬Ŀ��͸����: 1");
                }
            }
        }
    }

    float CalculateFadeAlpha(Vector3 leafPos, Rect clearRect, Rect fadeRect)
    {
        // ����Ҷ�ӵ��������߽����̾���
        float distToClear = CalculateDistanceToRect(leafPos, clearRect);
        float maxFadeDistance = fadeBuffer;

        // ������ӳ�䵽͸���� (0��fadeBuffer����ӳ�䵽0-1͸����)
        float alpha = Mathf.Clamp01(distToClear / maxFadeDistance);

        return alpha;
    }

    float CalculateDistanceToRect(Vector3 point, Rect rect)
    {
        // ����㵽���ε���̾���
        float dx = Mathf.Max(0, Mathf.Max(rect.xMin - point.x, point.x - rect.xMax));
        float dy = Mathf.Max(0, Mathf.Max(rect.yMin - point.y, point.y - rect.yMax));
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    IEnumerator FadeUpdateCoroutine()
    {
        while (true)
        {
            // ��������Ҷ�ӵ�͸����
            foreach (var kvp in leafFadeStates)
            {
                LeafFadeData fadeData = kvp.Value;
                if (fadeData.spriteRenderer == null) continue;

                // ƽ����ֵ��Ŀ��͸����
                fadeData.currentAlpha = Mathf.Lerp(
                    fadeData.currentAlpha,
                    fadeData.targetAlpha,
                    fadeSpeed * Time.deltaTime
                );

                // Ӧ��͸����
                Color currentColor = fadeData.originalColor;
                currentColor.a = fadeData.currentAlpha;
                fadeData.spriteRenderer.color = currentColor;
            }

            yield return null; // �ȴ���һ֡
        }
    }

    void UpdateUIWorldPositionAndSize()
    {
        // ��ȡUI����������
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

        // ����UI�����������е�ʵ�ʳߴ�
        uiSize = CalculateUIWorldSize();

        // �������������Σ�����ڱ߾ࣩ
        Vector2 clearSize = uiSize + Vector2.one * clearPadding * 2f;
        clearRect = new Rect(
            worldPosition.x - clearSize.x / 2f,
            worldPosition.y - clearSize.y / 2f,
            clearSize.x,
            clearSize.y
        );

        // ��������������Σ�����ľ��Σ����ڽ������ɣ�
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
            // WorldSpaceģʽ�£�ֱ��ʹ��lossyScale
            Vector3 scale = uiTarget.lossyScale;
            return new Vector2(rectSize.x * scale.x, rectSize.y * scale.y);
        }
        else
        {
            // ScreenSpaceģʽ�£���Ҫͨ����Ļ����ת������������ߴ�
            Vector3[] corners = new Vector3[4];
            uiTarget.GetWorldCorners(corners);

            // ����UI��������Ļ�ϵĳߴ�
            Vector2 screenSize = new Vector2(
                Mathf.Abs(corners[2].x - corners[0].x),
                Mathf.Abs(corners[2].y - corners[0].y)
            );

            // ����Ļ�ߴ�ת��Ϊ����ߴ�
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
    /// ��UI����ת��Ϊ�������� - �޸��汾
    /// </summary>
    Vector3 UIToWorldPosition(RectTransform rectTransform)
    {
        if (targetCanvas == null || mainCamera == null)
        {
            Debug.LogError("Canvas�������δ����!");
            return Vector3.zero;
        }

        Vector3 worldPos = Vector3.zero;

        if (targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Screen Space - Overlayģʽ
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector3 centerScreen = (corners[0] + corners[2]) / 2f;

            Vector3 screenPoint = new Vector3(centerScreen.x, centerScreen.y, 20f);
            worldPos = mainCamera.ScreenToWorldPoint(screenPoint);

            if (enableDebugLog)
            {
                Debug.Log($"Overlayģʽ - UI��Ļ�ǵ�: {corners[0]}, ��Ļ����: {centerScreen}, ��������: {worldPos}");
            }
        }
        else if (targetCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Camera canvasCamera = targetCanvas.worldCamera;
            if (canvasCamera == null)
            {
                Debug.LogError("Canvas��World Cameraδ����!");
                return Vector3.zero;
            }

            Vector3 uiWorldPos = rectTransform.position;
            Vector3 screenPos = canvasCamera.WorldToScreenPoint(uiWorldPos);
            Vector3 screenPoint = new Vector3(screenPos.x, screenPos.y, 10f);
            worldPos = mainCamera.ScreenToWorldPoint(screenPoint);

            if (enableDebugLog)
            {
                Debug.Log($"Cameraģʽ - UI����λ��: {uiWorldPos}, ��Ļ����: {screenPos}, ������������: {worldPos}");
            }
        }
        else if (targetCanvas.renderMode == RenderMode.WorldSpace)
        {
            worldPos = rectTransform.position;

            if (enableDebugLog)
            {
                Debug.Log($"WorldSpaceģʽ - ֱ����������: {worldPos}");
            }
        }

        worldPos.z = 0f;
        return worldPos;
    }

    /// <summary>
    /// �򻯰汾��UI����ת��
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
            Debug.Log($"Simpleת�� - UIλ��: {rectTransform.position}, ��Ļ����: {screenPos}, ��������: {worldPos}");
        }

        return worldPos;
    }

    /// <summary>
    /// �ֶ�ˢ��Ҷ���б�
    /// </summary>
    [ContextMenu("ˢ��Ҷ���б�")]
    public void RefreshLeaves()
    {
        FindAllLeaves();
    }

    /// <summary>
    /// ��������Ҷ��͸����
    /// </summary>
    [ContextMenu("����Ҷ��͸����")]
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
    /// �����µ�UIĿ��
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
            // ������������λ��
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(worldPosition, 0.2f);

            // ���ƽ�������
            Gizmos.color = Color.cyan;
            Vector3 fadeCenter = new Vector3(fadeRect.center.x, fadeRect.center.y, 0f);
            Vector3 fadeSize = new Vector3(fadeRect.width, fadeRect.height, 0.1f);
            Gizmos.DrawWireCube(fadeCenter, fadeSize);

            // ���������������
            Gizmos.color = Color.yellow;
            Vector3 rectCenter = new Vector3(clearRect.center.x, clearRect.center.y, 0f);
            Vector3 rectSize = new Vector3(clearRect.width, clearRect.height, 0.1f);
            Gizmos.DrawWireCube(rectCenter, rectSize);

            // ����UIԭʼ�ߴ�����
            Gizmos.color = Color.blue;
            Vector3 originalSize = new Vector3(uiSize.x, uiSize.y, 0.1f);
            Gizmos.DrawWireCube(worldPosition, originalSize);
        }
    }

    void OnValidate()
    {
        // ȷ�������ٶȴ���0
        if (fadeSpeed <= 0)
        {
            fadeSpeed = 1f;
        }

        // ȷ��������������С����
        if (fadeBuffer < 0)
        {
            fadeBuffer = 0;
        }

        // ȷ������ڱ߾಻Ϊ����
        if (clearPadding < 0)
        {
            clearPadding = 0;
        }
    }

    void OnDestroy()
    {
        // ����Э��
        StopAllCoroutines();
    }
}