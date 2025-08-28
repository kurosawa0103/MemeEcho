using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class EventSystemData
{
    public string name;
    public string status;
}

[Serializable]
public class EventStateData
{
    public List<EventSystemData> events = new List<EventSystemData>();
}

public interface IEventStateDataHandler
{
    EventStateData LoadEventStateData();
    void SaveEventStateData(EventStateData eventStateData);
    void ModifyEventStatus(string eventName, string newStatus);
    void ClearEventStateData(); // 清除所有事件状态
    void ClearSpecificEvent(string eventName); // 清除指定事件
    string GetEventStatus(string eventName); // 获取指定事件状态
}

public class JsonGenerator : IEventStateDataHandler
{
    private const string jsonFileName = "event_state_data.json";
    // 事件状态变更时触发的事件
    public static event Action<string, string> OnEventStatusChanged;
    private string GetJsonFilePath()
    {
        return Path.Combine(Application.persistentDataPath, jsonFileName);
    }

    // 加载事件状态数据
    public EventStateData LoadEventStateData()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<EventStateData>(json);
        }
        else
        {
            Debug.LogWarning("Event state data file not found. Creating new event state data.");
            return new EventStateData();
        }
    }

    // 检查文件是否存在，不存在则创建
    private void CheckAndCreateEventStateFile()
    {
        string filePath = GetJsonFilePath();

        if (!File.Exists(filePath))
        {
            EventStateData initialData = new EventStateData();
            SaveEventStateData(initialData);
        }
    }

    // 保存事件状态数据到 JSON 文件
    public void SaveEventStateData(EventStateData eventStateData)
    {
        string filePath = GetJsonFilePath();

        try
        {
            string json = JsonUtility.ToJson(eventStateData, true);
            File.WriteAllText(filePath, json);
            Debug.Log("事件状态数据已保存至: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving event state data: {e.Message}");
        }
    }

    // 修改指定事件的状态
    public void ModifyEventStatus(string eventName, string newStatus)
    {
        if (string.IsNullOrEmpty(eventName) || string.IsNullOrEmpty(newStatus))
        {
            Debug.LogWarning("Invalid event data. Event name and status must be provided.");
            return;
        }

        EventStateData eventStateData = LoadEventStateData();
        EventSystemData eventData = eventStateData.events.Find(x => x.name == eventName);

        if (eventData != null)
        {
            eventData.status = newStatus;
        }
        else
        {
            eventStateData.events.Add(new EventSystemData { name = eventName, status = newStatus });
        }

        SaveEventStateData(eventStateData);
        // 触发事件状态改变的事件通知所有订阅者
        OnEventStatusChanged?.Invoke(eventName, newStatus);
    }

    // 清除所有事件数据
    public void ClearEventStateData()
    {
        EventStateData eventStateData = new EventStateData();
        SaveEventStateData(eventStateData);
        Debug.Log("所有事件状态数据已清除");
        // 触发事件状态改变的事件通知所有订阅者
    }

    // 清除指定事件
    public void ClearSpecificEvent(string eventName)
    {
        EventStateData eventStateData = LoadEventStateData();

        if (eventStateData.events == null)
        {
            eventStateData.events = new List<EventSystemData>();
        }

        EventSystemData eventToRemove = eventStateData.events.Find(eventData => eventData.name == eventName);
        if (eventToRemove != null)
        {
            eventStateData.events.Remove(eventToRemove);
            Debug.Log($"事件 '{eventName}' 已清除");
        }

        SaveEventStateData(eventStateData);
    }

    // 获取指定事件的状态
    public string GetEventStatus(string eventName)
    {
        EventStateData eventStateData = LoadEventStateData();
        EventSystemData eventData = eventStateData.events.Find(x => x.name == eventName);
        return eventData != null ? eventData.status : "0"; // 返回 "0" 表示未找到
    }
}
