using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class EventState
{
    public string eventID;  // �¼���Ψһ��ʶ
    public string eventState; // �¼���״̬������ "completed" �� "in_progress"
}

[Serializable]
public class EventDataList
{
    public List<EventState> eventStates = new List<EventState>(); // �洢�����¼���״̬
    public string currentEventAddress;  // ��ǰ�����е��¼�·��
}

public interface IEventHandler
{
    void SaveEventState(List<EventState> eventStates);  // �����¼�״̬
    List<EventState> LoadEventState();                   // �����¼�״̬
    void SaveEventState(string eventName, string eventState);
    void SaveCurrentEventAddress(string currentEventAddress);  // ���浱ǰ�¼���ַ
    string GetCurrentEventAddress();                       // ��ȡ��ǰ�¼���ַ
    void ClearEventState();                              // ����¼�״̬
    bool IsEventCompleted(string eventName);
}

// ʵ���¼����ݱ���ͼ��ص���
public class EventToJson : MonoBehaviour, IEventHandler
{
    private const string jsonFileName = "event_data.json"; // �¼�״̬ JSON �ļ���

    // ��ȡ�¼������ļ�·��
    private string GetJsonFilePath()
    {
        string persistentDataPath = Application.persistentDataPath;
        return Path.Combine(persistentDataPath, jsonFileName);
    }

    // �����¼�״̬�� JSON �ļ�
    public void SaveEventState(List<EventState> eventStates)
    {
        string filePath = GetJsonFilePath();

        try
        {
            EventDataList eventDataList = LoadEventDataList();  // ��ȡ���е��¼�����
            eventDataList.eventStates = eventStates;  // �����¼�״̬

            string json = JsonUtility.ToJson(eventDataList, true);
            File.WriteAllText(filePath, json);
            Debug.Log("�¼����ݱ�����: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"�����¼����ݳ���: {e.Message}");
        }
    }

    // ���浥���¼�״̬
    public void SaveEventState(string eventName, string eventState)
    {
        List<EventState> eventStates = LoadEventState();  // ��ȡ��ǰ���¼�״̬

        // �����¼�״̬
        EventState existingEventState = eventStates.Find(e => e.eventID == eventName);

        if (existingEventState == null)
        {
            // ����Ҳ������¼�������һ���¼�״̬
            existingEventState = new EventState { eventID = eventName, eventState = eventState };
            eventStates.Add(existingEventState);
        }
        else
        {
            // ����¼��Ѵ��ڣ�����״̬
            existingEventState.eventState = eventState;
        }

        // ������º���¼�״̬
        SaveEventState(eventStates);
    }

    // ��ȡ�¼�����
    private EventDataList LoadEventDataList()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            EventDataList eventDataList = JsonUtility.FromJson<EventDataList>(json);
            return eventDataList;
        }
        else
        {
            // ����ļ������ڣ�����һ���µ� EventDataList
            return new EventDataList();
        }
    }

    // ���� JSON �ļ��е��¼�״̬����
    public List<EventState> LoadEventState()
    {
        return LoadEventDataList().eventStates;
    }

    // ���浱ǰ�¼�·��
    public void SaveCurrentEventAddress(string currentEventAddress)
    {
        string filePath = GetJsonFilePath();

        try
        {
            EventDataList eventDataList = LoadEventDataList();  // ��ȡ���е��¼�����
            eventDataList.currentEventAddress = currentEventAddress;  // ���µ�ǰ�¼�·��

            string json = JsonUtility.ToJson(eventDataList, true);
            File.WriteAllText(filePath, json);  // ������º������
            Debug.Log("��ǰ�¼�·��������: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"���浱ǰ�¼�·��ʱ����: {e.Message}");
        }
    }

    // ��ȡ��ǰ�¼�·��
    public string GetCurrentEventAddress()
    {
        return LoadEventDataList().currentEventAddress;
    }

    // ��������¼�״̬����
    public void ClearEventState()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log("�¼�������������ļ���ɾ��: " + filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"����¼����ݳ���: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("δ�ҵ��¼������ļ����޷�������ݡ�");
        }
    }

    // �����¼����Ƽ��ָ���¼��Ƿ����
    public bool IsEventCompleted(string eventName)
    {
        List<EventState> eventStates = LoadEventState();  // ��ȡ�����¼���״̬

        // ����ָ���¼���״̬
        EventState eventState = eventStates.Find(e => e.eventID == eventName);

        // ����¼����ڣ�������״̬����������ڣ����� false
        return eventState != null && eventState.eventState == "completed";
    }
}
