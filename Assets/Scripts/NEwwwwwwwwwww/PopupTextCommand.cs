using System.Collections;
using UnityEngine;
using TMPro;
using Fungus;
using DG.Tweening;

[CommandInfo("UI", "PopupTextDOTween", "显示弹出文字框，带DOTween动效和自动消失")]
public class PopupTextDOTween : Command
{
    [Tooltip("显示的文字内容")]
    [TextArea]
    public string message;

    [Tooltip("显示的 Panel (包含背景和文本)")]
    public GameObject popupPanel;

    [Tooltip("TMP_Text 组件，用于显示文字")]
    public TMP_Text popupText;

    [Tooltip("弹出动画持续时间")]
    public float punchDuration = 0.5f;

    [Tooltip("停留时间，秒")]
    public float displayTime = 3f;

    [Tooltip("缩放弹跳幅度")]
    public float punchStrength = 1.1f;

    [Tooltip("是否等待动画和显示结束后再继续流程")]
    public bool waitForComplete = true;

    public override void OnEnter()
    {
        if (popupPanel == null || popupText == null)
        {
            Debug.LogWarning("PopupTextDOTween: Panel 或 Text 未配置！");
            Continue();
            return;
        }

        // 设置文字
        popupText.text = message;

        // 激活 Panel
        popupPanel.SetActive(true);

        // 重置缩放
        popupPanel.transform.localScale = Vector3.zero;

        // DOTween 弹跳动画
        popupPanel.transform
            .DOScale(Vector3.one * punchStrength, punchDuration / 2)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                popupPanel.transform.DOScale(Vector3.one, punchDuration / 2).SetEase(Ease.OutBack);
            });

        if (waitForComplete)
        {
            StartCoroutine(HideAfterDelay());
        }
        else
        {
            // 不等待，直接继续
            Continue();
            StartCoroutine(HideAfterDelay(false));
        }
    }

    private IEnumerator HideAfterDelay(bool callContinue = true)
    {
        yield return new WaitForSeconds(displayTime);

        // 缩小消失
        popupPanel.transform.DOScale(Vector3.zero, punchDuration).SetEase(Ease.InBack);

        yield return new WaitForSeconds(punchDuration);

        popupPanel.SetActive(false);

        if (callContinue && waitForComplete)
        {
            Continue();
        }
    }

    public override string GetSummary()
    {
        string panelName = popupPanel != null ? popupPanel.name : "未配置Panel";
        string textPreview = !string.IsNullOrEmpty(message) ? message : "未配置文字";
        return $"Panel: {panelName}, Text: {textPreview}, 等待结束: {waitForComplete}";
    }
}
