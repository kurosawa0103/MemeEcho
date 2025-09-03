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
        GUILayout.Label("Fungus Say 指令文本导出", EditorStyles.boldLabel);

        targetFlowchart = (Flowchart)EditorGUILayout.ObjectField("目标 Flowchart", targetFlowchart, typeof(Flowchart), true);
        exportPath = EditorGUILayout.TextField("导出路径", exportPath);

        if (GUILayout.Button("导出文本"))
        {
            if (targetFlowchart == null)
            {
                EditorUtility.DisplayDialog("错误", "请先指定 Flowchart！", "OK");
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

                    // 抓文本
                    string text = GetStringProperty(so, "storyText");
                    if (string.IsNullOrEmpty(text))
                        text = GetStringProperty(so, "text");

                    // 抓角色名字（不同版本字段不同）
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

        EditorUtility.DisplayDialog("完成", $"文本已导出到: {exportPath}", "OK");
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
