using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using I2.Loc;
using System.Text.RegularExpressions;

public class DialogShow : MonoBehaviour
{
    [Tooltip("The duration of the fade-in animation for the text")]
    public float fadeInDuration = 1f;

    [Tooltip("The duration of the fade-out animation for the text")]
    public float fadeOutDuration = 1f;

    [Tooltip("The duration for dialog box scale-in")]
    public float scaleInDuration = 1f;

    [Tooltip("The duration for dialog box scale-out")]
    public float scaleOutDuration = 1f;

    public float waitTime;

    public RectTransform dialogBox;
    public TextMeshProUGUI textMeshPro;


    public void SetupDialog(string dialogText, float fadeInDuration, float fadeOutDuration, float scaleInDuration, float scaleOutDuration, float fontSize)
    {
        this.fadeInDuration = fadeInDuration;
        this.fadeOutDuration = fadeOutDuration;
        this.scaleInDuration = scaleInDuration;
        this.scaleOutDuration = scaleOutDuration;

        textMeshPro.fontSize = fontSize;
        textMeshPro.text = ""; // 清空文字
        textMeshPro.alpha = 0;

        textMeshPro.text = dialogText;

        // 剔除所有尖括号内容（包括富文本标签）
        string cleanText = Regex.Replace(dialogText, "<.*?>", "");
        textMeshPro.transform.GetComponent<I2.Loc.Localize>().SetTerm(cleanText); //多语言

        dialogBox.GetComponent<Image>().DOFade(0.5f, scaleInDuration).SetEase(Ease.OutBack);

        textMeshPro.DOFade(1f, fadeInDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            
        });
    } 

    public void CloseDialog(float fadeOutDuration, float scaleOutDuration)
    {
        // 文本渐出
        dialogBox.GetComponent <Image>().DOFade(0, scaleOutDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(gameObject); // 对话框销毁
        });

        textMeshPro.DOFade(0f, fadeOutDuration).SetEase(Ease.Linear);
    }
}
