using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum ObjectType
{
    Cloud,
    Sea,
    FrontGround,
    Background
}

public class BgCreat : MonoBehaviour
{
    [LabelText("背景生成位置")]
    public Transform objectInstancePos;

    [LabelText("背景预制体")]
    public List<GameObject> objectPool = new List<GameObject>(); // 背景资源列表
    [LabelText("前景预制体")]
    public List<GameObject> frontObjectPool = new List<GameObject>(); // 背景资源列表
    [LabelText("云朵预制体")]
    public List<GameObject> cloudList = new List<GameObject>(); // 云朵资源列表
    [LabelText("海洋预制体")]
    public List<GameObject> seaList = new List<GameObject>(); // 云朵资源列表
    [LabelText("起始点位")]
    public Transform startPoint;

    [LabelText("画面中点")]
    public Transform midPoint;

    [LabelText("销毁点位")]
    public Transform endPoint;

    //public bool isCloudGenerationActive = true;  // 控制云朵生成的开关

    private Dictionary<ObjectType, Queue<GameObject>> poolDictionary = new Dictionary<ObjectType, Queue<GameObject>>();

    [LabelText("替换列表（测试用）")]
    public List<GameObject> replaceList = new List<GameObject>(); // 云朵资源列表 

    void Start()
    {
        // 初始化对象池
        poolDictionary[ObjectType.Cloud] = new Queue<GameObject>();
        poolDictionary[ObjectType.Sea] = new Queue<GameObject>();
        poolDictionary[ObjectType.Background] = new Queue<GameObject>();
        poolDictionary[ObjectType.FrontGround] = new Queue<GameObject>();
        // 初始化并填充云朵和背景池
        InitializeObjectPool(ObjectType.Cloud, cloudList);
        InitializeObjectPool(ObjectType.Sea, cloudList);
        InitializeObjectPool(ObjectType.Background, objectPool);
        InitializeObjectPool(ObjectType.FrontGround, frontObjectPool);
        // 初始创建云朵和背景
        CreateCloud(true);
        CreateSea(true);
        CreateBackground(true);
        CreateFrontground(true);
    }
    private void Update()
    {
        if(Input .GetKeyDown(KeyCode.R))
        {
            ReplaceObjectPoolPrefab(ObjectType.Background, replaceList);
        }
    }
    // 初始化对象池
    private void InitializeObjectPool(ObjectType objectType, List<GameObject> prefabList)
    {
        foreach (var prefab in prefabList)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);  // 禁用对象
            poolDictionary[objectType].Enqueue(obj);  // 将对象放入对象池
        }
    }

    // 替换对象池中的预制体
    public void ReplaceObjectPoolPrefab(ObjectType objectType, List<GameObject> newPrefabs)
    {

        // 获取当前池的队列
        Queue<GameObject> currentPool = poolDictionary[objectType];

        // 清空当前池中的所有对象
        currentPool.Clear();

        // 替换对象池中的预制体列表
        if (objectType == ObjectType.Cloud)
        {
            cloudList = newPrefabs;
        }
        else if (objectType == ObjectType.Background)
        {
            objectPool = newPrefabs;
        }

        MarkObjectsAsOld(objectType); //打上标记
    }

    public void MarkObjectsAsOld(ObjectType? objectType = null)
    {
        var bgScrollObjects = FindObjectsOfType<BgScroll>();

        foreach (var bgScroll in bgScrollObjects)
        {
            // 如果没传 objectType，或当前物体的类型和传入类型一致，就标记
            if (objectType == null || bgScroll.objectType == objectType.Value)
            {
                bgScroll.isReplaced = true;
            }
        }

        if (objectType == null)
        {
            Debug.Log("已标记所有 BgScroll 为旧物体");
        }
        else
        {
            Debug.Log($"已标记 {objectType.Value} 类型的 BgScroll 为旧物体");
        }
    }


    // 创建背景或云朵
    public void CreateObject(ObjectType objectType)
    {
        if (objectType == ObjectType.Cloud)
        {
            // 创建云朵
            CreateCloud();
        }
        else if (objectType == ObjectType.Background)
        {
            // 创建背景
            CreateBackground();
        }
        else if (objectType == ObjectType.FrontGround)
        {
            // 创建背景
            CreateFrontground();
        }
        else if (objectType == ObjectType.Sea)
        {
            // 创建海洋
            CreateSea();
        }
    }

    private void CreateCloud(bool isFirstTime = false)
    {
        GameObject cloudObj = GetObjectFromPool(ObjectType.Cloud);  // 从对象池获取云朵物体
        if (cloudObj != null)
        {
            // 首次生成放在 midPoint，其余放在 startPoint
            cloudObj.transform.position = isFirstTime ? midPoint.position : startPoint.position;
            cloudObj.transform.rotation = isFirstTime ? midPoint.rotation : startPoint.rotation;
            cloudObj.SetActive(true);  // 激活物体
            cloudObj.transform.parent = objectInstancePos;
        }
    }
    private void CreateBackground(bool isFirstTime = false)
    {
        GameObject backgroundObj = GetObjectFromPool(ObjectType.Background);
        if (backgroundObj != null)
        {
            backgroundObj.transform.position = isFirstTime ? midPoint.position : startPoint.position;
            backgroundObj.transform.rotation = isFirstTime ? midPoint.rotation : startPoint.rotation;
            backgroundObj.SetActive(true);
            backgroundObj.transform.parent = objectInstancePos;
        }
    }
    private void CreateFrontground(bool isFirstTime = false)
    {
        GameObject frontgroundObj = GetObjectFromPool(ObjectType.FrontGround);
        if (frontgroundObj != null)
        {
            frontgroundObj.transform.position = isFirstTime ? midPoint.position : startPoint.position;
            frontgroundObj.transform.rotation = isFirstTime ? midPoint.rotation : startPoint.rotation;
            frontgroundObj.SetActive(true);
            frontgroundObj.transform.parent = objectInstancePos;
        }
    }
    private void CreateSea(bool isFirstTime = false)
    {
        GameObject seaObj = GetObjectFromPool(ObjectType.Sea);
        if (seaObj != null)
        {
            seaObj.transform.position = isFirstTime ? midPoint.position : startPoint.position;
            seaObj.transform.rotation = isFirstTime ? midPoint.rotation : startPoint.rotation;
            seaObj.SetActive(true);
            seaObj.transform.parent = objectInstancePos;
        }
    }
    // 从对象池获取物体
    private GameObject GetObjectFromPool(ObjectType objectType)
    {
        GameObject obj = null;

        if (poolDictionary[objectType].Count > 0)
        {
            obj = poolDictionary[objectType].Dequeue();  // 从队列中取出一个物体
        }
        else
        {
            // 如果池里没有对象，实例化一个新的
            obj = CreateNewObject(objectType);
        }

        return obj;
    }

    // 创建新的物体
    private GameObject CreateNewObject(ObjectType objectType)
    {
        GameObject obj = null;

        if (objectType == ObjectType.Cloud&&cloudList.Count !=0)
        {
            int randomIndex = Random.Range(0, cloudList.Count);
            obj = Instantiate(cloudList[randomIndex]);
        }
        else if(objectType == ObjectType.Background && objectPool.Count != 0)
        {
            int randomIndex = Random.Range(0, objectPool.Count);
            obj = Instantiate(objectPool[randomIndex]);
        }
        else if (objectType == ObjectType.FrontGround && frontObjectPool.Count != 0)
        {
            int randomIndex = Random.Range(0, frontObjectPool.Count);
            obj = Instantiate(frontObjectPool[randomIndex]);
        }
        else if (objectType == ObjectType.Sea && seaList.Count != 0)
        {
            int randomIndex = Random.Range(0, seaList.Count);
            obj = Instantiate(seaList[randomIndex]);
        }
        return obj;
    }

    // 将物体返回对象池或销毁
    public void ReturnObjectToPool(ObjectType objectType, GameObject obj, bool isReplaced)
    {
        // 如果物体标记为已替换，销毁它
        if (isReplaced)
        {
            Destroy(obj);  // 销毁物体
        }
        else
        {
            obj.SetActive(false);  // 禁用物体
            poolDictionary[objectType].Enqueue(obj);  // 放回池中
        }
    }

    public void ClearAllPrefabLists()
    {
        cloudList.Clear();
        //seaList.Clear();
        objectPool.Clear();
        frontObjectPool.Clear();
    }
}
