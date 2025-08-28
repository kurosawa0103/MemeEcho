using UnityEngine;
using Fungus;

[CommandInfo("Inventory", "SaveInventory", "���� InventorySystem ���汳��״̬")]
public class SaveInventoryCommand : Command
{
    public override void OnEnter()
    {
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory == null)
        {
            Debug.LogError("������û���ҵ� InventorySystem");
            Continue();
            return;
        }

        inventory.SaveInventory();
        Debug.Log("Fungus Command �ѵ��� InventorySystem ���汳��");

        Continue();
    }
}
