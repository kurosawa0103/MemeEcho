using UnityEngine;
using UnityEngine.UI;
using Fungus;

[CommandInfo("Custom",
             "Set Item UseButton State (By Item)",
             "通过 Item 实例控制 DraggableItem 的 useButton 显示与交互")]
public class SetItemUseButtonStateByItem : Command
{
    [Tooltip("目标 Item 对象")]
    public Item targetItem;


    [Tooltip("是否允许交互")]
    public BooleanData interactable = new BooleanData(true);

    public override void OnEnter()
    {
        // 找到场景中所有 DraggableItem
        DraggableItem[] allItems = GameObject.FindObjectsOfType<DraggableItem>();
        foreach (var di in allItems)
        {
            if (di.thisItem == targetItem)
            {
                if (di.useButton != null)
                {
                    Button btn = di.useButton.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.interactable = interactable.Value;
                    }
                }
                else
                {
                    Debug.LogWarning("useButton 未设置在目标 DraggableItem 上！");
                }

                break; // 已找到目标，退出循环
            }
        }

        Continue();
    }

    public override string GetSummary()
    {
        return $"Set UseButton for '{targetItem?.name}' Interactable={interactable.Value}";
    }
}
