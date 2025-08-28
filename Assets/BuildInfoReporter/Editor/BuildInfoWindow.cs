using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class BuildInfoWindow : EditorWindow
{
    private BuildReportData report;
    private Vector2 scroll;

    // ���һ���������洢 JSON �ļ�·��
    private string jsonFilePath = "BuildReports/LastBuildReport.json";

    [MenuItem("Tools/Build Report Viewer")]
    public static void ShowWindow()
    {
        GetWindow<BuildInfoWindow>("Build Report Viewer");
    }

    private void OnEnable()
    {
        LoadReport();
    }

    private void OnGUI()
    {
        // �������Ϊ�գ���ʾ������Ϣ
        if (report == null)
        {
            EditorGUILayout.HelpBox("No build report found. Load a report file to see the information.", MessageType.Info);
            if (GUILayout.Button("Load Report"))
            {
                LoadReport();
            }
            return;
        }

        GUILayout.Label("Build Summary", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Project", report.projectName);
        EditorGUILayout.LabelField("Platform", report.buildTarget);
        EditorGUILayout.LabelField("Build Time", report.buildTime);
        EditorGUILayout.LabelField("Total Size", report.totalSize);

        GUILayout.Space(10);

        // չʾ Scenes
        GUILayout.Label("Scenes", EditorStyles.boldLabel);
        foreach (var scene in report.scenes)
        {
            EditorGUILayout.LabelField("OK", scene);
        }

        GUILayout.Space(10);

        // չʾ Output Files
        GUILayout.Label("Output Files", EditorStyles.boldLabel);
        foreach (var file in report.outputFiles)
        {
            EditorGUILayout.LabelField(file.size, file.path);
        }

        GUILayout.Space(10);

        // չʾ Assets
        GUILayout.Label("Assets", EditorStyles.boldLabel);
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(300));
        foreach (var asset in report.assets)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(asset.size, GUILayout.Width(70));
            EditorGUILayout.LabelField(asset.type, GUILayout.Width(80));
            EditorGUILayout.LabelField(asset.path);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        // ��ʾ Load �� Refresh ��ť
        if (GUILayout.Button("Load Report"))
        {
            LoadReport();
        }

        if (GUILayout.Button("Refresh Data"))
        {
            RefreshData();
        }
    }

    private void LoadReport()
    {
        // ��� JSON �ļ��Ƿ����
        if (File.Exists(jsonFilePath))
        {
            var json = File.ReadAllText(jsonFilePath);
            report = JsonUtility.FromJson<BuildReportData>(json);
        }
        else
        {
            Debug.LogWarning("No build report found at " + jsonFilePath);
        }
    }

    private void RefreshData()
    {
        // ÿ��ˢ��ʱ���¼��� JSON ����
        LoadReport();
    }
}
