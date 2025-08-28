using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneFader : MonoBehaviour
{
    public Image blackScreenImage; // 引用全屏黑色 Image
    public float fadeDuration = 2f;

    void Awake()
    {
        if (blackScreenImage != null)
        {
            var color = blackScreenImage.color;
            color.a = 1f; // 确保初始为黑色
            blackScreenImage.color = color;
            blackScreenImage.gameObject.SetActive(true);

            // 淡出动画
            blackScreenImage.DOFade(0f, fadeDuration).OnComplete(() => {
                blackScreenImage.gameObject.SetActive(false);
            });
        }
    }
}
