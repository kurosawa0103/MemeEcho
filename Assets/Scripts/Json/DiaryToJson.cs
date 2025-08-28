using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DiaryProgressData
{
    public int currentPageId;          // 页面ID
    public int currentEntryOrderId;    // 条目顺序ID
}

public interface IDiaryProgressHandler
{
    void SaveProgress(int pageId, int entryOrderId); // 保存页面ID和顺序ID
    int LoadPageId();  // 加载页面ID
    int LoadEntryOrderId();  // 加载顺序ID
    void ClearProgress(); // 清除进度

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
            Debug.Log($"保存日记进度：页面ID={pageId}，条目顺序ID={entryOrderId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"保存进度失败：{e.Message}");
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
            cachedData = new DiaryProgressData(); // 默认空数据
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
        cachedData = new DiaryProgressData(); // 重置缓存

        string filePath = GetJsonFilePath();
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);  // 删除文件
                Debug.Log("已清除日记进度。");
            }
            catch (Exception e)
            {
                Debug.LogError($"清除进度失败：{e.Message}");
            }
        }
    }

}


