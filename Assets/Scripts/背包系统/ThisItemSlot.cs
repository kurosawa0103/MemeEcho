using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThisItemSlot : MonoBehaviour
{
    public Item thisItem;
    private InventorySystem inventorySystem;
    public TextTyper nameTyper;
    public TextTyper descTyper;
    void Start()
    {
        inventorySystem = FindObjectOfType<InventorySystem>();
    }

    public void SelectItem()
    {
        
        if(thisItem!=null)
        {
            inventorySystem.selectItem = thisItem;
            inventorySystem.itemName.text = thisItem.itemName;
            inventorySystem.itemDesc.text = thisItem.itemDesc;
            if (nameTyper != null)
                nameTyper.ShowText(thisItem.itemName);

            if (descTyper != null)
                descTyper.ShowText(thisItem.itemDesc);
        }

    }
}
