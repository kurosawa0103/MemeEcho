using System;
using System.IO;
using UnityEngine;

// �����л�����Ϸ������
[Serializable]
public class GameData
{
    public string currentEventPath;
}


// ��Ϸ���ݴ���ӿ�
public interface IGameDataHandler
{
    void SaveGameData(GameData gameData); // ������Ϸ����
    GameData LoadGameData();              // ��ȡ��Ϸ����

    // ���������úͻ�ȡ��ǰ�¼�·��
    void SetCurrentEventPath(string path);
    string GetCurrentEventPath();
    void ClearGameData(); // �����Ϸ����
}



// ʵ����Ϸ���ݱ���ͼ��ص���
public class GameDataToJson : IGameDataHandler
{
    private const string jsonFileName = "game_data.json"; // JSON �ļ���

    private string GetJsonFilePath()
    {
        string persistentDataPath = Application.persistentDataPath; // ��ȡ�־û�����·��
        return Path.Combine(persistentDataPath, jsonFileName); // ���������� JSON �ļ�·��
    }

    public void SaveGameData(GameData gameData)
    {
        string filePath = GetJsonFilePath(); // ��ȡ JSON �ļ�·��

        try
        {
            string json = JsonUtility.ToJson(gameData, true); // ת��Ϊ JSON �ַ���
            File.WriteAllText(filePath, json); // �� JSON д���ļ�
            Debug.Log("��Ϸ���ݱ�����: " + filePath); // ��־��¼�ɹ���Ϣ
        }
        catch (Exception e)
        {
            Debug.LogError($"������Ϸ���ݳ���: {e.Message}"); // ��¼����ʧ�ܵĴ�����Ϣ
        }
    }

    public GameData LoadGameData()
    {
        string filePath = GetJsonFilePath(); // ��ȡ JSON �ļ�·��

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath); // ��ȡ JSON �ļ�����
            GameData gameData = JsonUtility.FromJson<GameData>(json); // �� JSON ת��Ϊ GameData ����

            return gameData; // ������Ϸ���ݶ���
        }
        else
        {
            Debug.LogWarning("δ�ҵ���Ϸ�����ļ�������Ĭ�ϵ� GameData ����"); // ��־��¼�ļ�δ�ҵ��ľ�����Ϣ
            return new GameData(); // ����ļ������ڣ�����Ĭ�ϵ� GameData ����
        }
    }

    // ���õ�ǰ�¼�·��
    public void SetCurrentEventPath(string path)
    {
        GameData gameData = LoadGameData();
        gameData.currentEventPath = path;
        SaveGameData(gameData);
    }

    public string GetCurrentEventPath()
    {
        GameData gameData = LoadGameData();
        return gameData.currentEventPath;
    }
    public void ClearGameData()
    {
        string filePath = GetJsonFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("�������Ϸ����: " + filePath);
        }
        else
        {
            Debug.LogWarning("���������Ϸ���ݣ����ļ�������: " + filePath);
        }
    }

}
