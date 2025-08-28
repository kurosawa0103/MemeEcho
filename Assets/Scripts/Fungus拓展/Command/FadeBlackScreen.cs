using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Fungus;

[CommandInfo("Custom", "Fade Black Screen", "使用DOTween对黑幕Image进行淡入或淡出")]
public class FadeBlackScreen : Command
{
    public enum FadeType { FadeIn, FadeOut }

    [Tooltip("淡入或淡出")]
    public FadeType fadeType = FadeType.FadeIn;

    [Tooltip("动画持续时间")]
    public float duration = 1f;

    [Tooltip("是否等待动画完成再继续流程")]
    public bool waitUntilFinished = true;

    public Image targetImage;

    public override void OnEnter()
    {
        FindTargetImage();

        if (targetImage == null)
        {
            Debug.LogWarning("找不到黑幕 Image！");
            Continue();
            return;
        }

        // 确保激活
        if (!targetImage.gameObject.activeInHierarchy)
        {
            targetImage.gameObject.SetActive(true);
        }

        float startAlpha = fadeType == FadeType.FadeIn ? 0f : 1f;
        float endAlpha = fadeType == FadeType.FadeIn ? 1f : 0f;

        // 设置初始 alpha
        Color color = targetImage.color;
        color.a = startAlpha;
        targetImage.color = color;

        Tween tween = targetImage.DOFade(endAlpha, duration);

        tween.OnComplete(() =>
        {
            if (fadeType == FadeType.FadeOut)
            {
                targetImage.gameObject.SetActive(false);
            }

            if (waitUntilFinished)
            {
                Continue();
            }
        });

        if (!waitUntilFinished)
        {
            Continue();
        }
    }

    private void FindTargetImage()
    {
        if (targetImage == null)
        {
            GameObject boxBorder = GameObject.FindGameObjectWithTag("BoxBorder");
            if (boxBorder != null && boxBorder.transform.childCount > 0)
            {
                Transform firstChild = boxBorder.transform.GetChild(0);
                foreach (Transform child in firstChild)
                {
                    if (child.name == "黑幕")
                    {
                        targetImage = child.GetComponent<Image>();
                        break;
                    }
                }
            }
        }
    }

    public override string GetSummary()
    {
        return $"{fadeType} over {duration}s (Wait: {waitUntilFinished})";
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }
}
