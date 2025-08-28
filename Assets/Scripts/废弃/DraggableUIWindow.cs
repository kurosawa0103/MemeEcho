using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using System;
public enum RotationDetectMode
{
    [LabelText ("顺时针")]
    Clockwise,       // 顺时针
    [LabelText("逆时针")]
    CounterClockwise // 逆时针
}
public class DraggableUIWindow : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    [Header("自动跟随鼠标设置")]
    [ToggleLeft]
    public bool followMouse = false;

    [ShowIf("followMouse")]
    public Vector2 mouseOffset = Vector2.zero;
    [Header("拖拽设置")]
    public bool moveToTopOnDrag = true;
    public bool canDrag = true;
    [Header("晃动检测设置")]
    [ToggleLeft]
    public bool enableShakeDetection = false;

    [ShowIf("enableShakeDetection"),ReadOnly]
    public float shakeThreshold = 30f;

    [ShowIf("enableShakeDetection"),ReadOnly]
    public int shakeRequiredCount = 3;

    [ReadOnly, ShowIf("enableShakeDetection"), ShowInInspector]
    private int horizontalShakeCount = 0;

    [ReadOnly, ShowIf("enableShakeDetection"), ShowInInspector]
    private int verticalShakeCount = 0;

    private int lastHorizontalDirection = 0;
    private int lastVerticalDirection = 0;
    private Vector2 lastDragPosition;

    public static Action<bool> OnDraggingChanged;



    [Header("旋转检测设置")]
    [ToggleLeft]
    public bool enableRotationDetection;
    [ShowIf("enableRotationDetection"),LabelText("检测方向")]
    public RotationDetectMode rotationDetectMode = RotationDetectMode.Clockwise;
    private Vector2 previousDirection = Vector2.zero;
    [ShowIf("enableRotationDetection"),ReadOnly]
    public float cumulativeRotationAngle;
    private int rotationSign = 0; // 1: 顺时针, -1: 逆时针, 0: 未定
    private const float directionThreshold = 35f; // 小角度容忍阈值，防止误判
    public Action OnRotationDetected;  // 达成一圈触发


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null)
        {
            Debug.LogError("No Canvas found in parent hierarchy.");
        }
    }
    private void Update()
    {
        if (followMouse && canvas != null)
        {
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out localMousePosition
            );

            rectTransform.anchoredPosition = localMousePosition + mouseOffset;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canDrag) return; // 禁用拖拽
        originalPosition = rectTransform.anchoredPosition;

        if (moveToTopOnDrag)
        {
            rectTransform.SetAsLastSibling();
        }

        //  不再清零晃动计数
        lastHorizontalDirection = 0;
        lastVerticalDirection = 0;
        lastDragPosition = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag || eventData.button != PointerEventData.InputButton.Left) return; // ← 只响应左键
        canvasGroup.alpha = 1f;
        //开始拖拽的时候还是继续不可穿透
        canvasGroup.blocksRaycasts = true;
        OnDraggingChanged?.Invoke(true);

        if (enableRotationDetection)
        {
            cumulativeRotationAngle = 0f;
            lastDragPosition = eventData.position;
            previousDirection = Vector2.zero; // 初始为空
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag || eventData.button != PointerEventData.InputButton.Left) return; // ← 只响应左键
        Vector2 delta = eventData.delta / canvas.scaleFactor;
        rectTransform.anchoredPosition += delta;

        if (!enableShakeDetection) return;

        Vector2 dragDelta = eventData.position - lastDragPosition;

        float absX = Mathf.Abs(dragDelta.x);
        float absY = Mathf.Abs(dragDelta.y);

        // 判断是否需要切换到水平方向
        if (absX > shakeThreshold && absX > absY * 1.5f)
        {
            // 如果从垂直切换到水平，清零垂直晃动次数
            if (lastVerticalDirection != 0)
            {
                verticalShakeCount = 0;
                //Debug.Log("垂直方向次数清零");
            }

            int direction = dragDelta.x > 0 ? 1 : -1;
            if (direction != lastHorizontalDirection && lastHorizontalDirection != 0)
            {
                horizontalShakeCount++;
                //Debug.Log($"水平晃动 {horizontalShakeCount} 次");
            }
            lastHorizontalDirection = direction;
            lastDragPosition = eventData.position;
        }
        // 判断是否需要切换到垂直方向
        else if (absY > shakeThreshold && absY > absX * 1.5f)
        {
            // 如果从水平切换到垂直，清零水平晃动次数
            if (lastHorizontalDirection != 0)
            {
                horizontalShakeCount = 0;
                //Debug.Log("水平方向次数清零");
            }

            int direction = dragDelta.y > 0 ? 1 : -1;
            if (direction != lastVerticalDirection && lastVerticalDirection != 0)
            {
                verticalShakeCount++;
                //Debug.Log($"垂直晃动 {verticalShakeCount} 次");
            }
            lastVerticalDirection = direction;
            lastDragPosition = eventData.position;
        }
        // 如果拖动方向不明确，忽略当前帧

        if (!enableRotationDetection) return;

        Vector2 currentDirection = dragDelta.normalized;

        if (previousDirection != Vector2.zero && currentDirection != Vector2.zero)
        {
            float angle = Vector2.SignedAngle(previousDirection, currentDirection);

            if (Mathf.Abs(angle) < directionThreshold)
            {
                // 忽略过小角度变化
                return;
            }

            int currentSign = angle < 0 ? 1 : -1;

            bool isCorrectDirection =
                (rotationDetectMode == RotationDetectMode.Clockwise && currentSign == 1) ||
                (rotationDetectMode == RotationDetectMode.CounterClockwise && currentSign == -1);

            if (rotationSign == 0)
            {
                rotationSign = currentSign;
            }

            if (currentSign == rotationSign && isCorrectDirection)
            {
                cumulativeRotationAngle += Mathf.Abs(angle);

                if (cumulativeRotationAngle >= 360f)
                {
                    string dirStr = rotationDetectMode == RotationDetectMode.Clockwise ? "顺时针" : "逆时针";
                    Debug.Log($"转圈成功（{dirStr}）");
                    OnRotationDetected?.Invoke();
                    cumulativeRotationAngle = 0f;
                    rotationSign = 0;
                }
            }
            else
            {
                // 方向变了就清零
                cumulativeRotationAngle = 0f;
                rotationSign = 0;
            }
        }

        previousDirection = currentDirection;
        lastDragPosition = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag || eventData.button != PointerEventData.InputButton.Left) return; // ← 只响应左键
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        OnDraggingChanged?.Invoke(false);

        Vector3[] canvasCorners = new Vector3[4];
        canvas.GetComponent<RectTransform>().GetWorldCorners(canvasCorners);

        Vector3[] windowCorners = new Vector3[4];
        rectTransform.GetWorldCorners(windowCorners);

        Vector2 offset = Vector2.zero;
        if (windowCorners[0].x < canvasCorners[0].x) offset.x = canvasCorners[0].x - windowCorners[0].x;
        if (windowCorners[2].x > canvasCorners[2].x) offset.x = canvasCorners[2].x - windowCorners[2].x;
        if (windowCorners[0].y < canvasCorners[0].y) offset.y = canvasCorners[0].y - windowCorners[0].y;
        if (windowCorners[2].y > canvasCorners[2].y) offset.y = canvasCorners[2].y - windowCorners[2].y;

        rectTransform.anchoredPosition += offset / canvas.scaleFactor;

        ResetShakeCount();

        if (enableRotationDetection)
        {
            cumulativeRotationAngle = 0f;
            previousDirection = Vector2.zero;
        }
    }

    public int GetCurrentShakeCount(bool vertical)
    {
        return vertical ? verticalShakeCount : horizontalShakeCount;
    }

    public void ResetShakeCount()
    {
        horizontalShakeCount = 0;
        verticalShakeCount = 0;
    }
}
