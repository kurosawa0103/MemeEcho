using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TipPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tipPanel; // 提示窗口
    public TextMeshProUGUI tooltipText; // 提示窗口文本
    [TextArea(3, 8)]
    public string tipText;
    private void Start()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 鼠标悬浮在按钮上时显示工具提示
        if (tipPanel != null)
        {
            tooltipText.text = tipText;
            tipPanel.SetActive(true);
            // 获取物品的RectTransform
            RectTransform itemRectTransform = GetComponent<RectTransform>();

            // 计算提示窗口的位置：格子右下角
            Vector3 tooltipPosition = itemRectTransform.position;
            tooltipPosition.x += itemRectTransform.rect.width / 2 + tipPanel.GetComponent<RectTransform>().rect.width / 2;
            tooltipPosition.y -= itemRectTransform.rect.height / 2 + tipPanel.GetComponent<RectTransform>().rect.height / 2;

            // 将提示窗口的位置设置到格子右下角
            tipPanel.transform.position = tooltipPosition;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 鼠标离开按钮时隐藏工具提示
        if (tipPanel != null)
        {
            tipPanel.SetActive(false);
        }
    }
}
