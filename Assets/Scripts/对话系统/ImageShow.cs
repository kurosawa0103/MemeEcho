using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageShow : MonoBehaviour
{
    public RectTransform imageRect;
    public Image image;

    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    public float scaleInDuration = 1f;
    public float scaleOutDuration = 1f;

    public void SetupImage(Sprite sprite, float fadeIn, float fadeOut, float scaleIn, float scaleOut)
    {
        fadeInDuration = fadeIn;
        fadeOutDuration = fadeOut;
        scaleInDuration = scaleIn;
        scaleOutDuration = scaleOut;

        image.sprite = sprite;
        image.color = new Color(1, 1, 1, 0);
        imageRect.localScale = Vector3.zero;

        imageRect.DOScale(Vector3.one, scaleInDuration).SetEase(Ease.OutBack);
        image.DOFade(1f, fadeInDuration).SetEase(Ease.Linear);
    }

    public void SetupCharacterImage(Sprite dialogImage, float fadeIn, float fadeOut, float scaleIn, float scaleOut)
    {
        SetupImage(dialogImage, fadeIn, fadeOut, scaleIn, scaleOut);
    }

    public void CloseImage(float fadeOut, float scaleOut)
    {
        imageRect.DOScale(Vector3.zero, scaleOut).SetEase(Ease.InBack);
        image.DOFade(0f, fadeOut).SetEase(Ease.Linear);


        DOVirtual.DelayedCall(Mathf.Max(fadeOut, scaleOut), () => Destroy(gameObject));
    }
}

