using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [Header("背包配置")]
    public List<Item> items = new List<Item>();
    private IItemHandler itemHandler = new ItemToJson();
    public int maxSlots = 6;
    public Item selectItem;
    public GameObject[] slots; // 背包格子
    public Sprite emptySlotSprite; // 空格子图片

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDesc;


    // 新增：添加、移除事件（带 item 参数）
    public static event Action<Item> OnItemAdded;
    public static event Action<Item> OnItemRemoved;

    void Start()
    {
        LoadInventory();
        DisplayItems();
    }

    public void LoadInventory()
    {
        InventoryData data = itemHandler.LoadInventory();
        if (data == null || data.packItems == null)
        {
            Debug.LogWarning("没有找到有效的背包存档，使用空数据初始化");
            data = new InventoryData(); // 这里初始化一个空数据，防止下面报错
        }
        items.Clear();

        foreach (var address in data.packItems)
        {
            Item item = Resources.Load<Item>(address);
            if (item != null)
            {
                items.Add(item);
            }
        }
    }

    public void SaveInventory()
    {
        InventoryData data = new InventoryData();
        foreach (var item in items)
        {
            data.packItems.Add(item.itemAddress);
        }
        itemHandler.SaveInventory(data);
    }

    public void DisplayItems()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].transform.GetComponent<ThisItemSlot>().thisItem = items[i]; // 更新item
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].itemImage; // 更新图片
                slots[i].transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners(); // 移除旧事件
                int index = i; // 避免闭包问题
            }
            else
            {
                slots[i].transform.GetComponent<ThisItemSlot>().thisItem = null; // 更新item
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = emptySlotSprite; // 显示空图片
                slots[i].transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners(); // 清空点击事件
            }
        }
    }


    public void AddItemToInventory(Item item)
    {
        AddItemToInventory(item, null); // 调用带坐标的重载，传null表示用随机位置
    }
    public void AddItemToInventory(Item item, Vector3? spawnPositionOverride)
    {

        if (items.Count < maxSlots)
        {
            items.Add(item);
            //SaveInventory();

            DisplayItems();

            Debug.Log($"物品 {item.name} 已添加到背包。");
            OnItemAdded?.Invoke(item);
        }
        else
        {
            Debug.Log("背包已满，无法添加新物品。");
        }
    }


    public void RemoveItemFromInventory(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            //SaveInventory();
            DisplayItems();

            Debug.Log($"物品 {item.name} 已从背包中移除。");
            // 触发事件
            OnItemRemoved?.Invoke(item);
        }
        else
        {
            Debug.Log($"物品 {item.name} 不在背包中。");
        }
    }

    public void ClearInventory()
    {
        items.Clear();
        SaveInventory();
        DisplayItems();
        Debug.Log("背包已清空！");
    }
}
