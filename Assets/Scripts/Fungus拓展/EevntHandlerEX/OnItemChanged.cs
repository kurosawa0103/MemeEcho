using UnityEngine;
using Fungus;

[EventHandlerInfo("Inventory", "On Item Changed", "当获得或移除指定 Item 时触发 Block")]
public class OnItemChanged : EventHandler
{
    [Tooltip("要监听的物品")]
    public Item targetItem;

    [Tooltip("是否在获得该物品时触发")]
    public bool triggerOnAdd = true;

    [Tooltip("是否在移除该物品时触发")]
    public bool triggerOnRemove = false;

    [Tooltip("是否只触发一次")]
    public bool triggerOnce = false;

    private bool triggered = false;

    void OnEnable()
    {
        InventorySystem.OnItemAdded += HandleItemAdded;
        InventorySystem.OnItemRemoved += HandleItemRemoved;
    }

    void OnDisable()
    {
        InventorySystem.OnItemAdded -= HandleItemAdded;
        InventorySystem.OnItemRemoved -= HandleItemRemoved;
    }

    void HandleItemAdded(Item item)
    {
        if (triggered && triggerOnce) return;
        if (triggerOnAdd && item == targetItem)
        {
            triggered = true;
            ExecuteBlock();
        }
    }

    void HandleItemRemoved(Item item)
    {
        if (triggered && triggerOnce) return;
        if (triggerOnRemove && item == targetItem)
        {
            triggered = true;
            ExecuteBlock();
        }
    }

    public override string GetSummary()
    {
        if (targetItem == null)
            return "未指定物品";

        string summary = targetItem.itemName + "：";
        if (triggerOnAdd) summary += " 获得时";
        if (triggerOnAdd && triggerOnRemove) summary += " / ";
        if (triggerOnRemove) summary += " 移除时";
        if (triggerOnce) summary += "（仅触发一次）";
        return summary;
    }
}
