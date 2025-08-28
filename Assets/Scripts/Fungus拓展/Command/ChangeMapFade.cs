using UnityEngine;
using Fungus;
using DG.Tweening;
using UnityEngine.UI; // UI Image

[CommandInfo("Custom", "ChangeMapFade", "切换地图时淡入/淡出效果（使用 DOTween，UI Image黑幕）")]
public class ChangeMapFade : Command
{
    public enum FadeType { FadeIn, FadeOut }

    [Tooltip("目标对象，包含 SpriteRenderer，可为空")]
    public GameObject target;

    [Tooltip("黑幕对象，包含 UI Image，必填")]
    public GameObject blackScreen;

    [Tooltip("淡入 or 淡出")]
    public FadeType fadeType = FadeType.FadeOut;

    [Tooltip("目标缩放和透明度变化时间")]
    public float duration = 1f;

    [Tooltip("黑幕淡入淡出时间")]
    public float blackFadeDuration = 1f;

    [Tooltip("是否等待黑幕完成后再继续流程")]
    public bool waitForBlackFade = true;

    public override void OnEnter()
    {
        if (blackScreen == null)
        {
            Debug.LogWarning("ChangeMapFade: blackScreen 未赋值");
            Continue();
            return;
        }

        SpriteRenderer targetSR = target != null ? target.GetComponent<SpriteRenderer>() : null;
        Image blackImage = blackScreen.GetComponent<Image>();

        if (blackImage == null)
        {
            Debug.LogWarning("ChangeMapFade: blackScreen 缺少 Image 组件");
            Continue();
            return;
        }

        blackScreen.SetActive(true);
        DOTween.Kill(blackImage);

        if (target != null)
        {
            DOTween.Kill(target.transform);
            if (targetSR != null) DOTween.Kill(targetSR);
        }

        if (fadeType == FadeType.FadeOut)
        {
            SetAlpha(blackImage, 0f);

            Sequence seq = DOTween.Sequence();
            seq.Join(blackImage.DOFade(1f, blackFadeDuration));

            if (target != null)
            {
                if (targetSR != null) SetAlpha(targetSR, 1f);
                seq.Join(target.transform.DOScale(0f, duration));
                if (targetSR != null)
                    seq.Join(targetSR.DOFade(0f, duration));
            }

            seq.OnComplete(() =>
            {
                if (waitForBlackFade)
                {
                    Continue();
                }
                else
                {
                    Continue();
                }
            });
        }
        else // FadeIn
        {
            SetAlpha(blackImage, 1f);

            if (target != null)
            {
                target.transform.localScale = Vector3.zero;
                if (targetSR != null) SetAlpha(targetSR, 0f);
            }

            blackImage.DOFade(0f, blackFadeDuration).OnComplete(() =>
            {
                if (target != null)
                {
                    Sequence seq = DOTween.Sequence();
                    seq.Join(target.transform.DOScale(1f, duration));
                    if (targetSR != null)
                        seq.Join(targetSR.DOFade(1f, duration));

                    seq.OnComplete(() =>
                    {
                        blackScreen.SetActive(false);
                        if (waitForBlackFade)
                            Continue();
                        else
                            Continue();
                    });
                }
                else
                {
                    blackScreen.SetActive(false);
                    Continue();
                }
            });
        }
    }

    void SetAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    void SetAlpha(SpriteRenderer sr, float alpha)
    {
        if (sr == null) return;
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }

    public override string GetSummary()
    {
        string targetName = target != null ? target.name : "(无)";
        string blackName = blackScreen != null ? blackScreen.name : "(未赋值)";
        return $"{fadeType} {targetName} with {blackName}" + (waitForBlackFade ? " [等待黑幕完成]" : "");
    }

    public override Color GetButtonColor()
    {
        return new Color(0.7f, 0.5f, 0.8f);
    }
}
