using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface IPhotoHandler
{
    void SavePhotos(InventoryData data);  // 保存照片数据
    InventoryData LoadPhotos();           // 加载照片数据
    void ClearPhotos();                   // 清除所有照片数据
}

public class PhotoToJson : IPhotoHandler
{
    private const string jsonFileName = "photo_inventory.json";

    private string GetJsonFilePath()
    {
        return Path.Combine(Application.persistentDataPath, jsonFileName);
    }

    public void SavePhotos(InventoryData data)
    {
        string filePath = GetJsonFilePath();
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
            Debug.Log("照片数据已保存至: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"保存照片数据时出错: {e.Message}");
        }
    }

    public InventoryData LoadPhotos()
    {
        string filePath = GetJsonFilePath();
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<InventoryData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"加载照片数据时出错: {e.Message}");
                return new InventoryData();
            }
        }
        else
        {
            Debug.LogWarning("未找到照片数据文件，返回默认空数据。");
            return new InventoryData();
        }
    }

    public void ClearPhotos()
    {
        string filePath = GetJsonFilePath();
        if (File.Exists(filePath))
        {
            try
            {
                InventoryData emptyData = new InventoryData();
                SavePhotos(emptyData);
                Debug.Log("照片数据已清除！");
            }
            catch (Exception e)
            {
                Debug.LogError($"清除照片数据时出错: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("未找到照片数据文件，无法清除。");
        }
    }
}