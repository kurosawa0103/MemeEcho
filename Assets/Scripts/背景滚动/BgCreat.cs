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
    [LabelText("��������λ��")]
    public Transform objectInstancePos;

    [LabelText("����Ԥ����")]
    public List<GameObject> objectPool = new List<GameObject>(); // ������Դ�б�
    [LabelText("ǰ��Ԥ����")]
    public List<GameObject> frontObjectPool = new List<GameObject>(); // ������Դ�б�
    [LabelText("�ƶ�Ԥ����")]
    public List<GameObject> cloudList = new List<GameObject>(); // �ƶ���Դ�б�
    [LabelText("����Ԥ����")]
    public List<GameObject> seaList = new List<GameObject>(); // �ƶ���Դ�б�
    [LabelText("��ʼ��λ")]
    public Transform startPoint;

    [LabelText("�����е�")]
    public Transform midPoint;

    [LabelText("���ٵ�λ")]
    public Transform endPoint;

    //public bool isCloudGenerationActive = true;  // �����ƶ����ɵĿ���

    private Dictionary<ObjectType, Queue<GameObject>> poolDictionary = new Dictionary<ObjectType, Queue<GameObject>>();

    [LabelText("�滻�б������ã�")]
    public List<GameObject> replaceList = new List<GameObject>(); // �ƶ���Դ�б� 

    void Start()
    {
        // ��ʼ�������
        poolDictionary[ObjectType.Cloud] = new Queue<GameObject>();
        poolDictionary[ObjectType.Sea] = new Queue<GameObject>();
        poolDictionary[ObjectType.Background] = new Queue<GameObject>();
        poolDictionary[ObjectType.FrontGround] = new Queue<GameObject>();
        // ��ʼ��������ƶ�ͱ�����
        InitializeObjectPool(ObjectType.Cloud, cloudList);
        InitializeObjectPool(ObjectType.Sea, cloudList);
        InitializeObjectPool(ObjectType.Background, objectPool);
        InitializeObjectPool(ObjectType.FrontGround, frontObjectPool);
        // ��ʼ�����ƶ�ͱ���
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
    // ��ʼ�������
    private void InitializeObjectPool(ObjectType objectType, List<GameObject> prefabList)
    {
        foreach (var prefab in prefabList)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);  // ���ö���
            poolDictionary[objectType].Enqueue(obj);  // �������������
        }
    }

    // �滻������е�Ԥ����
    public void ReplaceObjectPoolPrefab(ObjectType objectType, List<GameObject> newPrefabs)
    {

        // ��ȡ��ǰ�صĶ���
        Queue<GameObject> currentPool = poolDictionary[objectType];

        // ��յ�ǰ���е����ж���
        currentPool.Clear();

        // �滻������е�Ԥ�����б�
        if (objectType == ObjectType.Cloud)
        {
            cloudList = newPrefabs;
        }
        else if (objectType == ObjectType.Background)
        {
            objectPool = newPrefabs;
        }

        MarkObjectsAsOld(objectType); //���ϱ��
    }

    public void MarkObjectsAsOld(ObjectType? objectType = null)
    {
        var bgScrollObjects = FindObjectsOfType<BgScroll>();

        foreach (var bgScroll in bgScrollObjects)
        {
            // ���û�� objectType����ǰ��������ͺʹ�������һ�£��ͱ��
            if (objectType == null || bgScroll.objectType == objectType.Value)
            {
                bgScroll.isReplaced = true;
            }
        }

        if (objectType == null)
        {
            Debug.Log("�ѱ������ BgScroll Ϊ������");
        }
        else
        {
            Debug.Log($"�ѱ�� {objectType.Value} ���͵� BgScroll Ϊ������");
        }
    }


    // �����������ƶ�
    public void CreateObject(ObjectType objectType)
    {
        if (objectType == ObjectType.Cloud)
        {
            // �����ƶ�
            CreateCloud();
        }
        else if (objectType == ObjectType.Background)
        {
            // ��������
            CreateBackground();
        }
        else if (objectType == ObjectType.FrontGround)
        {
            // ��������
            CreateFrontground();
        }
        else if (objectType == ObjectType.Sea)
        {
            // ��������
            CreateSea();
        }
    }

    private void CreateCloud(bool isFirstTime = false)
    {
        GameObject cloudObj = GetObjectFromPool(ObjectType.Cloud);  // �Ӷ���ػ�ȡ�ƶ�����
        if (cloudObj != null)
        {
            // �״����ɷ��� midPoint��������� startPoint
            cloudObj.transform.position = isFirstTime ? midPoint.position : startPoint.position;
            cloudObj.transform.rotation = isFirstTime ? midPoint.rotation : startPoint.rotation;
            cloudObj.SetActive(true);  // ��������
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
    // �Ӷ���ػ�ȡ����
    private GameObject GetObjectFromPool(ObjectType objectType)
    {
        GameObject obj = null;

        if (poolDictionary[objectType].Count > 0)
        {
            obj = poolDictionary[objectType].Dequeue();  // �Ӷ�����ȡ��һ������
        }
        else
        {
            // �������û�ж���ʵ����һ���µ�
            obj = CreateNewObject(objectType);
        }

        return obj;
    }

    // �����µ�����
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

    // �����巵�ض���ػ�����
    public void ReturnObjectToPool(ObjectType objectType, GameObject obj, bool isReplaced)
    {
        // ���������Ϊ���滻��������
        if (isReplaced)
        {
            Destroy(obj);  // ��������
        }
        else
        {
            obj.SetActive(false);  // ��������
            poolDictionary[objectType].Enqueue(obj);  // �Żس���
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
