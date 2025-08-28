using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
                             IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("要进行缩放的目标对象。如果为空，则使用当前物体")]
    public Transform scaleTarget;

    public Vector3 targetScale = new Vector3(1.3f, 1.3f, 1.3f);
    public float duration = 0.5f;
    public float punchDuration = 0.2f;
    public Vector3 punchScale = new Vector3(0.2f, 0.2f, 0.2f);

    private Vector3 originalScale;
    private Tween currentTween;
    private bool isDragging = false;
    private bool isHovering = false;

    void Awake()
    {
        if (scaleTarget == null)
        {
            scaleTarget = transform;
        }

        originalScale = scaleTarget.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        if (isDragging) return;

        if (currentTween != null) currentTween.Kill();
        currentTween = scaleTarget.DOScale(targetScale, duration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (isDragging) return;

        if (currentTween != null) currentTween.Kill();
        currentTween = scaleTarget.DOScale(originalScale, duration).SetEase(Ease.OutBack);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        scaleTarget.DOKill();
        scaleTarget.DOPunchScale(punchScale, punchDuration, vibrato: 5, elasticity: 0.6f);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        // 保持放大状态，不触发任何缩放动画
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 拖拽逻辑视情况添加，这里不处理
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (!isHovering)
        {
            if (currentTween != null) currentTween.Kill();
            currentTween = scaleTarget.DOScale(originalScale, duration).SetEase(Ease.OutBack);
        }
    }
}
