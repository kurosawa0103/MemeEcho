using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [Header("��������")]
    public List<Item> items = new List<Item>();
    private IItemHandler itemHandler = new ItemToJson();
    public int maxSlots = 6;
    public Item selectItem;
    public GameObject[] slots; // ��������
    public Sprite emptySlotSprite; // �ո���ͼƬ

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDesc;


    // ��������ӡ��Ƴ��¼����� item ������
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
            Debug.LogWarning("û���ҵ���Ч�ı����浵��ʹ�ÿ����ݳ�ʼ��");
            data = new InventoryData(); // �����ʼ��һ�������ݣ���ֹ���汨��
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
                slots[i].transform.GetComponent<ThisItemSlot>().thisItem = items[i]; // ����item
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].itemImage; // ����ͼƬ
                slots[i].transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners(); // �Ƴ����¼�
                int index = i; // ����հ�����
            }
            else
            {
                slots[i].transform.GetComponent<ThisItemSlot>().thisItem = null; // ����item
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = emptySlotSprite; // ��ʾ��ͼƬ
                slots[i].transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners(); // ��յ���¼�
            }
        }
    }


    public void AddItemToInventory(Item item)
    {
        AddItemToInventory(item, null); // ���ô���������أ���null��ʾ�����λ��
    }
    public void AddItemToInventory(Item item, Vector3? spawnPositionOverride)
    {

        if (items.Count < maxSlots)
        {
            items.Add(item);
            //SaveInventory();

            DisplayItems();

            Debug.Log($"��Ʒ {item.name} ����ӵ�������");
            OnItemAdded?.Invoke(item);
        }
        else
        {
            Debug.Log("�����������޷��������Ʒ��");
        }
    }


    public void RemoveItemFromInventory(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            //SaveInventory();
            DisplayItems();

            Debug.Log($"��Ʒ {item.name} �Ѵӱ������Ƴ���");
            // �����¼�
            OnItemRemoved?.Invoke(item);
        }
        else
        {
            Debug.Log($"��Ʒ {item.name} ���ڱ����С�");
        }
    }

    public void ClearInventory()
    {
        items.Clear();
        SaveInventory();
        DisplayItems();
        Debug.Log("��������գ�");
    }
}
