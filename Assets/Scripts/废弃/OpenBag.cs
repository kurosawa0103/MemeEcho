using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBag : MonoBehaviour
{
    public bool bagIsOn = false;
    public GameObject bagPanel;
    private InventorySystem inventorySystem;
    void Start()
    {
        inventorySystem = FindObjectOfType<InventorySystem>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenBagPanel()
    {
        if(!bagIsOn)
        {
            bagIsOn = true;
            bagPanel.SetActive(true);
            inventorySystem.DisplayItems();
        }
        else if(bagIsOn)
        {
            bagIsOn = false;
            bagPanel.SetActive(false);
        }

    }
}
