using System;
using System.IO;
using UnityEngine;

[Serializable]
public class TimeState
{
    public int day;        // ��ǰ����
    public float timeCount; // ��ǰʱ�����
}

public interface ITimeHandler
{
    void SaveTimeState(int day, float timeCount);    // ����ʱ��״̬
    TimeState LoadTimeState();                       // ����ʱ��״̬
    void ClearTimeState();                            // ���ʱ��״̬
}

public class TimeToJson : ITimeHandler
{
    private const string jsonFileName = "time_data.json"; // ʱ������ JSON �ļ���

    // ��ȡʱ�������ļ�·��
    private string GetJsonFilePath()
    {
        string persistentDataPath = Application.persistentDataPath;
        return Path.Combine(persistentDataPath, jsonFileName);
    }

    // ����ʱ��״̬�� JSON �ļ�
    public void SaveTimeState(int day, float timeCount)
    {
        string filePath = GetJsonFilePath();

        try
        {
            TimeState timeState = LoadTimeState() ?? new TimeState(); // ���û���ҵ��Ѵ��ڵ�״̬���򴴽�һ���µ�

            // ����������ʱ�����
            timeState.day = day;
            timeState.timeCount = timeCount;

            string json = JsonUtility.ToJson(timeState, true);
            File.WriteAllText(filePath, json);
            Debug.Log("ʱ�����ݱ�����: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"����ʱ�����ݳ���: {e.Message}");
        }
    }

    // ����ʱ������
    public TimeState LoadTimeState()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            TimeState timeState = JsonUtility.FromJson<TimeState>(json);
            return timeState;
        }
        else
        {
            // ����ļ������ڣ�����һ���µ� TimeState������������Ϊ 1
            return new TimeState { day = 1, timeCount = 0 };
        }
    }

    // ���ʱ��״̬����
    public void ClearTimeState()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log("ʱ��������������ļ���ɾ��: " + filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"���ʱ�����ݳ���: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("δ�ҵ�ʱ�������ļ����޷�������ݡ�");
        }
    }
}
