using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Fungus;

[CommandInfo("Custom", "Fade Black Screen", "ʹ��DOTween�Ժ�ĻImage���е���򵭳�")]
public class FadeBlackScreen : Command
{
    public enum FadeType { FadeIn, FadeOut }

    [Tooltip("����򵭳�")]
    public FadeType fadeType = FadeType.FadeIn;

    [Tooltip("��������ʱ��")]
    public float duration = 1f;

    [Tooltip("�Ƿ�ȴ���������ټ�������")]
    public bool waitUntilFinished = true;

    public Image targetImage;

    public override void OnEnter()
    {
        FindTargetImage();

        if (targetImage == null)
        {
            Debug.LogWarning("�Ҳ�����Ļ Image��");
            Continue();
            return;
        }

        // ȷ������
        if (!targetImage.gameObject.activeInHierarchy)
        {
            targetImage.gameObject.SetActive(true);
        }

        float startAlpha = fadeType == FadeType.FadeIn ? 0f : 1f;
        float endAlpha = fadeType == FadeType.FadeIn ? 1f : 0f;

        // ���ó�ʼ alpha
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
                    if (child.name == "��Ļ")
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
