using UnityEngine;
using Fungus;
using DG.Tweening;
using UnityEngine.UI; // UI Image

[CommandInfo("Custom", "ChangeMapFade", "�л���ͼʱ����/����Ч����ʹ�� DOTween��UI Image��Ļ��")]
public class ChangeMapFade : Command
{
    public enum FadeType { FadeIn, FadeOut }

    [Tooltip("Ŀ����󣬰��� SpriteRenderer����Ϊ��")]
    public GameObject target;

    [Tooltip("��Ļ���󣬰��� UI Image������")]
    public GameObject blackScreen;

    [Tooltip("���� or ����")]
    public FadeType fadeType = FadeType.FadeOut;

    [Tooltip("Ŀ�����ź�͸���ȱ仯ʱ��")]
    public float duration = 1f;

    [Tooltip("��Ļ���뵭��ʱ��")]
    public float blackFadeDuration = 1f;

    [Tooltip("�Ƿ�ȴ���Ļ��ɺ��ټ�������")]
    public bool waitForBlackFade = true;

    public override void OnEnter()
    {
        if (blackScreen == null)
        {
            Debug.LogWarning("ChangeMapFade: blackScreen δ��ֵ");
            Continue();
            return;
        }

        SpriteRenderer targetSR = target != null ? target.GetComponent<SpriteRenderer>() : null;
        Image blackImage = blackScreen.GetComponent<Image>();

        if (blackImage == null)
        {
            Debug.LogWarning("ChangeMapFade: blackScreen ȱ�� Image ���");
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
        string targetName = target != null ? target.name : "(��)";
        string blackName = blackScreen != null ? blackScreen.name : "(δ��ֵ)";
        return $"{fadeType} {targetName} with {blackName}" + (waitForBlackFade ? " [�ȴ���Ļ���]" : "");
    }

    public override Color GetButtonColor()
    {
        return new Color(0.7f, 0.5f, 0.8f);
    }
}
