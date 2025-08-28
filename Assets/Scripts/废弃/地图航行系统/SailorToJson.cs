using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SailorState
{
    public string currentMapPath;       // ��ǰ��ͼ·��
    public float currentDistance;       // ��ǰ����
    public string nextDestinationPath;  // ��һ��Ŀ�ĵ�·��
}


public interface ISailorHandler
{
    void SaveSailorState(string mapPath, float distance, string nextDestinationPath = null); // ���洬Ա״̬
    SailorState LoadSailorState();                                                  // ���ش�Ա״̬
    void ClearSailorState();                                                         // �����Ա״̬
}


public class SailorToJson : ISailorHandler
{
    private const string jsonFileName = "sailor_data.json"; // ��Ա״̬ JSON �ļ���

    // ��ȡ��Ա�����ļ�·��
    private string GetJsonFilePath()
    {
        string persistentDataPath = Application.persistentDataPath;
        return Path.Combine(persistentDataPath, jsonFileName);
    }

    // ���洬Ա״̬�� JSON �ļ�
    public void SaveSailorState(string mapPath, float distance, string nextDestinationPath = null)
    {
        string filePath = GetJsonFilePath();

        try
        {
            SailorState sailorState = LoadSailorState() ?? new SailorState(); // ���û���ҵ��Ѵ��ڵ�״̬���򴴽�һ���µ�

            // ���µ�ͼ·���;���
            sailorState.currentMapPath = mapPath;
            sailorState.currentDistance = distance;

            // ��������� nextDestinationPath�������
            if (nextDestinationPath != null)
            {
                sailorState.nextDestinationPath = nextDestinationPath;
            }

            string json = JsonUtility.ToJson(sailorState, true);
            File.WriteAllText(filePath, json);
            //Debug.Log("��Ա���ݱ�����: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"���洬Ա���ݳ���: {e.Message}");
        }
    }


    // ���ش�Ա����
    public SailorState LoadSailorState()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SailorState sailorState = JsonUtility.FromJson<SailorState>(json);
            return sailorState;
        }
        else
        {
            // ����ļ������ڣ�����һ���µ� SailorState
            return new SailorState();
        }
    }

    // �����Ա״̬����
    public void ClearSailorState()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log("��Ա������������ļ���ɾ��: " + filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"�����Ա���ݳ���: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("δ�ҵ���Ա�����ļ����޷�������ݡ�");
        }
    }
}


