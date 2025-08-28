#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public partial class GmWindow
{
    // ---------- �ֶ� ----------
    [TabGroup("ҳǩ", "״̬����"), ReadOnly] public EventStateData jsonStateData = new();
    private JsonGenerator jsonHandler = new();
    private string newEventName = "", newEventStatus = "";
    private string modifyTargetEventName = "", modifyTargetEventStatus = "";

    // ---------- �������� ----------
    public void LoadJsonData() => jsonStateData = jsonHandler.LoadEventStateData();

    // ---------- UI ----------
    [TabGroup("ҳǩ", "״̬����"), Button("��ȡ JSON ����", ButtonSizes.Large)]
    private void BtnLoadJson() => LoadJsonData();

    [TabGroup("ҳǩ", "״̬����"), OnInspectorGUI, PropertyOrder(-10)]
    private void DrawJsonList()
    {
        if (jsonStateData.events == null || jsonStateData.events.Count == 0)
        {
            GUILayout.Label("��ǰû���κ��¼�״̬�����ȵ���Ϸ���ť��ȡ��", EditorStyles.helpBox);
            return;
        }

        GUILayout.Label("��ǰ JSON �е��¼�״̬��", EditorStyles.boldLabel);

        foreach (var evt in jsonStateData.events)
        {
            GUILayout.BeginHorizontal();
            evt.name = EditorGUILayout.TextField(evt.name);
            evt.status = EditorGUILayout.TextField(evt.status);

            if (GUILayout.Button("����", GUILayout.Width(50)))
                jsonHandler.ModifyEventStatus(evt.name, evt.status);

            if (GUILayout.Button("ɾ��", GUILayout.Width(50)))
            {
                jsonHandler.ClearSpecificEvent(evt.name);
                LoadJsonData();
                break;
            }
            GUILayout.EndHorizontal();
        }
    }

    [TabGroup("ҳǩ", "״̬����"), Title("�޸�ָ���¼�״̬"),  OnInspectorGUI,PropertyOrder(200)]
    private void DrawModifyEntry()
    {
        modifyTargetEventName = EditorGUILayout.TextField("Ŀ���¼���", modifyTargetEventName);
        modifyTargetEventStatus = EditorGUILayout.TextField("�µ��¼�״̬", modifyTargetEventStatus);

        if (GUILayout.Button("�޸ĸ��¼�״̬"))
        {
            if (!string.IsNullOrEmpty(modifyTargetEventName) && !string.IsNullOrEmpty(modifyTargetEventStatus))
            {
                jsonHandler.ModifyEventStatus(modifyTargetEventName, modifyTargetEventStatus);
                Debug.Log($"�¼���{modifyTargetEventName}��״̬���޸�Ϊ��{modifyTargetEventStatus}");
                modifyTargetEventName = modifyTargetEventStatus = "";
                LoadJsonData();
            }
            else Debug.LogWarning("�¼�����״̬����Ϊ�գ�");
        }
    }

    [TabGroup("ҳǩ", "״̬����"), Button("һ����������¼�״̬"), GUIColor(1f, .5f, .5f)]
    private void BtnClearAll()
    {
        jsonHandler.ClearEventStateData();
        LoadJsonData();
    }
}
#endif
