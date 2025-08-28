using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using I2.Loc;
using System.Text.RegularExpressions;

public class BubbleTextShow : MonoBehaviour
{
    public RectTransform rootBox;
    public RectTransform dialogBox;
    public TextMeshProUGUI textMeshPro;
    public CanvasGroup dialogCanvasGroup;  // 添加 CanvasGroup 控制整体淡入淡出
    public bool dialogBoxUseScale = true;

    public string sfxName= "SE_bubble_high";
    public void SetupDialog(string dialogText, float fadeInDuration, float scaleInDuration, float fontSize, bool dialogBoxUseScale,Vector2  boxSize, string sfxName)
    {
        rootBox.sizeDelta = boxSize;

        // 设置文本
        textMeshPro.text = dialogText;
        textMeshPro.alpha = 0; // 初始透明度
        textMeshPro.fontSize = fontSize;

        // 剔除所有尖括号内容（包括富文本标签）
        string cleanText = Regex.Replace(dialogText, "<.*?>", "");
        textMeshPro.transform.GetComponent<I2.Loc.Localize>().SetTerm(cleanText); //多语言
        // 初始化状态
        if (dialogBoxUseScale)
        {
            dialogBox.localScale = Vector3.zero; // 初始为零
        }
        else
        {
            dialogBox.localScale = Vector3.one; // 初始为正常大小
        }
        SoundManager.Instance.PlaySFX(sfxName);
        // 如果使用CanvasGroup淡入
        if (dialogCanvasGroup != null)
        {
            dialogCanvasGroup.alpha = dialogBoxUseScale ? 1f : 0f; // 初始透明度
        }

        textMeshPro.alpha = dialogBoxUseScale ? 0f : 1f; // 初始透明度

        // 处理对话框的缩放和淡入效果
        if (dialogBoxUseScale)
        {
            dialogBox.localScale = Vector3.zero;
            dialogBox.DOScale(Vector3.one, scaleInDuration).SetEase(Ease.OutBack);
            textMeshPro.DOFade(1f, fadeInDuration).SetEase(Ease.Linear);
        }
        else
        {
            dialogCanvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.Linear);
        }
    }

    public void CloseDialog(float fadeOutDuration, float scaleOutDuration)
    {
        if (dialogBoxUseScale)
        {
            dialogBox.DOScale(Vector3.zero, scaleOutDuration).SetEase(Ease.InBack);
            textMeshPro.DOFade(0f, fadeOutDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                Destroy(gameObject); // 对话框销毁
            });
        }
        else
        {
            dialogCanvasGroup.DOFade(0f, fadeOutDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                Destroy(gameObject); // 对话框销毁
            });

        }
    }
}
