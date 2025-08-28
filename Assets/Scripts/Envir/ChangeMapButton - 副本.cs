using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class ChangeMapButton1 : MonoBehaviour
{
    [Tooltip("当 Box 进入触发区域时要激活的物体")]
    public GameObject targetObject;

    [Header("淡入淡出设置")]
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;

    [Header("缩放设置")]
    public Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f);
    public float scaleDuration = 0.3f;
    public GameObject arrowObject;
    public Vector3 arrowTargetScale = Vector3.one;
    public Vector3 arrowOriginalScale = Vector3.zero;
    public float arrowScaleDuration = 0.3f;


    [Header("额外缩放物体设置")]
    public GameObject fadeOtherObject;
    public Vector3 fadeOtherTargetScale = Vector3.zero;
    public float fadeOtherScaleDuration = 0.3f;

    private Vector3 originalScale;
    private Vector3 fadeOtherOriginalScale;
    private SpriteRenderer spriteRenderer;

    private bool isFading = false;
    private bool isVisible = false;
    private bool isMouseOver = false;

    [SerializeField]
    private bool _canShow = true;

    private Sequence enterSequence;
    private Sequence exitSequence;

    private bool arrowStarted = false;
    public bool canShow
    {
        get => _canShow;
        set
        {
            if (_canShow != value)
            {
                _canShow = value;

                if (!_canShow)
                {
                    if (isVisible && spriteRenderer != null)
                        HideTarget();
                    ShrinkFadeOther();
                }
                else
                {
                    if (!isVisible && spriteRenderer != null)
                        ShowTarget();
                    RestoreFadeOther();
                }
            }
        }
    }

    private void Start()
    {
        if (targetObject != null)
        {
            spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
            originalScale = targetObject.transform.localScale;

            if (!canShow)
            {
                Color c = spriteRenderer.color;
                c.a = 0f;
                spriteRenderer.color = c;
                targetObject.SetActive(false);
                isVisible = false;
            }
        }

        if (fadeOtherObject != null)
        {
            fadeOtherOriginalScale = fadeOtherObject.transform.localScale;

            if (!canShow)
                fadeOtherObject.transform.localScale = fadeOtherTargetScale;
        }
    }

    private void Update()
    {
        if (!canShow) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == GetComponent<Collider>())
            {
                if (!isMouseOver)
                {
                    isMouseOver = true;
                    OnMouseEnterSafe();
                }
            }
            else
            {
                if (isMouseOver)
                {
                    isMouseOver = false;
                    OnMouseExitSafe();
                }
            }
        }
        else
        {
            if (isMouseOver)
            {
                isMouseOver = false;
                OnMouseExitSafe();
            }
        }
    }
    private void OnMouseEnterSafe()
    {
        // 终止所有动画
        enterSequence?.Kill();
        exitSequence?.Kill();
        DOTween.Kill(targetObject?.transform);
        DOTween.Kill(arrowObject?.transform);

        arrowStarted = false; // 重置状态

        enterSequence = DOTween.Sequence();

        enterSequence.Append(targetObject.transform.DOScale(targetScale, scaleDuration)
            .SetEase(Ease.OutBack))
            .AppendCallback(() => arrowStarted = true) // 当框框动画结束，标记箭头开始
            .Append(arrowObject.transform.DOScale(arrowTargetScale, arrowScaleDuration)
            .SetEase(Ease.OutQuad));
    }
    private void OnMouseExitSafe()
    {
        // 终止所有动画
        enterSequence?.Kill();
        exitSequence?.Kill();
        DOTween.Kill(targetObject?.transform);
        DOTween.Kill(arrowObject?.transform);

        exitSequence = DOTween.Sequence();

        if (arrowStarted)
        {
            // 箭头已经开始了 → 先缩箭头再缩框框
            exitSequence.Append(arrowObject.transform.DOScale(arrowOriginalScale, arrowScaleDuration)
                .SetEase(Ease.InBack))
                .Join(targetObject.transform.DOScale(originalScale, scaleDuration)
                .SetEase(Ease.InBack));
        }
        else
        {
            // 箭头还没开始 → 只缩框框
            exitSequence.Append(targetObject.transform.DOScale(originalScale, scaleDuration)
                .SetEase(Ease.InBack));
        }
    }



    private void ShrinkFadeOther()
    {
        if (fadeOtherObject != null)
        {
            fadeOtherObject.transform.DOKill();
            fadeOtherObject.transform.DOScale(fadeOtherTargetScale, fadeOtherScaleDuration).SetEase(Ease.InQuad);
        }
    }

    private void RestoreFadeOther()
    {
        if (fadeOtherObject != null)
        {
            fadeOtherObject.transform.DOKill();
            fadeOtherObject.transform.DOScale(fadeOtherOriginalScale, fadeOtherScaleDuration).SetEase(Ease.OutQuad);
        }
    }

    private void ShowTarget()
    {
        spriteRenderer.DOKill();
        isFading = true;
        targetObject.SetActive(true);

        spriteRenderer.DOFade(1f, fadeInDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                isFading = false;
                isVisible = true;
            });
    }

    private void HideTarget()
    {
        spriteRenderer.DOKill();
        isFading = true;

        spriteRenderer.DOFade(0f, fadeOutDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                targetObject.SetActive(false);
                isFading = false;
                isVisible = false;
            });
    }

    public void RefreshShowStatus()
    {
        if (canShow)
        {
            if (!isVisible)
                ShowTarget();
            else if (isVisible)
                HideTarget();
        }
        else
        {
            if (isVisible)
                HideTarget();
            ShrinkFadeOther();
        }
    }
}
