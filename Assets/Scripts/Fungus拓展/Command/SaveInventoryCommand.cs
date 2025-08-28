using UnityEngine;
using Fungus;

[CommandInfo("Inventory", "SaveInventory", "调用 InventorySystem 保存背包状态")]
public class SaveInventoryCommand : Command
{
    public override void OnEnter()
    {
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory == null)
        {
            Debug.LogError("场景中没有找到 InventorySystem");
            Continue();
            return;
        }

        inventory.SaveInventory();
        Debug.Log("Fungus Command 已调用 InventorySystem 保存背包");

        Continue();
    }
}
