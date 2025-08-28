using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ResourceReferenceFinder : EditorWindow
{
    private Object targetObject; // Ҫ������Ŀ�����
    private Vector2 scrollPosition; // ������ͼ��λ��
    private bool hasFoundReferences = false; // �Ƿ��ҵ����õı�־

    private List<string> referencedScenes = new List<string>(); // �洢������Դ�ĳ���·��
    private List<string> referencedPrefabs = new List<string>(); // �洢������Դ��Ԥ����·��
    private List<string> referencedScriptableObjects = new List<string>(); // �洢������Դ�� ScriptableObject ·��

    [MenuItem("����/��Դ·���������ù���")]
    public static void ShowWindow()
    {
        GetWindow<ResourceReferenceFinder>("��Դ·���������ù���");
    }

    private void OnGUI()
    {
        GUILayout.Label("���ҳ�����Ԥ����� ScriptableObject ���õ���Դ", EditorStyles.boldLabel);

        targetObject = EditorGUILayout.ObjectField("Ŀ�����", targetObject, typeof(Object), false);

        if (GUILayout.Button("��������"))
        {
            FindReferences();
        }

        if (referencedScenes.Count > 0 || referencedPrefabs.Count > 0 || referencedScriptableObjects.Count > 0)
        {
            hasFoundReferences = true; // �ҵ�����
            GUILayout.Label("�ҵ�������:", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            GUILayout.Label("�ڳ�����:");
            foreach (string sceneInfo in referencedScenes)
            {
                GUILayout.Label(sceneInfo);
            }
            GUILayout.Space(5); // ����һЩ���
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // ��ӷָ���
            GUILayout.Space(5); // ����һЩ���

            GUILayout.Label("��Ԥ������:");
            foreach (string prefabInfo in referencedPrefabs)
            {
                GUILayout.Label(prefabInfo);
            }
            GUILayout.Space(5); // ����һЩ���
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // ��ӷָ���
            GUILayout.Space(5); // ����һЩ���

            GUILayout.Label("�� ScriptableObjects ��:");
            foreach (string soInfo in referencedScriptableObjects)
            {
                GUILayout.Label(soInfo);
            }

            GUILayout.EndScrollView();
        }
        else if (hasFoundReferences) // ����Ѿ����ҹ�����û���ҵ�����
        {
            GUILayout.Label("δ�ҵ����á�", EditorStyles.boldLabel);
        }
    }

    private void FindReferences()
    {
        referencedScenes.Clear();
        referencedPrefabs.Clear();
        referencedScriptableObjects.Clear();
        hasFoundReferences = false; // ���ñ�־

        if (targetObject == null)
        {
            Debug.LogWarning("��ѡ��һ��Ŀ�����������ü�����");
            return;
        }

        string targetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(targetObject));

        // ��������
        string[] allScenes = AssetDatabase.FindAssets("t:Scene");
        foreach (string sceneGUID in allScenes)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            if (IsObjectReferencedInAsset(scenePath, targetGUID))
            {
                referencedScenes.Add($"����: {scenePath}");
            }
        }

        // ����Ԥ����
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab");
        foreach (string prefabGUID in allPrefabs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            if (IsObjectReferencedInAsset(prefabPath, targetGUID))
            {
                referencedPrefabs.Add($"Ԥ����: {prefabPath}");
            }
        }

        // ���� ScriptableObject
        string[] allScriptableObjects = AssetDatabase.FindAssets("t:ScriptableObject");
        foreach (string soGUID in allScriptableObjects)
        {
            string soPath = AssetDatabase.GUIDToAssetPath(soGUID);
            if (IsObjectReferencedInAsset(soPath, targetGUID))
            {
                referencedScriptableObjects.Add($"ScriptableObject: {soPath}");
            }
        }

        // ����Ƿ�û���ҵ��κ�����
        if (referencedScenes.Count == 0 && referencedPrefabs.Count == 0 && referencedScriptableObjects.Count == 0)
        {
            hasFoundReferences = true; // �����ҵ����õı�־Ϊ true
        }
    }

    // �����Դ�ڸ������ļ���������Ԥ����� ScriptableObject�����Ƿ�����
    private bool IsObjectReferencedInAsset(string assetPath, string targetGUID)
    {
        string fileContent = File.ReadAllText(assetPath);
        return fileContent.Contains(targetGUID);
    }

    // ���Ʊ߽��ĸ����ߣ��ڱ༭��ģʽ�¿ɼ���
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(20, 10, 20)); // ���Ʊ߽��
    }
}
