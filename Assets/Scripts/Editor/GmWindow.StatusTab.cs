#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public partial class GmWindow
{
    // ---------- 字段 ----------
    [TabGroup("页签", "状态数据"), ReadOnly] public EventStateData jsonStateData = new();
    private JsonGenerator jsonHandler = new();
    private string newEventName = "", newEventStatus = "";
    private string modifyTargetEventName = "", modifyTargetEventStatus = "";

    // ---------- 公共方法 ----------
    public void LoadJsonData() => jsonStateData = jsonHandler.LoadEventStateData();

    // ---------- UI ----------
    [TabGroup("页签", "状态数据"), Button("读取 JSON 数据", ButtonSizes.Large)]
    private void BtnLoadJson() => LoadJsonData();

    [TabGroup("页签", "状态数据"), OnInspectorGUI, PropertyOrder(-10)]
    private void DrawJsonList()
    {
        if (jsonStateData.events == null || jsonStateData.events.Count == 0)
        {
            GUILayout.Label("当前没有任何事件状态，请先点击上方按钮读取。", EditorStyles.helpBox);
            return;
        }

        GUILayout.Label("当前 JSON 中的事件状态：", EditorStyles.boldLabel);

        foreach (var evt in jsonStateData.events)
        {
            GUILayout.BeginHorizontal();
            evt.name = EditorGUILayout.TextField(evt.name);
            evt.status = EditorGUILayout.TextField(evt.status);

            if (GUILayout.Button("保存", GUILayout.Width(50)))
                jsonHandler.ModifyEventStatus(evt.name, evt.status);

            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                jsonHandler.ClearSpecificEvent(evt.name);
                LoadJsonData();
                break;
            }
            GUILayout.EndHorizontal();
        }
    }

    [TabGroup("页签", "状态数据"), Title("修改指定事件状态"),  OnInspectorGUI,PropertyOrder(200)]
    private void DrawModifyEntry()
    {
        modifyTargetEventName = EditorGUILayout.TextField("目标事件名", modifyTargetEventName);
        modifyTargetEventStatus = EditorGUILayout.TextField("新的事件状态", modifyTargetEventStatus);

        if (GUILayout.Button("修改该事件状态"))
        {
            if (!string.IsNullOrEmpty(modifyTargetEventName) && !string.IsNullOrEmpty(modifyTargetEventStatus))
            {
                jsonHandler.ModifyEventStatus(modifyTargetEventName, modifyTargetEventStatus);
                Debug.Log($"事件【{modifyTargetEventName}】状态已修改为：{modifyTargetEventStatus}");
                modifyTargetEventName = modifyTargetEventStatus = "";
                LoadJsonData();
            }
            else Debug.LogWarning("事件名和状态不能为空！");
        }
    }

    [TabGroup("页签", "状态数据"), Button("一键清除所有事件状态"), GUIColor(1f, .5f, .5f)]
    private void BtnClearAll()
    {
        jsonHandler.ClearEventStateData();
        LoadJsonData();
    }
}
#endif
