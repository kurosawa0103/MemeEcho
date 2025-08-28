using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface IPhotoHandler
{
    void SavePhotos(InventoryData data);  // ������Ƭ����
    InventoryData LoadPhotos();           // ������Ƭ����
    void ClearPhotos();                   // ���������Ƭ����
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
            Debug.Log("��Ƭ�����ѱ�����: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"������Ƭ����ʱ����: {e.Message}");
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
                Debug.LogError($"������Ƭ����ʱ����: {e.Message}");
                return new InventoryData();
            }
        }
        else
        {
            Debug.LogWarning("δ�ҵ���Ƭ�����ļ�������Ĭ�Ͽ����ݡ�");
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
                Debug.Log("��Ƭ�����������");
            }
            catch (Exception e)
            {
                Debug.LogError($"�����Ƭ����ʱ����: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("δ�ҵ���Ƭ�����ļ����޷������");
        }
    }
}