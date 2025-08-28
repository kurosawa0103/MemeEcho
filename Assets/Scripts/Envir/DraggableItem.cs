using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Sirenix.OdinInspector;

[RequireComponent(typeof(RectTransform))]
public class DraggableItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("����")]
    public bool canDrag = true;
    public bool moveToTopOnDrag = true;
    public float minClickDuration = 0.2f;
    public float maxClickMoveDistance = 10f;

    [Header("�Զ��ر�����")]
    public float autoCloseTime = 5f;

    [Header("�������")]
    public GameObject dialogBox;
    public GameObject useButton;

    [Header("�¼�")]
    public Action OnClick;
    public Action OnDragStart;
    public Action OnDragEnd;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private UIRigidbody2D uiRigidbody;

    private float pointerDownTime;
    private Vector2 pointerDownPosition;
    private bool isDragging = false;

    private bool isDialogVisible = false;
    private float dialogTimer = 0f;

    private RandomMoveInUI randomMover;

    public Item thisItem;

    [Header("�������")]
    public bool useArcTrack = false;
    [ShowIf("useArcTrack", true)]
    public float arcRadiusX = 100f; // ˮƽ����
    [ShowIf("useArcTrack", true)]
    public float arcRadiusY = 100f; // ��ֱ����
    [ShowIf("useArcTrack", true)]
    public float arcStartAngle = 0f;
    [ShowIf("useArcTrack", true)]
    public float arcEndAngle = 180f;
    [ShowIf("useArcTrack", true)]
    public RectTransform arcCenter;
    [ShowIf("useArcTrack", true), ReadOnly]
    public float anglePercent = 0f;

    private void Awake()
    {
        uiRigidbody = GetComponent<UIRigidbody2D>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        randomMover = GetComponent<RandomMoveInUI>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (useArcTrack && arcCenter == null)
        {
            GameObject rootObj = GameObject.FindWithTag("Root");
            if (rootObj != null)
            {
                arcCenter = rootObj.GetComponent<RectTransform>();
            }
            else
            {
                Debug.LogWarning("δ�ҵ� tag Ϊ Root �Ķ���");
            }
        }
    }

    private void Update()
    {
        // �������ǿ����ģ��ҿ����˶�ʱ����
        if (isDialogVisible)
        {
            dialogTimer += Time.deltaTime;
            if (dialogTimer >= autoCloseTime)
            {
                CloseDialogUI();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canDrag) return;

        pointerDownTime = Time.time;
        pointerDownPosition = eventData.position;

        if (moveToTopOnDrag)
        {
            rectTransform.SetAsLastSibling();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        isDragging = true;
        canvasGroup.blocksRaycasts = false;

        if (randomMover != null)
            randomMover.movementEnabled = false;

        if (uiRigidbody != null)
            uiRigidbody.enabled = false; // ��ͣ����

        OnDragStart?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        if (useArcTrack && arcCenter != null)
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                arcCenter, eventData.position, eventData.pressEventCamera, out localMousePos
            );

            float angle = Mathf.Atan2(localMousePos.y, localMousePos.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f; // ת��Ϊ 0~360��

            // ���ƽǶȱ����� arcStartAngle ~ arcEndAngle ��
            if (angle >= arcStartAngle && angle <= arcEndAngle)
            {
                float clampedAngle = Mathf.Clamp(angle, arcStartAngle, arcEndAngle);
                anglePercent = (clampedAngle - arcStartAngle) / (arcEndAngle - arcStartAngle);

                float rad = clampedAngle * Mathf.Deg2Rad;

                Vector2 finalPos = new Vector2(
                    Mathf.Cos(rad) * arcRadiusX,
                    Mathf.Sin(rad) * arcRadiusY
                );

                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, finalPos, 0.35f);

            }
            else
            {
                // �����Ƕȷ�Χʱ����ѡ����հٷֱȻ򱣳ֲ��䣬������Ϊ0
                //anglePercent = 0f;
            }
        }
        else
        {
            // �켣�ǻ���ʱ�ٷֱ�û����
            anglePercent = 0f;

            Vector2 delta = eventData.delta / canvas.scaleFactor;
            rectTransform.anchoredPosition += delta;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        if (randomMover != null)
            randomMover.movementEnabled = true;

        if (uiRigidbody != null)
        {
            Vector2 dragVelocity = eventData.delta / Time.deltaTime / canvas.scaleFactor;
            uiRigidbody.velocity = dragVelocity * uiRigidbody.inertiaMultiplier;
            uiRigidbody.enabled = true;
        }

        OnDragEnd?.Invoke();
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (!canDrag) return;

        float heldTime = Time.time - pointerDownTime;
        float movedDistance = Vector2.Distance(eventData.position, pointerDownPosition);

        if (heldTime <= minClickDuration && movedDistance <= maxClickMoveDistance && !isDragging)
        {
            // �л������ʾ
            if (dialogBox != null && useButton != null)
            {
                isDialogVisible = !isDialogVisible;
                dialogBox.SetActive(isDialogVisible);
                useButton.SetActive(isDialogVisible);

                if (isDialogVisible)
                {
                    dialogTimer = 0f; // ��ʼ��ʱ
                    if (randomMover != null)
                        randomMover.movementEnabled = false; //  ֹͣ�ƶ�
                }
                else
                {
                    if (randomMover != null)
                    {
                        randomMover.movementEnabled = true; //  �ر�ʱ�ָ��ƶ�
                    }

                        
                }
            }

            OnClick?.Invoke();
        }

        isDragging = false;
    }

    private void CloseDialogUI()
    {
        dialogBox.SetActive(false);
        useButton.SetActive(false);
        isDialogVisible = false;
        dialogTimer = 0f;
        if (randomMover != null)
        {
            randomMover.movementEnabled = true; // �Ի���ʱ�ر�ʱ�ָ��ƶ�
        }
            
    }
}
