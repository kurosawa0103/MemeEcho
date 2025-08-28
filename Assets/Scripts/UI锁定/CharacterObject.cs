using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class CharacterObject : MonoBehaviour
{
    public string trackName;
    public bool needRecord;
    [ShowIf("needRecord", true)]
    public string eventName;
    [ShowIf("needRecord", true)]
    public string eventValue;
    [ShowIf("needRecord", true), ReadOnly]
    public float accumulatedLockTime = 0f;  // 累积的锁定时间
    [ShowIf("needRecord", true)]
    public float targetTime;
    [ShowIf("needRecord", true)]
    public bool hasRecorded = false;        // 标志，记录是否已经执行过

    public bool needPhoto;
    [ShowIf("needPhoto", true)]
    public string photoTargetName;  // 只有在 needPhoto 为 true 时显示

    public bool canClickTalk=true;//用于点击对话检测

    [Header("鼠标气泡控制")]
    public bool canShow = true;

    [LabelText("气泡对象（UI或3D物体）")]
    public GameObject speechBubble;

    private bool isUIDragging = false;
    private Tween currentTween;
    private SpriteRenderer spriteRenderer;

    private Tween hoverTween;
    private Vector3 initialLocalPosition;
    private Tween idleTween;
    [Header("气泡显示逻辑控制")]
    public bool onlyShowOnHover = false;  // 勾选后仅在鼠标悬浮时才显示气泡
    public float targetHintScale = 1;

    private void Awake()
    {
        if (speechBubble != null)
        {
            spriteRenderer = speechBubble.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogWarning("speechBubble 需要挂载 SpriteRenderer 组件");
            }

            initialLocalPosition = speechBubble.transform.localPosition;

            // 如果启用了“仅悬浮时显示”，则初始隐藏气泡并设置为不可见状态
            if (onlyShowOnHover)
            {
                speechBubble.transform.localScale = Vector3.zero;
                if (spriteRenderer != null)
                {
                    Color c = spriteRenderer.color;
                    spriteRenderer.color = new Color(c.r, c.g, c.b, 0f);
                }
                speechBubble.SetActive(false);
            }
        }
    }


    private void OnEnable()
    {
        DraggableUIWindow.OnDraggingChanged += OnDraggingChanged;
        StartIdleAnimation(); //  开启默认跳动
    }

    private void OnDisable()
    {
        DraggableUIWindow.OnDraggingChanged -= OnDraggingChanged;
        StopIdleAnimation(); //  关闭默认跳动
    }

    private void OnDraggingChanged(bool isDragging)
    {
        isUIDragging = isDragging;
    }


    private void OnMouseOver()
    {
        if (canShow && !isUIDragging)
        {
            if (speechBubble != null )
            {
                StartHoverAnimation();
            }
        }
    }
    private void OnMouseEnter()
    {
        if (canShow && onlyShowOnHover)
        {
            ShowBubble(); //  勾选后进入时才显示
        }
    }
    private void OnMouseExit()
    {
        StopHoverAnimation();
        if (onlyShowOnHover)
        {
            HideBubble(); // 勾选后离开时才隐藏
        }
    }

    public void ShowBubble()
{
    if (speechBubble == null || spriteRenderer == null) return;

    currentTween?.Kill();

    speechBubble.SetActive(true);
    speechBubble.transform.localPosition = initialLocalPosition;
    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
    speechBubble.transform.localScale = Vector3.zero;

        currentTween = DOTween.Sequence()
            .Append(spriteRenderer.DOFade(1f, 0.4f))
            .Join(speechBubble.transform.DOScale(targetHintScale, 0.4f).SetEase(Ease.OutBack))
            .Join(speechBubble.transform.DOShakePosition(
                    0.4f,
                    0.1f,
                    10,
                    90
                    ).SetRelative(true));
    }


    public void HideBubble()
    {
        if (speechBubble == null || spriteRenderer == null) return;

        currentTween?.Kill();

        currentTween = DOTween.Sequence()
            .Append(spriteRenderer.DOFade(0f, 0.5f))
            .Join(speechBubble.transform.DOScale(0f, 0.5f))
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                speechBubble.SetActive(false);
            });
    }

    private void StartHoverAnimation()
    {
        if (hoverTween != null && hoverTween.IsActive())
            return; // 避免重复触发

        hoverTween = speechBubble.transform.DOPunchPosition(
            new Vector3(0, 0.08f, 0), // 向上弹0.05单位
            0.8f,                     // 动画时长0.5秒
            8,                       // 震动次数
            1f                        // 振幅强度
        ).OnComplete(() =>
        {
            // 动画完成后再归位一次
            speechBubble.transform.localPosition = initialLocalPosition;
            hoverTween = null;
        });
    }

    private void StopHoverAnimation()
    {
        // Punch动画结束自动归位，不用强制停止
        // 这里清理下引用，避免误判动画状态
        if (hoverTween != null)
        {
            hoverTween.Kill();
            hoverTween = null;
        }
        if (speechBubble != null)
        {
            // 离开时也强制归位
            speechBubble.transform.localPosition = initialLocalPosition;
        }
    }
    private void StartIdleAnimation()
    {
        if (speechBubble == null) return;

        // 避免重复
        if (idleTween != null && idleTween.IsActive()) return;

        idleTween = DOTween.Sequence()
            .AppendCallback(() =>
            {
                speechBubble.transform.DOPunchPosition(
                    new Vector3(0, 0.05f, 0f), //  轻微跳动
                    0.5f,
                    4,
                    0.5f
                ).SetRelative(true);
            })
            .AppendInterval(0.5f) // 每1.5秒跳一下
            .SetLoops(-1, LoopType.Restart);
    }
    private void StopIdleAnimation()
    {
        if (idleTween != null)
        {
            idleTween.Kill();
            idleTween = null;
        }
    }

    public bool HasReachedRecordTime()
    {
        return accumulatedLockTime >= targetTime;
    }

}
