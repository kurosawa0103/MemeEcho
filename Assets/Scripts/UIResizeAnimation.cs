using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class UIResizeAnimation : MonoBehaviour
{
    [Header("Ŀ��UIԪ��")]
    public RectTransform targetUI;

    [Header("��������")]
    public float animationDuration = 0.5f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("ȫ��״̬")]
    public Vector2 fullscreenPosition;
    public Vector2 fullscreenSize;

    [Header("��С״̬")]
    public Vector2 minimizedPosition;
    public Vector2 minimizedSize;

    [Header("������ť")]
    public Button toggleButton;

    [Header("�¼�")]
    public UnityEvent onExpandComplete;
    public UnityEvent onMinimizeComplete;

    private bool isFullscreen = true;
    private Coroutine currentAnimation;

    void Start()
    {
        // ȷ��Ŀ��UI������
        if (targetUI == null)
        {
            targetUI = GetComponent<RectTransform>();
        }

        // ��ʼ��Ϊȫ��״̬
        SetFullscreenState();

        // ��Ӱ�ť����
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleSize);
        }
    }

    /// <summary>
    /// �л�UI��С״̬
    /// </summary>
    public void ToggleSize()
    {
        // ֹͣ��ǰ�������еĶ���
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        if (isFullscreen)
        {
            // ��ȫ���л�����С״̬
            currentAnimation = StartCoroutine(AnimateUI(
                targetUI.anchoredPosition, minimizedPosition,
                targetUI.sizeDelta, minimizedSize,
                animationDuration
            ));
            isFullscreen = false;
        }
        else
        {
            // ����С״̬�л���ȫ��
            currentAnimation = StartCoroutine(AnimateUI(
                targetUI.anchoredPosition, fullscreenPosition,
                targetUI.sizeDelta, fullscreenSize,
                animationDuration
            ));
            isFullscreen = true;
        }
    }

    /// <summary>
    /// ��������Ϊȫ��״̬
    /// </summary>
    public void SetFullscreenState()
    {
        targetUI.anchoredPosition = fullscreenPosition;
        targetUI.sizeDelta = fullscreenSize;
        isFullscreen = true;
    }

    /// <summary>
    /// ��������Ϊ��С״̬
    /// </summary>
    public void SetMinimizedState()
    {
        targetUI.anchoredPosition = minimizedPosition;
        targetUI.sizeDelta = minimizedSize;
        isFullscreen = false;
    }

    /// <summary>
    /// ����Э��
    /// </summary>
    private IEnumerator AnimateUI(Vector2 startPos, Vector2 endPos, Vector2 startSize, Vector2 endSize, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curvedT = animationCurve.Evaluate(t);

            // ��ֵ����λ�úʹ�С
            targetUI.anchoredPosition = Vector2.Lerp(startPos, endPos, curvedT);
            targetUI.sizeDelta = Vector2.Lerp(startSize, endSize, curvedT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ȷ���������þ�ȷ��Ŀ��ֵ
        targetUI.anchoredPosition = endPos;
        targetUI.sizeDelta = endSize;

        // ������Ӧ�¼�
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
    /// ֱ��չ��UI��ȫ��
    /// </summary>
    public void ExpandUI()
    {
        if (!isFullscreen)
        {
            ToggleSize();
        }
    }

    /// <summary>
    /// ֱ����СUI
    /// </summary>
    public void MinimizeUI()
    {
        if (isFullscreen)
        {
            ToggleSize();
        }
    }
}