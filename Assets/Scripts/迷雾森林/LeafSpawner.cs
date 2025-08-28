using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LeafType
{
    [Header("Ҷ������")]
    public GameObject leafPrefab;
    public string leafName;

    [Header("Ȩ������")]
    [Range(0f, 100f)]
    public float weight = 25f; // ����Ȩ�أ��ٷֱȣ�

    [Header("��ɫ����")]
    public Color[] leafColors = {
        new Color(0.3f, 0.7f, 0.2f, 1f),  // ����ɫ
        new Color(0.5f, 0.8f, 0.3f, 1f),  // ǳ��ɫ
        new Color(0.2f, 0.6f, 0.1f, 1f)   // ����ɫ
    };

    [Header("��������")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
}

public class LeafSpawner : MonoBehaviour
{
    [Header("��������")]
    public int leafCount = 300;
    public Vector2 spawnArea = new Vector2(20f, 15f);

    [Header("Ҷ������")]
    public LeafType[] leafTypes = new LeafType[4];

    [Header("�ֲ�����")]
    public bool useGridBasedDistribution = true; // ���񻯷ֲ�
    public bool addRandomOffset = true; // ������ƫ��
    public float gridSpacing = 1f; // ������
    public float randomOffset = 0.3f; // ���ƫ����
    public int maxAttemptsPerCell = 3; // ÿ����������Դ���

    [Header("ͼ������")]
    [SerializeField] private int leafLayer = 8;

    private List<GameObject> spawnedLeaves = new List<GameObject>();
    private List<int> weightedLeafIndices = new List<int>(); // Ȩ�������б�

    void Start()
    {
        InitializeWeightedList();
        SpawnLeaves();
    }

    void InitializeWeightedList()
    {
        weightedLeafIndices.Clear();

        // ��֤Ȩ���ܺ�
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
            Debug.LogError("Ҷ��Ȩ���ܺͱ������0��");
            return;
        }

        // ����Ȩ�ش��������б�
        for (int i = 0; i < leafTypes.Length; i++)
        {
            if (leafTypes[i].leafPrefab != null && leafTypes[i].weight > 0)
            {
                // ����Ȩ�ر������������Ӧ����ӵ���������
                int count = Mathf.RoundToInt((leafTypes[i].weight / totalWeight) * 100);
                for (int j = 0; j < count; j++)
                {
                    weightedLeafIndices.Add(i);
                }
            }
        }

        // ����б�Ϊ�գ�������ӵ�һ����Ч��Ҷ������
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

    [ContextMenu("������Ҷ")]
    public void SpawnLeaves()
    {
        ClearLeaves();
        InitializeWeightedList();

        if (weightedLeafIndices.Count == 0)
        {
            Debug.LogError("û����Ч��Ҷ��Ԥ���壡");
            return;
        }

        // ͳ��ÿ��Ҷ�ӵ���������
        Dictionary<string, int> leafCounts = new Dictionary<string, int>();

        for (int i = 0; i < leafCount; i++)
        {
            // ����Ȩ�����ѡ��Ҷ������
            int randomIndex = Random.Range(0, weightedLeafIndices.Count);
            int leafTypeIndex = weightedLeafIndices[randomIndex];

            SpawnSingleLeaf(i, leafTypeIndex);

            // ͳ������
            string leafName = leafTypes[leafTypeIndex].leafName;
            if (string.IsNullOrEmpty(leafName))
                leafName = $"LeafType_{leafTypeIndex}";

            if (leafCounts.ContainsKey(leafName))
                leafCounts[leafName]++;
            else
                leafCounts[leafName] = 1;
        }

        // ���ͳ����Ϣ
        Debug.Log($"�ɹ������� {leafCount} Ƭ��Ҷ");
        foreach (var kvp in leafCounts)
        {
            float percentage = (float)kvp.Value / leafCount * 100f;
            Debug.Log($"{kvp.Key}: {kvp.Value} Ƭ ({percentage:F1}%)");
        }

        // ֪ͨUILeafControllerˢ��ҶƬ�б�
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
            Debug.LogError($"Ҷ������ {leafTypeIndex} ��Ԥ����Ϊ�գ�");
            return;
        }

        // ���λ��
        Vector3 position = new Vector3(
            Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f),
            Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f),
            0f
        ) + transform.position;

        // �����ת
        Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        // ������Ҷ
        GameObject leaf = Instantiate(leafType.leafPrefab, position, rotation, transform);
        leaf.name = $"Leaf_{leafTypeIndex}_{index:000}";

        // ����ͼ��
        leaf.layer = leafLayer;

        // ������ţ�ʹ�ø�Ҷ�����͵����ŷ�Χ��
        float scale = Random.Range(leafType.minScale, leafType.maxScale);
        leaf.transform.localScale = Vector3.one * scale;

        // ������ɫ��ʹ�ø�Ҷ�����͵���ɫ��
        SpriteRenderer sr = leaf.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (leafType.leafColors != null && leafType.leafColors.Length > 0)
            {
                Color leafColor = leafType.leafColors[Random.Range(0, leafType.leafColors.Length)];
                leafColor.a = 1f; // ȷ����͸��
                sr.color = leafColor;
            }
        }

        // �Ƴ�������ײ�����������еĻ���
        Collider2D[] colliders = leaf.GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            DestroyImmediate(colliders[i]);
        }

        // ȷ����Leaf���
        if (leaf.GetComponent<Leaf>() == null)
        {
            leaf.AddComponent<Leaf>();
        }

        spawnedLeaves.Add(leaf);
    }

    [ContextMenu("�����Ҷ")]
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

    [ContextMenu("��ʾȨ�طֲ�")]
    public void ShowWeightDistribution()
    {
        float totalWeight = 0f;
        foreach (var leafType in leafTypes)
        {
            if (leafType.leafPrefab != null)
                totalWeight += leafType.weight;
        }

        Debug.Log("=== Ȩ�طֲ� ===");
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
        // ��Inspector���޸�ʱ�Զ���ʼ��Ȩ���б�
        if (Application.isPlaying)
        {
            InitializeWeightedList();
        }
    }
}