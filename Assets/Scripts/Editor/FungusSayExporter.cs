using UnityEngine;
using UnityEditor;
using Fungus;
using System.Text;
using System.IO;

public class FungusSayExporter : EditorWindow
{
    private Flowchart targetFlowchart;
    private string exportPath = "Assets/FungusSayExport.txt";

    [MenuItem("Tools/Fungus/Export Say Texts")]
    public static void ShowWindow()
    {
        GetWindow<FungusSayExporter>("Fungus Say Exporter");
    }

    void OnGUI()
    {
        GUILayout.Label("Fungus Say ָ���ı�����", EditorStyles.boldLabel);

        targetFlowchart = (Flowchart)EditorGUILayout.ObjectField("Ŀ�� Flowchart", targetFlowchart, typeof(Flowchart), true);
        exportPath = EditorGUILayout.TextField("����·��", exportPath);

        if (GUILayout.Button("�����ı�"))
        {
            if (targetFlowchart == null)
            {
                EditorUtility.DisplayDialog("����", "����ָ�� Flowchart��", "OK");
                return;
            }

            ExportSayTexts();
        }
    }

    void ExportSayTexts()
    {
        StringBuilder sb = new StringBuilder();

        foreach (Block block in targetFlowchart.GetComponents<Block>())
        {
            sb.AppendLine($"=== Block: {block.BlockName} ===");

            foreach (Command cmd in block.CommandList)
            {
                if (cmd is Say sayCmd)
                {
                    SerializedObject so = new SerializedObject(sayCmd);

                    // ץ�ı�
                    string text = GetStringProperty(so, "storyText");
                    if (string.IsNullOrEmpty(text))
                        text = GetStringProperty(so, "text");

                    // ץ��ɫ���֣���ͬ�汾�ֶβ�ͬ��
                    string characterName = GetObjectNameProperty(so, "character");
                    if (string.IsNullOrEmpty(characterName))
                        characterName = GetObjectNameProperty(so, "setCharacter");

                    if (string.IsNullOrEmpty(characterName))
                        characterName = "Narrator";

                    sb.AppendLine($"{characterName}: {text}");
                }
            }

            sb.AppendLine();
        }

        File.WriteAllText(exportPath, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("���", $"�ı��ѵ�����: {exportPath}", "OK");
    }

    string GetStringProperty(SerializedObject so, string propertyName)
    {
        SerializedProperty sp = so.FindProperty(propertyName);
        if (sp != null && sp.propertyType == SerializedPropertyType.String)
        {
            return sp.stringValue;
        }
        return null;
    }

    string GetObjectNameProperty(SerializedObject so, string propertyName)
    {
        SerializedProperty sp = so.FindProperty(propertyName);
        if (sp != null && sp.propertyType == SerializedPropertyType.ObjectReference)
        {
            if (sp.objectReferenceValue != null)
                return sp.objectReferenceValue.name;
        }
        return null;
    }
}
