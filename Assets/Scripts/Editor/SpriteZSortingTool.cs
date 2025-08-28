using UnityEngine;
using UnityEditor;
using System.Linq;

public class SpriteZSortingTool : EditorWindow
{
    private GameObject rootObject;
    private ZSegmentRuleConfig config;

    private Vector2 scrollPos;

    [MenuItem("Tools/Sprite 分段 Z 排序工具")]
    static void Init()
    {
        var window = (SpriteZSortingTool)GetWindow(typeof(SpriteZSortingTool));
        window.titleContent = new GUIContent("分段 Z 排序");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("SpriteRenderer 分段 + 段内排序 (ScriptableObject)", EditorStyles.boldLabel);

        rootObject = (GameObject)EditorGUILayout.ObjectField("父物体", rootObject, typeof(GameObject), true);
        config = (ZSegmentRuleConfig)EditorGUILayout.ObjectField("配置文件 (SO)", config, typeof(ZSegmentRuleConfig), false);

        if (config == null)
        {
            EditorGUILayout.HelpBox("请先创建并分配一个 ZSegmentRuleConfig 配置文件", MessageType.Warning);
            if (GUILayout.Button("新建配置文件"))
            {
                string path = EditorUtility.SaveFilePanelInProject("保存配置文件", "ZSegmentRuleConfig.asset", "asset", "请选择保存路径");
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
            GUILayout.Label($"分段 {i + 1}", EditorStyles.boldLabel);

            config.rules[i].minZ = EditorGUILayout.FloatField("最小 Z", config.rules[i].minZ);
            config.rules[i].maxZ = EditorGUILayout.FloatField("最大 Z", config.rules[i].maxZ);
            config.rules[i].sortingLayer = EditorGUILayout.TextField("Sorting Layer", config.rules[i].sortingLayer);

            if (GUILayout.Button("删除此分段"))
            {
                config.rules.RemoveAt(i);
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                break;
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("添加新分段"))
        {
            config.rules.Add(new ZSegmentRuleConfig.ZSegmentRule());
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("应用排序"))
        {
            ApplySegmentedSorting();
        }
    }

    void ApplySegmentedSorting()
    {
        if (rootObject == null)
        {
            Debug.LogWarning("请指定父物体");
            return;
        }

        if (config == null || config.rules.Count == 0)
        {
            Debug.LogWarning("请先设置分段规则");
            return;
        }

        int totalSorted = 0;

        var allRenderers = rootObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in allRenderers)
        {
            Undo.RecordObject(sr, "重置排序");
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
                Undo.RecordObject(sr, "修改 SpriteRenderer 排序");
                sr.sortingLayerName = rule.sortingLayer;
                sr.sortingOrder = i;
                Debug.Log($"排序：{sr.gameObject.name} Z={sr.transform.position.z} Layer={rule.sortingLayer} Order={i}");
                EditorUtility.SetDirty(sr);
                totalSorted++;
            }
        }

        // 标记场景已修改，保证保存
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        Debug.Log($"已完成排序，总共处理 {totalSorted} 个 SpriteRenderer");
    }
}
