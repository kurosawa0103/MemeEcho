#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
public partial class GmWindow
{
    // ---------- 字段 ----------
    private List<EventData> allEventDatas = new();
    [TabGroup("页签", "事件系统"), ReadOnly] public EventData selectedEvent;

    // ---------- 公共方法 ----------
    public void RefreshEventList() => allEventDatas = Resources.LoadAll<EventData>("GameData/EventData").ToList();

    // ---------- UI ----------
    [TabGroup("页签", "事件系统"), Button("刷新事件列表", ButtonSizes.Large)]
    private void BtnRefreshEvents() => RefreshEventList();

    [TabGroup("页签", "事件系统"), Button("生成选中事件")]
    private void BtnGenerateEvent()
    {
        if (selectedEvent != null && EventManager.Instance != null)
        {
            EventManager.Instance.LoadEvent(selectedEvent);
            Debug.Log($"[GmWindow] 已生成事件：{selectedEvent.name}");
        }
    }

    [TabGroup("页签", "事件系统"), OnInspectorGUI, PropertyOrder(-10)]
    private void DrawEventButtons()
    {
        GUILayout.Label("事件指令大全", titleStyle); GUILayout.Space(10);

        if (allEventDatas.Count == 0)
        {
            GUILayout.Label("未加载任何事件，请点击上方按钮刷新。", EditorStyles.helpBox);
            return;
        }

        int perRow = 4;
        float w = (EditorGUIUtility.currentViewWidth - 10 * 2 - 5 * (perRow - 1)) / perRow;

        int count = 0;
        GUILayout.BeginVertical(); GUILayout.BeginHorizontal();
        foreach (var evt in allEventDatas)
        {
            GUI.color = selectedEvent == evt ? Color.cyan : Color.white;
            if (GUILayout.Button(evt.name, GUILayout.Width(w), GUILayout.Height(30)))
                selectedEvent = evt;

            if (++count % perRow == 0) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); }
        }
        GUILayout.EndHorizontal(); GUILayout.EndVertical();
        GUI.color = Color.white;
    }
}
#endif
