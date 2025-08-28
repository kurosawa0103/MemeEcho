using UnityEngine;
using UnityEngine.EventSystems;
using Fungus;

[EventHandlerInfo("UI", "On UI Hover", "当鼠标悬停在指定 UI 元素上时触发事件")]
public class OnUIHover : EventHandler, IPointerEnterHandler
{
    [Tooltip("要监听的 UI 对象（需挂在 UI 上）")]
    public GameObject targetUI;

    [Tooltip("是否只触发一次")]
    public bool triggerOnce = false;

    private bool triggered = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (triggered && triggerOnce) return;

        if (eventData.pointerEnter == targetUI)
        {
            triggered = true;
            ExecuteBlock();
        }
    }

    public override string GetSummary()
    {
        return targetUI != null ? targetUI.name : "未设置 UI 元素";
    }

}
