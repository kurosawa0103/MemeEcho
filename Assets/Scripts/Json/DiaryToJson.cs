using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DiaryProgressData
{
    public int currentPageId;          // ҳ��ID
    public int currentEntryOrderId;    // ��Ŀ˳��ID
}

public interface IDiaryProgressHandler
{
    void SaveProgress(int pageId, int entryOrderId); // ����ҳ��ID��˳��ID
    int LoadPageId();  // ����ҳ��ID
    int LoadEntryOrderId();  // ����˳��ID
    void ClearProgress(); // �������

}


public class DiaryToJson : IDiaryProgressHandler
{
    private const string jsonFileName = "DiaryProgress.json";
    private DiaryProgressData cachedData;

    private string GetJsonFilePath()
    {
        return Path.Combine(Application.persistentDataPath, jsonFileName);
    }

    public void SaveProgress(int pageId, int entryOrderId)
    {
        cachedData = new DiaryProgressData
        {
            currentPageId = pageId,
            currentEntryOrderId = entryOrderId
        };

        try
        {
            string json = JsonUtility.ToJson(cachedData, true);
            File.WriteAllText(GetJsonFilePath(), json);
            Debug.Log($"�����ռǽ��ȣ�ҳ��ID={pageId}����Ŀ˳��ID={entryOrderId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"�������ʧ�ܣ�{e.Message}");
        }
    }

    private void LoadCachedData()
    {
        if (cachedData != null) return;

        string filePath = GetJsonFilePath();
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            cachedData = JsonUtility.FromJson<DiaryProgressData>(json);
        }
        else
        {
            cachedData = new DiaryProgressData(); // Ĭ�Ͽ�����
        }
    }

    public int LoadPageId()
    {
        LoadCachedData();
        return cachedData.currentPageId;
    }

    public int LoadEntryOrderId()
    {
        LoadCachedData();
        return cachedData.currentEntryOrderId;
    }

    public void ClearProgress()
    {
        cachedData = new DiaryProgressData(); // ���û���

        string filePath = GetJsonFilePath();
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);  // ɾ���ļ�
                Debug.Log("������ռǽ��ȡ�");
            }
            catch (Exception e)
            {
                Debug.LogError($"�������ʧ�ܣ�{e.Message}");
            }
        }
    }

}


