using UnityEngine;
using Fungus;

[CommandInfo("背包系统", "获得/移除物品", "向背包中添加或移除一个指定的物品")]
public class AddItemToInventoryCommand : Command
{
    [Tooltip("要添加或移除的物品配置")]
    public Item item;

    [Tooltip("勾选则为移除操作，否则为添加")]
    public bool removeInstead = false;

    public Vector3 newPosition;

    public override void OnEnter()
    {
        if (item != null)
        {
            InventorySystem inventory = GameObject.FindObjectOfType<InventorySystem>();
            if (inventory != null)
            {
                if (removeInstead)
                {
                    inventory.RemoveItemFromInventory(item);
                    Debug.Log($"Fungus 命令移除物品：{item.itemName}");
                }
                else
                {
                    inventory.AddItemToInventory(item, newPosition); // 调用带坐标的版本

                    Debug.Log($"Fungus 命令添加物品：{item.itemName}，位置：{newPosition}");
                }
            }
            else
            {
                Debug.LogWarning("找不到 InventorySystem 实例！");
            }
        }
        else
        {
            Debug.LogWarning("未配置 Item！");
        }

        Continue(); // 继续执行下一条指令
    }


    public override string GetSummary()
    {
        if (item == null)
        {
            return "未设置物品";
        }

        string action = removeInstead ? "移除" : "添加";

        if (removeInstead)
        {
            return $"{action}物品：{item.itemName}";
        }
        else
        {
            return $"{action}物品：{item.itemName}（位置：{newPosition}）";
        }
    }


    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }
}
