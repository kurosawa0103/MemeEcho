using UnityEngine;
using System.Collections.Generic;

public class UILeafController : MonoBehaviour
{
    [Header("UI����")]
    public RectTransform uiTarget; // Ŀ��UI����
    public Canvas targetCanvas; // UI���ڵ�Canvas

    [Header("�ƿ�����")]
    public float clearPadding = 0.5f; // ��������ڱ߾�
    public float pushDistance = 3f; // �ƿ�����
    public float stabilityBuffer = 1f; // �ȶ��Ի���������ֹƵ���л�
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
    private Rect stabilityRect; // �ȶ��Լ�����򣨸���ľ��Σ�
    private List<Leaf> allLeaves = new List<Leaf>();
    private HashSet<Leaf> pushedLeaves = new HashSet<Leaf>();
    private Dictionary<Leaf, Vector3> leafTargetPositions = new Dictionary<Leaf, Vector3>(); // ��¼ÿ��Ҷ�ӵ�Ŀ��λ��

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
        InvokeRepeating(nameof(UpdateLeafPositions), 0f, updateInterval);
    }

    void FindAllLeaves()
    {
        allLeaves.Clear();

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
                }
            }
        }

        Debug.Log($"�ҵ� {allLeaves.Count} ƬҶ��");
    }

    void UpdateLeafPositions()
    {
        if (uiTarget == null) return;

        // ��ȡUI����������ͳߴ�
        UpdateUIWorldPositionAndSize();

        if (enableDebugLog)
        {
            Debug.Log($"=== ����ת������ ===");
            Debug.Log($"UI RectTransform.position: {uiTarget.position}");
            Debug.Log($"UI anchoredPosition: {uiTarget.anchoredPosition}");
            Debug.Log($"UI Rect Size: {uiTarget.rect.size}");
            Debug.Log($"UI World Size: {uiSize}");
            Debug.Log($"Canvas RenderMode: {targetCanvas.renderMode}");
            Debug.Log($"ת������������: {worldPosition}");
            Debug.Log($"�����������: {clearRect}");
        }

        // ���ÿƬҶ��
        List<Leaf> leavesToReturn = new List<Leaf>(pushedLeaves);

        foreach (var leaf in allLeaves)
        {
            if (leaf == null) continue;

            Vector3 leafOriginalPos = leaf.GetOriginalPosition();

            // ���Ҷ���Ƿ��ھ������������
            if (IsPointInRect(leafOriginalPos, clearRect))
            {
                // Ҷ������������ڣ���Ҫ�ƿ�
                Vector3 pushTarget = CalculateRectangularPushTarget(leafOriginalPos, clearRect);

                leaf.PushTo(pushTarget);
                pushedLeaves.Add(leaf);
                leavesToReturn.Remove(leaf);

                if (enableDebugLog)
                {
                    Debug.Log($"�ƿ�Ҷ�� {leaf.name}: �� {leafOriginalPos} �Ƶ� {pushTarget}");
                }
            }
        }

        // �ò�����Ҫ�ƿ���Ҷ�ӻص�ԭλ
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

        // �����ȶ��Լ�����򣨸���ľ��Σ����ڷ�ֹƵ���л���
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

    Vector3 CalculateRectangularPushTarget(Vector3 leafPos, Rect clearRect)
    {
        Vector3 rectCenter = new Vector3(clearRect.center.x, clearRect.center.y, 0f);
        Vector3 direction = (leafPos - rectCenter).normalized;

        // ����Ҷ������ھ������ĵķ���
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        Vector3 pushTarget;

        if (absX > absY)
        {
            // ��Ҫ��ˮƽ�����Ƶ���߻��ұ�
            float targetX = direction.x > 0 ?
                clearRect.xMax + pushDistance :
                clearRect.xMin - pushDistance;
            pushTarget = new Vector3(targetX, leafPos.y, 0f);
        }
        else
        {
            // ��Ҫ�ڴ�ֱ�����Ƶ��ϱ߻��±�
            float targetY = direction.y > 0 ?
                clearRect.yMax + pushDistance :
                clearRect.yMin - pushDistance;
            pushTarget = new Vector3(leafPos.x, targetY, 0f);
        }

        return pushTarget;
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
        pushedLeaves.Clear();
        leafTargetPositions.Clear();
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

            // �����ȶ��Լ������
            Gizmos.color = Color.cyan;
            if (Application.isPlaying)
            {
                Vector3 stabilityCenter = new Vector3(stabilityRect.center.x, stabilityRect.center.y, 0f);
                Vector3 stabilitySize = new Vector3(stabilityRect.width, stabilityRect.height, 0.1f);
                Gizmos.DrawWireCube(stabilityCenter, stabilitySize);
            }

            // ���������������
            Gizmos.color = Color.yellow;
            Vector3 rectCenter = new Vector3(clearRect.center.x, clearRect.center.y, 0f);
            Vector3 rectSize = new Vector3(clearRect.width, clearRect.height, 0.1f);
            Gizmos.DrawWireCube(rectCenter, rectSize);

            // �����ƿ�����߽�
            Gizmos.color = Color.green;
            Vector3 pushRectSize = new Vector3(clearRect.width + pushDistance * 2f, clearRect.height + pushDistance * 2f, 0.1f);
            Gizmos.DrawWireCube(rectCenter, pushRectSize);

            // ����UIԭʼ�ߴ�����
            Gizmos.color = Color.blue;
            Vector3 originalSize = new Vector3(uiSize.x, uiSize.y, 0.1f);
            Gizmos.DrawWireCube(worldPosition, originalSize);
        }
    }

    void OnValidate()
    {
        // ȷ���ƿ��������0
        if (pushDistance <= 0)
        {
            pushDistance = 1f;
        }

        // ȷ���ȶ��Ի�������С����
        if (stabilityBuffer < 0)
        {
            stabilityBuffer = 0;
        }
    }
}