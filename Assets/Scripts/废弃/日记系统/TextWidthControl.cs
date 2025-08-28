using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextWidthControl : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float maxWidth = 300f; // 设置最大宽度

    void Start()
    {
        AdjustTextWidth();
    }

    void AdjustTextWidth()
    {
        // 获取 RectTransform 组件
        RectTransform rectTransform = textMeshPro.GetComponent<RectTransform>();

        // 获取文本的实际宽度
        float textWidth = textMeshPro.preferredWidth;

        // 计算实际宽度与最大宽度之间的差异
        float finalWidth = Mathf.Min(textWidth, maxWidth);

        // 设置 RectTransform 宽度
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, finalWidth);

        // 使 Content Size Fitter 适应文本的高度
        ContentSizeFitter fitter = textMeshPro.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // 启用换行
        textMeshPro.enableWordWrapping = true;
    }
}
