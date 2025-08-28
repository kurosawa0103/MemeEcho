using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Sirenix.OdinInspector;

[RequireComponent(typeof(RectTransform))]
public class DragStar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("设置")]
    public bool canDrag = true;
    public bool moveToTopOnDrag = true;
    public float minClickDuration = 0.2f;
    public float maxClickMoveDistance = 10f;

    [Header("自动关闭配置")]
    public float autoCloseTime = 5f;

    [Header("组件引用")]
    public GameObject dialogBox;
    public GameObject useButton;

    [Header("世界跟随物体")]
    public GameObject worldFollowPrefab;

    [Header("事件")]
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

    private GameObject worldFollowInstance;

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
    }

    private void Update()
    {
        if (isDialogVisible)
        {
            dialogTimer += Time.deltaTime;
            if (dialogTimer >= autoCloseTime)
            {
                CloseDialogUI();
            }
        }

        if (worldFollowInstance != null)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = 5f; // 深度距离相机
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            worldFollowInstance.transform.position = worldPos;
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
            uiRigidbody.enabled = false;

        if (worldFollowPrefab != null)
        {
            Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            spawnPos.z = 5f;
            worldFollowInstance = Instantiate(worldFollowPrefab, spawnPos, Quaternion.identity);
        }

        OnDragStart?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        Vector2 delta = eventData.delta / canvas.scaleFactor;
        rectTransform.anchoredPosition += delta;
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

        if (worldFollowInstance != null)
        {
            Destroy(worldFollowInstance);
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
            if (dialogBox != null && useButton != null)
            {
                isDialogVisible = !isDialogVisible;
                dialogBox.SetActive(isDialogVisible);
                useButton.SetActive(isDialogVisible);

                if (isDialogVisible)
                {
                    dialogTimer = 0f;
                    if (randomMover != null)
                        randomMover.movementEnabled = false;
                }
                else
                {
                    if (randomMover != null)
                        randomMover.movementEnabled = true;
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
            randomMover.movementEnabled = true;
    }
}
