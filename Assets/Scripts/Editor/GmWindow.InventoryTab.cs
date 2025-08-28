#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public partial class GmWindow
{
    // ---------- 字段 ----------
    private const string itemResourcePath = "GameData/ItemData";
    private List<Item> allItems = new List<Item>();

    [TabGroup("页签", "背包系统"), ReadOnly] public InventorySystem inventorySystem;
    [TabGroup("页签", "背包系统"), ReadOnly] public Item selectedItem;

    // ---------- 公共方法 ----------
    public void RefreshItemList()
    {
        allItems.Clear();
        allItems.AddRange(Resources.LoadAll<Item>(itemResourcePath));
        AutoFindInventorySystem();
    }

    // ---------- 内部工具 ----------
    private void AutoFindInventorySystem()
    {
        if (inventorySystem == null)
        {
            inventorySystem = GameObject.FindObjectOfType<InventorySystem>();
            if (inventorySystem != null)
                Debug.Log("[GmWindow] 已自动找到 InventorySystem。");
        }
    }

    // ---------- UI ----------
    [TabGroup("页签", "背包系统"), Button("刷新道具列表", ButtonSizes.Large)]
    private void BtnRefreshItemList() => RefreshItemList();

    [TabGroup("页签", "背包系统"), Button("添加选中道具")]
    private void BtnAddSelected() => inventorySystem?.AddItemToInventory(selectedItem);

    [TabGroup("页签", "背包系统"), Button("移除选中道具")]
    private void BtnRemoveSelected() => inventorySystem?.RemoveItemFromInventory(selectedItem);

    [TabGroup("页签", "背包系统"), Button("一键清空背包"), GUIColor(1f, .5f, .5f)]
    private void BtnClearInventory() => inventorySystem?.ClearInventory();

    [TabGroup("页签", "背包系统"), OnInspectorGUI, PropertyOrder(-10)]
    private void DrawItemButtons()
    {
        GUILayout.Label("道具指令大全", titleStyle); GUILayout.Space(10);

        if (allItems == null || allItems.Count == 0)
        {
            GUILayout.Label("未加载任何道具，请点击上方按钮刷新。", EditorStyles.helpBox);
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
