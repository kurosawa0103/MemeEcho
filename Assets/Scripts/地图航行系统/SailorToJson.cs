using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SailorState
{
    public string currentMapPath;       // 当前地图路径
    public float currentDistance;       // 当前距离
    public string nextDestinationPath;  // 下一个目的地路径
}


public interface ISailorHandler
{
    void SaveSailorState(string mapPath, float distance, string nextDestinationPath = null); // 保存船员状态
    SailorState LoadSailorState();                                                  // 加载船员状态
    void ClearSailorState();                                                         // 清除船员状态
}


public class SailorToJson : ISailorHandler
{
    private const string jsonFileName = "sailor_data.json"; // 船员状态 JSON 文件名

    // 获取船员数据文件路径
    private string GetJsonFilePath()
    {
        string persistentDataPath = Application.persistentDataPath;
        return Path.Combine(persistentDataPath, jsonFileName);
    }

    // 保存船员状态到 JSON 文件
    public void SaveSailorState(string mapPath, float distance, string nextDestinationPath = null)
    {
        string filePath = GetJsonFilePath();

        try
        {
            SailorState sailorState = LoadSailorState() ?? new SailorState(); // 如果没有找到已存在的状态，则创建一个新的

            // 更新地图路径和距离
            sailorState.currentMapPath = mapPath;
            sailorState.currentDistance = distance;

            // 如果传入了 nextDestinationPath，则更新
            if (nextDestinationPath != null)
            {
                sailorState.nextDestinationPath = nextDestinationPath;
            }

            string json = JsonUtility.ToJson(sailorState, true);
            File.WriteAllText(filePath, json);
            //Debug.Log("船员数据保存至: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"保存船员数据出错: {e.Message}");
        }
    }


    // 加载船员数据
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
            // 如果文件不存在，返回一个新的 SailorState
            return new SailorState();
        }
    }

    // 清除船员状态数据
    public void ClearSailorState()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log("船员数据已清除，文件已删除: " + filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"清除船员数据出错: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("未找到船员数据文件，无法清除数据。");
        }
    }
}


