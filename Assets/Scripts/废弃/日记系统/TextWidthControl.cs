using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextWidthControl : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float maxWidth = 300f; // ���������

    void Start()
    {
        AdjustTextWidth();
    }

    void AdjustTextWidth()
    {
        // ��ȡ RectTransform ���
        RectTransform rectTransform = textMeshPro.GetComponent<RectTransform>();

        // ��ȡ�ı���ʵ�ʿ��
        float textWidth = textMeshPro.preferredWidth;

        // ����ʵ�ʿ���������֮��Ĳ���
        float finalWidth = Mathf.Min(textWidth, maxWidth);

        // ���� RectTransform ���
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, finalWidth);

        // ʹ Content Size Fitter ��Ӧ�ı��ĸ߶�
        ContentSizeFitter fitter = textMeshPro.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // ���û���
        textMeshPro.enableWordWrapping = true;
    }
}
