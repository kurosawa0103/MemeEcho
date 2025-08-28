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
    public CanvasGroup dialogCanvasGroup;  // ��� CanvasGroup �������嵭�뵭��
    public bool dialogBoxUseScale = true;

    public string sfxName= "SE_bubble_high";
    public void SetupDialog(string dialogText, float fadeInDuration, float scaleInDuration, float fontSize, bool dialogBoxUseScale,Vector2  boxSize, string sfxName)
    {
        rootBox.sizeDelta = boxSize;

        // �����ı�
        textMeshPro.text = dialogText;
        textMeshPro.alpha = 0; // ��ʼ͸����
        textMeshPro.fontSize = fontSize;

        // �޳����м��������ݣ��������ı���ǩ��
        string cleanText = Regex.Replace(dialogText, "<.*?>", "");
        textMeshPro.transform.GetComponent<I2.Loc.Localize>().SetTerm(cleanText); //������
        // ��ʼ��״̬
        if (dialogBoxUseScale)
        {
            dialogBox.localScale = Vector3.zero; // ��ʼΪ��
        }
        else
        {
            dialogBox.localScale = Vector3.one; // ��ʼΪ������С
        }
        SoundManager.Instance.PlaySFX(sfxName);
        // ���ʹ��CanvasGroup����
        if (dialogCanvasGroup != null)
        {
            dialogCanvasGroup.alpha = dialogBoxUseScale ? 1f : 0f; // ��ʼ͸����
        }

        textMeshPro.alpha = dialogBoxUseScale ? 0f : 1f; // ��ʼ͸����

        // ����Ի�������ź͵���Ч��
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
                Destroy(gameObject); // �Ի�������
            });
        }
        else
        {
            dialogCanvasGroup.DOFade(0f, fadeOutDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                Destroy(gameObject); // �Ի�������
            });

        }
    }
}
