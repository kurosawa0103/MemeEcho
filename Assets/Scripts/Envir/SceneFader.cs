using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneFader : MonoBehaviour
{
    public Image blackScreenImage; // ����ȫ����ɫ Image
    public float fadeDuration = 2f;

    void Awake()
    {
        if (blackScreenImage != null)
        {
            var color = blackScreenImage.color;
            color.a = 1f; // ȷ����ʼΪ��ɫ
            blackScreenImage.color = color;
            blackScreenImage.gameObject.SetActive(true);

            // ��������
            blackScreenImage.DOFade(0f, fadeDuration).OnComplete(() => {
                blackScreenImage.gameObject.SetActive(false);
            });
        }
    }
}
