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
    void ClearEventStateData(); // ��������¼�״̬
    void ClearSpecificEvent(string eventName); // ���ָ���¼�
    string GetEventStatus(string eventName); // ��ȡָ���¼�״̬
}

public class JsonGenerator : IEventStateDataHandler
{
    private const string jsonFileName = "event_state_data.json";
    // �¼�״̬���ʱ�������¼�
    public static event Action<string, string> OnEventStatusChanged;
    private string GetJsonFilePath()
    {
        return Path.Combine(Application.persistentDataPath, jsonFileName);
    }

    // �����¼�״̬����
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

    // ����ļ��Ƿ���ڣ��������򴴽�
    private void CheckAndCreateEventStateFile()
    {
        string filePath = GetJsonFilePath();

        if (!File.Exists(filePath))
        {
            EventStateData initialData = new EventStateData();
            SaveEventStateData(initialData);
        }
    }

    // �����¼�״̬���ݵ� JSON �ļ�
    public void SaveEventStateData(EventStateData eventStateData)
    {
        string filePath = GetJsonFilePath();

        try
        {
            string json = JsonUtility.ToJson(eventStateData, true);
            File.WriteAllText(filePath, json);
            Debug.Log("�¼�״̬�����ѱ�����: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving event state data: {e.Message}");
        }
    }

    // �޸�ָ���¼���״̬
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
        // �����¼�״̬�ı���¼�֪ͨ���ж�����
        OnEventStatusChanged?.Invoke(eventName, newStatus);
    }

    // ��������¼�����
    public void ClearEventStateData()
    {
        EventStateData eventStateData = new EventStateData();
        SaveEventStateData(eventStateData);
        Debug.Log("�����¼�״̬���������");
        // �����¼�״̬�ı���¼�֪ͨ���ж�����
    }

    // ���ָ���¼�
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
            Debug.Log($"�¼� '{eventName}' �����");
        }

        SaveEventStateData(eventStateData);
    }

    // ��ȡָ���¼���״̬
    public string GetEventStatus(string eventName)
    {
        EventStateData eventStateData = LoadEventStateData();
        EventSystemData eventData = eventStateData.events.Find(x => x.name == eventName);
        return eventData != null ? eventData.status : "0"; // ���� "0" ��ʾδ�ҵ�
    }
}
