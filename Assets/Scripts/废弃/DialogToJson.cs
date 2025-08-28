using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DialogState
{
    public string dialogID;  // 对话的唯一标识
    public string dialogState; // 对话的状态，例如 "completed" 或 "in_progress"
}

[Serializable]
public class DialogDataList
{
    public List<DialogState> dialogStates = new List<DialogState>(); // 存储所有对话的状态
    public string currentDialogAddress;  // 当前进行中的对话路径
}

public interface IDialogHandler
{
    void SaveDialogState(List<DialogState> dialogStates);  // 保存对话状态
    List<DialogState> LoadDialogState();                   // 加载对话状态
    void SaveDialogState(string dialogName, string dialogState);
    void SaveCurrentDialogAddress(string currentDialogAddress);  // 保存当前对话地址
    string GetCurrentDialogAddress();                       // 获取当前对话地址
    void ClearDialogState();                              // 清除对话状态
    bool IsDialogCompleted(string dialogName);
}

// 实现对话数据保存和加载的类
public class DialogToJson : MonoBehaviour, IDialogHandler
{
    private const string jsonFileName = "dialog_data.json"; // 对话状态 JSON 文件名

    // 获取对话数据文件路径
    private string GetJsonFilePath()
    {
        string persistentDataPath = Application.persistentDataPath;
        return Path.Combine(persistentDataPath, jsonFileName);
    }

    // 保存对话状态到 JSON 文件
    public void SaveDialogState(List<DialogState> dialogStates)
    {
        string filePath = GetJsonFilePath();

        try
        {
            DialogDataList dialogDataList = LoadDialogDataList();  // 获取现有的对话数据
            dialogDataList.dialogStates = dialogStates;  // 更新对话状态

            string json = JsonUtility.ToJson(dialogDataList, true);
            File.WriteAllText(filePath, json);
            Debug.Log("对话数据保存至: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"保存对话数据出错: {e.Message}");
        }
    }

    // 保存单个对话状态
    public void SaveDialogState(string dialogName, string dialogState)
    {
        List<DialogState> dialogStates = LoadDialogState();  // 获取当前的对话状态

        // 查找对话状态
        DialogState existingDialogState = dialogStates.Find(d => d.dialogID == dialogName);

        if (existingDialogState == null)
        {
            // 如果找不到该对话，新增一个对话状态
            existingDialogState = new DialogState { dialogID = dialogName, dialogState = dialogState };
            dialogStates.Add(existingDialogState);
        }
        else
        {
            // 如果对话已存在，更新状态
            existingDialogState.dialogState = dialogState;
        }

        // 保存更新后的对话状态
        SaveDialogState(dialogStates);
    }

    // 获取对话数据
    private DialogDataList LoadDialogDataList()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            DialogDataList dialogDataList = JsonUtility.FromJson<DialogDataList>(json);
            return dialogDataList;
        }
        else
        {
            // 如果文件不存在，返回一个新的 DialogDataList
            return new DialogDataList();
        }
    }

    // 加载 JSON 文件中的对话状态数据
    public List<DialogState> LoadDialogState()
    {
        return LoadDialogDataList().dialogStates;
    }

    // 保存当前对话路径
    public void SaveCurrentDialogAddress(string currentDialogAddress)
    {
        string filePath = GetJsonFilePath();

        try
        {
            DialogDataList dialogDataList = LoadDialogDataList();  // 获取现有的对话数据
            dialogDataList.currentDialogAddress = currentDialogAddress;  // 更新当前对话路径

            string json = JsonUtility.ToJson(dialogDataList, true);
            File.WriteAllText(filePath, json);  // 保存更新后的数据
            Debug.Log("当前对话路径保存至: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"保存当前对话路径时出错: {e.Message}");
        }
    }

    // 获取当前对话路径
    public string GetCurrentDialogAddress()
    {
        return LoadDialogDataList().currentDialogAddress;
    }

    // 清除所有对话状态数据
    public void ClearDialogState()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log("对话数据已清除，文件已删除: " + filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"清除对话数据出错: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("未找到对话数据文件，无法清除数据。");
        }
    }

    // 根据对话名称检查指定对话是否完成
    public bool IsDialogCompleted(string dialogName)
    {
        List<DialogState> dialogStates = LoadDialogState();  // 获取所有对话的状态

        // 查找指定对话的状态
        DialogState dialogState = dialogStates.Find(d => d.dialogID == dialogName);

        // 如果对话存在，返回其状态；如果不存在，返回 false
        return dialogState != null && dialogState.dialogState == "completed";
    }
}
