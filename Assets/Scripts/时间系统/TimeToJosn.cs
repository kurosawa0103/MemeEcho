using System;
using System.IO;
using UnityEngine;

[Serializable]
public class TimeState
{
    public int day;        // 当前天数
    public float timeCount; // 当前时间计数
}

public interface ITimeHandler
{
    void SaveTimeState(int day, float timeCount);    // 保存时间状态
    TimeState LoadTimeState();                       // 加载时间状态
    void ClearTimeState();                            // 清除时间状态
}

public class TimeToJson : ITimeHandler
{
    private const string jsonFileName = "time_data.json"; // 时间数据 JSON 文件名

    // 获取时间数据文件路径
    private string GetJsonFilePath()
    {
        string persistentDataPath = Application.persistentDataPath;
        return Path.Combine(persistentDataPath, jsonFileName);
    }

    // 保存时间状态到 JSON 文件
    public void SaveTimeState(int day, float timeCount)
    {
        string filePath = GetJsonFilePath();

        try
        {
            TimeState timeState = LoadTimeState() ?? new TimeState(); // 如果没有找到已存在的状态，则创建一个新的

            // 更新天数和时间计数
            timeState.day = day;
            timeState.timeCount = timeCount;

            string json = JsonUtility.ToJson(timeState, true);
            File.WriteAllText(filePath, json);
            Debug.Log("时间数据保存至: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"保存时间数据出错: {e.Message}");
        }
    }

    // 加载时间数据
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
            // 如果文件不存在，返回一个新的 TimeState，并将天数设为 1
            return new TimeState { day = 1, timeCount = 0 };
        }
    }

    // 清除时间状态数据
    public void ClearTimeState()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log("时间数据已清除，文件已删除: " + filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"清除时间数据出错: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("未找到时间数据文件，无法清除数据。");
        }
    }
}
