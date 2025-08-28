using UnityEngine;
using UnityEditor;
using System.Linq;

public class SpriteZSortingTool : EditorWindow
{
    private GameObject rootObject;
    private ZSegmentRuleConfig config;

    private Vector2 scrollPos;

    [MenuItem("Tools/Sprite �ֶ� Z ���򹤾�")]
    static void Init()
    {
        var window = (SpriteZSortingTool)GetWindow(typeof(SpriteZSortingTool));
        window.titleContent = new GUIContent("�ֶ� Z ����");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("SpriteRenderer �ֶ� + �������� (ScriptableObject)", EditorStyles.boldLabel);

        rootObject = (GameObject)EditorGUILayout.ObjectField("������", rootObject, typeof(GameObject), true);
        config = (ZSegmentRuleConfig)EditorGUILayout.ObjectField("�����ļ� (SO)", config, typeof(ZSegmentRuleConfig), false);

        if (config == null)
        {
            EditorGUILayout.HelpBox("���ȴ���������һ�� ZSegmentRuleConfig �����ļ�", MessageType.Warning);
            if (GUILayout.Button("�½������ļ�"))
            {
                string path = EditorUtility.SaveFilePanelInProject("���������ļ�", "ZSegmentRuleConfig.asset", "asset", "��ѡ�񱣴�·��");
                if (!string.IsNullOrEmpty(path))
                {
                    var newConfig = ScriptableObject.CreateInstance<ZSegmentRuleConfig>();
                    AssetDatabase.CreateAsset(newConfig, path);
                    AssetDatabase.SaveAssets();
                    config = newConfig;
                }
            }
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < config.rules.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label($"�ֶ� {i + 1}", EditorStyles.boldLabel);

            config.rules[i].minZ = EditorGUILayout.FloatField("��С Z", config.rules[i].minZ);
            config.rules[i].maxZ = EditorGUILayout.FloatField("��� Z", config.rules[i].maxZ);
            config.rules[i].sortingLayer = EditorGUILayout.TextField("Sorting Layer", config.rules[i].sortingLayer);

            if (GUILayout.Button("ɾ���˷ֶ�"))
            {
                config.rules.RemoveAt(i);
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                break;
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("����·ֶ�"))
        {
            config.rules.Add(new ZSegmentRuleConfig.ZSegmentRule());
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Ӧ������"))
        {
            ApplySegmentedSorting();
        }
    }

    void ApplySegmentedSorting()
    {
        if (rootObject == null)
        {
            Debug.LogWarning("��ָ��������");
            return;
        }

        if (config == null || config.rules.Count == 0)
        {
            Debug.LogWarning("�������÷ֶι���");
            return;
        }

        int totalSorted = 0;

        var allRenderers = rootObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in allRenderers)
        {
            Undo.RecordObject(sr, "��������");
            sr.sortingLayerName = "";
            sr.sortingOrder = 0;
            EditorUtility.SetDirty(sr);
        }

        foreach (var rule in config.rules)
        {
            var segmentSprites = allRenderers
                .Where(sr =>
                {
                    float z = sr.transform.position.z;
                    return z >= rule.minZ && z <= rule.maxZ;
                })
                .OrderByDescending(sr => sr.transform.position.z)
                .ToList();

            for (int i = 0; i < segmentSprites.Count; i++)
            {
                var sr = segmentSprites[i];
                Undo.RecordObject(sr, "�޸� SpriteRenderer ����");
                sr.sortingLayerName = rule.sortingLayer;
                sr.sortingOrder = i;
                Debug.Log($"����{sr.gameObject.name} Z={sr.transform.position.z} Layer={rule.sortingLayer} Order={i}");
                EditorUtility.SetDirty(sr);
                totalSorted++;
            }
        }

        // ��ǳ������޸ģ���֤����
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        Debug.Log($"����������ܹ����� {totalSorted} �� SpriteRenderer");
    }
}
