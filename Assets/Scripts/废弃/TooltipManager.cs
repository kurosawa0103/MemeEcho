using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipPanel; // 提示窗口
    public TextMeshProUGUI tooltipText; // 提示窗口文本
    public TextMeshProUGUI toolNameText; // 提示窗口文本
    public Image toolItemImage;
    private void Start()
    {
        // 确保工具提示一开始是隐藏的
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 鼠标悬浮在按钮上时显示工具提示
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 鼠标离开按钮时隐藏工具提示
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}
