using UnityEngine;
using Fungus;

[CommandInfo("����ϵͳ", "���/�Ƴ���Ʒ", "�򱳰�����ӻ��Ƴ�һ��ָ������Ʒ")]
public class AddItemToInventoryCommand : Command
{
    [Tooltip("Ҫ��ӻ��Ƴ�����Ʒ����")]
    public Item item;

    [Tooltip("��ѡ��Ϊ�Ƴ�����������Ϊ���")]
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
                    Debug.Log($"Fungus �����Ƴ���Ʒ��{item.itemName}");
                }
                else
                {
                    inventory.AddItemToInventory(item, newPosition); // ���ô�����İ汾

                    Debug.Log($"Fungus ���������Ʒ��{item.itemName}��λ�ã�{newPosition}");
                }
            }
            else
            {
                Debug.LogWarning("�Ҳ��� InventorySystem ʵ����");
            }
        }
        else
        {
            Debug.LogWarning("δ���� Item��");
        }

        Continue(); // ����ִ����һ��ָ��
    }


    public override string GetSummary()
    {
        if (item == null)
        {
            return "δ������Ʒ";
        }

        string action = removeInstead ? "�Ƴ�" : "���";

        if (removeInstead)
        {
            return $"{action}��Ʒ��{item.itemName}";
        }
        else
        {
            return $"{action}��Ʒ��{item.itemName}��λ�ã�{newPosition}��";
        }
    }


    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }
}
