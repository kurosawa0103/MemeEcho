using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class UIResizeAnimation : MonoBehaviour
{
    [Header("目标UI元素")]
    public RectTransform targetUI;

    [Header("动画设置")]
    public float animationDuration = 0.5f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("全屏状态")]
    public Vector2 fullscreenPosition;
    public Vector2 fullscreenSize;

    [Header("缩小状态")]
    public Vector2 minimizedPosition;
    public Vector2 minimizedSize;

    [Header("触发按钮")]
    public Button toggleButton;

    [Header("事件")]
    public UnityEvent onExpandComplete;
    public UnityEvent onMinimizeComplete;

    private bool isFullscreen = true;
    private Coroutine currentAnimation;

    void Start()
    {
        // 确保目标UI已设置
        if (targetUI == null)
        {
            targetUI = GetComponent<RectTransform>();
        }

        // 初始化为全屏状态
        SetFullscreenState();

        // 添加按钮监听
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleSize);
        }
    }

    /// <summary>
    /// 切换UI大小状态
    /// </summary>
    public void ToggleSize()
    {
        // 停止当前正在运行的动画
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        if (isFullscreen)
        {
            // 从全屏切换到缩小状态
            currentAnimation = StartCoroutine(AnimateUI(
                targetUI.anchoredPosition, minimizedPosition,
                targetUI.sizeDelta, minimizedSize,
                animationDuration
            ));
            isFullscreen = false;
        }
        else
        {
            // 从缩小状态切换到全屏
            currentAnimation = StartCoroutine(AnimateUI(
                targetUI.anchoredPosition, fullscreenPosition,
                targetUI.sizeDelta, fullscreenSize,
                animationDuration
            ));
            isFullscreen = true;
        }
    }

    /// <summary>
    /// 立即设置为全屏状态
    /// </summary>
    public void SetFullscreenState()
    {
        targetUI.anchoredPosition = fullscreenPosition;
        targetUI.sizeDelta = fullscreenSize;
        isFullscreen = true;
    }

    /// <summary>
    /// 立即设置为缩小状态
    /// </summary>
    public void SetMinimizedState()
    {
        targetUI.anchoredPosition = minimizedPosition;
        targetUI.sizeDelta = minimizedSize;
        isFullscreen = false;
    }

    /// <summary>
    /// 动画协程
    /// </summary>
    private IEnumerator AnimateUI(Vector2 startPos, Vector2 endPos, Vector2 startSize, Vector2 endSize, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curvedT = animationCurve.Evaluate(t);

            // 插值计算位置和大小
            targetUI.anchoredPosition = Vector2.Lerp(startPos, endPos, curvedT);
            targetUI.sizeDelta = Vector2.Lerp(startSize, endSize, curvedT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终设置精确的目标值
        targetUI.anchoredPosition = endPos;
        targetUI.sizeDelta = endSize;

        // 触发相应事件
        if (isFullscreen)
        {
            onExpandComplete?.Invoke();
        }
        else
        {
            onMinimizeComplete?.Invoke();
        }

        currentAnimation = null;
    }

    /// <summary>
    /// 直接展开UI到全屏
    /// </summary>
    public void ExpandUI()
    {
        if (!isFullscreen)
        {
            ToggleSize();
        }
    }

    /// <summary>
    /// 直接缩小UI
    /// </summary>
    public void MinimizeUI()
    {
        if (isFullscreen)
        {
            ToggleSize();
        }
    }
}