using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 可序列化的背包数据类，包含背包、主角和跟随者
[Serializable]
public class InventoryData
{
    public List<string> packItems = new List<string>(); // 背包物品地址列表
}

// 物品数据处理接口
public interface IItemHandler
{
    void SaveInventory(InventoryData data); // 保存整个背包数据
    InventoryData LoadInventory(); // 加载整个背包数据
    void ClearInventory(); // 清除所有背包数据
}

// 实现物品数据保存和加载的类
public class ItemToJson : IItemHandler
{
    private const string jsonFileName = "inventory_data.json"; // JSON 文件名

    // 获取 JSON 文件路径
    private string GetJsonFilePath()
    {
        return Path.Combine(Application.persistentDataPath, jsonFileName); // 获取持久化路径
    }

    // 保存背包数据到 JSON 文件
    public void SaveInventory(InventoryData data)
    {
        string filePath = GetJsonFilePath(); // 获取 JSON 文件路径

        try
        {
            string json = JsonUtility.ToJson(data, true); // 转换为 JSON 字符串
            File.WriteAllText(filePath, json); // 将 JSON 写入文件
            Debug.Log("背包数据已保存至: " + filePath); // 日志记录保存成功
        }
        catch (Exception e)
        {
            Debug.LogError($"保存背包数据时出错: {e.Message}"); // 记录保存失败的错误消息
        }
    }

    // 从 JSON 文件加载背包数据
    public InventoryData LoadInventory()
    {
        string filePath = GetJsonFilePath(); // 获取 JSON 文件路径

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath); // 读取 JSON 文件内容
                var data = JsonUtility.FromJson<InventoryData>(json); // 转换为对象
                if (data == null)
                {
                    Debug.LogWarning("解析 JSON 失败，返回默认空数据。");
                    return new InventoryData();
                }
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"加载背包数据时出错: {e.Message}");
                return new InventoryData(); // 返回默认空数据
            }
        }
        else
        {
            Debug.LogWarning("未找到背包数据文件，返回默认空数据。");
            return new InventoryData(); // 返回默认空数据
        }
    }


    // 清除所有背包数据
    public void ClearInventory()
    {
        string filePath = GetJsonFilePath(); // 获取 JSON 文件路径

        if (File.Exists(filePath))
        {
            try
            {
                // 创建一个空的 InventoryData 实例
                InventoryData emptyData = new InventoryData
                {
                    packItems = new List<string>()
                };

                SaveInventory(emptyData); // 保存空的背包数据
                Debug.Log("背包数据已清除，所有物品已重置。");
            }
            catch (Exception e)
            {
                Debug.LogError($"清除背包数据时出错: {e.Message}"); // 记录删除失败的错误消息
            }
        }
        else
        {
            Debug.LogWarning("未找到背包数据文件，无法清除数据。"); // 日志记录文件未找到
        }
    }
}
