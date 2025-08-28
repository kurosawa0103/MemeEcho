using UnityEngine;
using Fungus;

[EventHandlerInfo("Inventory", "On Item Changed", "����û��Ƴ�ָ�� Item ʱ���� Block")]
public class OnItemChanged : EventHandler
{
    [Tooltip("Ҫ��������Ʒ")]
    public Item targetItem;

    [Tooltip("�Ƿ��ڻ�ø���Ʒʱ����")]
    public bool triggerOnAdd = true;

    [Tooltip("�Ƿ����Ƴ�����Ʒʱ����")]
    public bool triggerOnRemove = false;

    [Tooltip("�Ƿ�ֻ����һ��")]
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
            return "δָ����Ʒ";

        string summary = targetItem.itemName + "��";
        if (triggerOnAdd) summary += " ���ʱ";
        if (triggerOnAdd && triggerOnRemove) summary += " / ";
        if (triggerOnRemove) summary += " �Ƴ�ʱ";
        if (triggerOnce) summary += "��������һ�Σ�";
        return summary;
    }
}
