using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LeafType
{
    [Header("叶子设置")]
    public GameObject leafPrefab;
    public string leafName;

    [Header("权重设置")]
    [Range(0f, 100f)]
    public float weight = 25f; // 出现权重（百分比）

    [Header("颜色设置")]
    public Color[] leafColors = {
        new Color(0.3f, 0.7f, 0.2f, 1f),  // 深绿色
        new Color(0.5f, 0.8f, 0.3f, 1f),  // 浅绿色
        new Color(0.2f, 0.6f, 0.1f, 1f)   // 暗绿色
    };

    [Header("缩放设置")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
}

public class LeafSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public int leafCount = 300;
    public Vector2 spawnArea = new Vector2(20f, 15f);

    [Header("叶子类型")]
    public LeafType[] leafTypes = new LeafType[4];

    [Header("分布设置")]
    public bool useGridBasedDistribution = true; // 网格化分布
    public bool addRandomOffset = true; // 添加随机偏移
    public float gridSpacing = 1f; // 网格间距
    public float randomOffset = 0.3f; // 随机偏移量
    public int maxAttemptsPerCell = 3; // 每个网格最大尝试次数

    [Header("图层设置")]
    [SerializeField] private int leafLayer = 8;

    private List<GameObject> spawnedLeaves = new List<GameObject>();
    private List<int> weightedLeafIndices = new List<int>(); // 权重索引列表

    void Start()
    {
        InitializeWeightedList();
        SpawnLeaves();
    }

    void InitializeWeightedList()
    {
        weightedLeafIndices.Clear();

        // 验证权重总和
        float totalWeight = 0f;
        for (int i = 0; i < leafTypes.Length; i++)
        {
            if (leafTypes[i].leafPrefab != null)
            {
                totalWeight += leafTypes[i].weight;
            }
        }

        if (totalWeight <= 0)
        {
            Debug.LogError("叶子权重总和必须大于0！");
            return;
        }

        // 根据权重创建索引列表
        for (int i = 0; i < leafTypes.Length; i++)
        {
            if (leafTypes[i].leafPrefab != null && leafTypes[i].weight > 0)
            {
                // 根据权重比例计算该类型应该添加的索引数量
                int count = Mathf.RoundToInt((leafTypes[i].weight / totalWeight) * 100);
                for (int j = 0; j < count; j++)
                {
                    weightedLeafIndices.Add(i);
                }
            }
        }

        // 如果列表为空，至少添加第一个有效的叶子类型
        if (weightedLeafIndices.Count == 0)
        {
            for (int i = 0; i < leafTypes.Length; i++)
            {
                if (leafTypes[i].leafPrefab != null)
                {
                    weightedLeafIndices.Add(i);
                    break;
                }
            }
        }
    }

    [ContextMenu("生成树叶")]
    public void SpawnLeaves()
    {
        ClearLeaves();
        InitializeWeightedList();

        if (weightedLeafIndices.Count == 0)
        {
            Debug.LogError("没有有效的叶子预制体！");
            return;
        }

        // 统计每种叶子的生成数量
        Dictionary<string, int> leafCounts = new Dictionary<string, int>();

        for (int i = 0; i < leafCount; i++)
        {
            // 根据权重随机选择叶子类型
            int randomIndex = Random.Range(0, weightedLeafIndices.Count);
            int leafTypeIndex = weightedLeafIndices[randomIndex];

            SpawnSingleLeaf(i, leafTypeIndex);

            // 统计数量
            string leafName = leafTypes[leafTypeIndex].leafName;
            if (string.IsNullOrEmpty(leafName))
                leafName = $"LeafType_{leafTypeIndex}";

            if (leafCounts.ContainsKey(leafName))
                leafCounts[leafName]++;
            else
                leafCounts[leafName] = 1;
        }

        // 输出统计信息
        Debug.Log($"成功生成了 {leafCount} 片树叶");
        foreach (var kvp in leafCounts)
        {
            float percentage = (float)kvp.Value / leafCount * 100f;
            Debug.Log($"{kvp.Key}: {kvp.Value} 片 ({percentage:F1}%)");
        }

        // 通知UILeafController刷新叶片列表
        UILeafController controller = FindObjectOfType<UILeafController>();
        if (controller != null)
        {
            controller.RefreshLeaves();
        }
    }

    void SpawnSingleLeaf(int index, int leafTypeIndex)
    {
        LeafType leafType = leafTypes[leafTypeIndex];

        if (leafType.leafPrefab == null)
        {
            Debug.LogError($"叶子类型 {leafTypeIndex} 的预制体为空！");
            return;
        }

        // 随机位置
        Vector3 position = new Vector3(
            Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f),
            Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f),
            0f
        ) + transform.position;

        // 随机旋转
        Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        // 创建树叶
        GameObject leaf = Instantiate(leafType.leafPrefab, position, rotation, transform);
        leaf.name = $"Leaf_{leafTypeIndex}_{index:000}";

        // 设置图层
        leaf.layer = leafLayer;

        // 随机缩放（使用该叶子类型的缩放范围）
        float scale = Random.Range(leafType.minScale, leafType.maxScale);
        leaf.transform.localScale = Vector3.one * scale;

        // 设置颜色（使用该叶子类型的颜色）
        SpriteRenderer sr = leaf.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (leafType.leafColors != null && leafType.leafColors.Length > 0)
            {
                Color leafColor = leafType.leafColors[Random.Range(0, leafType.leafColors.Length)];
                leafColor.a = 1f; // 确保不透明
                sr.color = leafColor;
            }
        }

        // 移除所有碰撞体组件（如果有的话）
        Collider2D[] colliders = leaf.GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            DestroyImmediate(colliders[i]);
        }

        // 确保有Leaf组件
        if (leaf.GetComponent<Leaf>() == null)
        {
            leaf.AddComponent<Leaf>();
        }

        spawnedLeaves.Add(leaf);
    }

    [ContextMenu("清除树叶")]
    public void ClearLeaves()
    {
        foreach (var leaf in spawnedLeaves)
        {
            if (leaf != null)
            {
                DestroyImmediate(leaf);
            }
        }
        spawnedLeaves.Clear();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    [ContextMenu("显示权重分布")]
    public void ShowWeightDistribution()
    {
        float totalWeight = 0f;
        foreach (var leafType in leafTypes)
        {
            if (leafType.leafPrefab != null)
                totalWeight += leafType.weight;
        }

        Debug.Log("=== 权重分布 ===");
        for (int i = 0; i < leafTypes.Length; i++)
        {
            if (leafTypes[i].leafPrefab != null)
            {
                float percentage = (leafTypes[i].weight / totalWeight) * 100f;
                string name = string.IsNullOrEmpty(leafTypes[i].leafName) ? $"LeafType_{i}" : leafTypes[i].leafName;
                Debug.Log($"{name}: {leafTypes[i].weight} ({percentage:F1}%)");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea.x, spawnArea.y, 0));
    }

    void OnValidate()
    {
        // 在Inspector中修改时自动初始化权重列表
        if (Application.isPlaying)
        {
            InitializeWeightedList();
        }
    }
}