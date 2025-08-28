#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
public partial class GmWindow
{
    // ---------- �ֶ� ----------
    private List<EventData> allEventDatas = new();
    [TabGroup("ҳǩ", "�¼�ϵͳ"), ReadOnly] public EventData selectedEvent;

    // ---------- �������� ----------
    public void RefreshEventList() => allEventDatas = Resources.LoadAll<EventData>("GameData/EventData").ToList();

    // ---------- UI ----------
    [TabGroup("ҳǩ", "�¼�ϵͳ"), Button("ˢ���¼��б�", ButtonSizes.Large)]
    private void BtnRefreshEvents() => RefreshEventList();

    [TabGroup("ҳǩ", "�¼�ϵͳ"), Button("����ѡ���¼�")]
    private void BtnGenerateEvent()
    {
        if (selectedEvent != null && EventManager.Instance != null)
        {
            EventManager.Instance.LoadEvent(selectedEvent);
            Debug.Log($"[GmWindow] �������¼���{selectedEvent.name}");
        }
    }

    [TabGroup("ҳǩ", "�¼�ϵͳ"), OnInspectorGUI, PropertyOrder(-10)]
    private void DrawEventButtons()
    {
        GUILayout.Label("�¼�ָ���ȫ", titleStyle); GUILayout.Space(10);

        if (allEventDatas.Count == 0)
        {
            GUILayout.Label("δ�����κ��¼��������Ϸ���ťˢ�¡�", EditorStyles.helpBox);
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
