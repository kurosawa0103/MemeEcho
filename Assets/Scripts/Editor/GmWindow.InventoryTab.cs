#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public partial class GmWindow
{
    // ---------- �ֶ� ----------
    private const string itemResourcePath = "GameData/ItemData";
    private List<Item> allItems = new List<Item>();

    [TabGroup("ҳǩ", "����ϵͳ"), ReadOnly] public InventorySystem inventorySystem;
    [TabGroup("ҳǩ", "����ϵͳ"), ReadOnly] public Item selectedItem;

    // ---------- �������� ----------
    public void RefreshItemList()
    {
        allItems.Clear();
        allItems.AddRange(Resources.LoadAll<Item>(itemResourcePath));
        AutoFindInventorySystem();
    }

    // ---------- �ڲ����� ----------
    private void AutoFindInventorySystem()
    {
        if (inventorySystem == null)
        {
            inventorySystem = GameObject.FindObjectOfType<InventorySystem>();
            if (inventorySystem != null)
                Debug.Log("[GmWindow] ���Զ��ҵ� InventorySystem��");
        }
    }

    // ---------- UI ----------
    [TabGroup("ҳǩ", "����ϵͳ"), Button("ˢ�µ����б�", ButtonSizes.Large)]
    private void BtnRefreshItemList() => RefreshItemList();

    [TabGroup("ҳǩ", "����ϵͳ"), Button("���ѡ�е���")]
    private void BtnAddSelected() => inventorySystem?.AddItemToInventory(selectedItem);

    [TabGroup("ҳǩ", "����ϵͳ"), Button("�Ƴ�ѡ�е���")]
    private void BtnRemoveSelected() => inventorySystem?.RemoveItemFromInventory(selectedItem);

    [TabGroup("ҳǩ", "����ϵͳ"), Button("һ����ձ���"), GUIColor(1f, .5f, .5f)]
    private void BtnClearInventory() => inventorySystem?.ClearInventory();

    [TabGroup("ҳǩ", "����ϵͳ"), OnInspectorGUI, PropertyOrder(-10)]
    private void DrawItemButtons()
    {
        GUILayout.Label("����ָ���ȫ", titleStyle); GUILayout.Space(10);

        if (allItems == null || allItems.Count == 0)
        {
            GUILayout.Label("δ�����κε��ߣ������Ϸ���ťˢ�¡�", EditorStyles.helpBox);
            return;
        }

        int perRow = 4;
        float w = (EditorGUIUtility.currentViewWidth - 10 * 2 - 5 * (perRow - 1)) / perRow;

        int count = 0;
        GUILayout.BeginVertical(); GUILayout.BeginHorizontal();
        foreach (var item in allItems)
        {
            GUI.color = selectedItem == item ? Color.green : Color.white;
            if (GUILayout.Button(item.itemName, GUILayout.Width(w), GUILayout.Height(30)))
                selectedItem = item;

            if (++count % perRow == 0) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); }
        }
        GUILayout.EndHorizontal(); GUILayout.EndVertical();
        GUI.color = Color.white;
    }
}
#endif
