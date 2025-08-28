using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DialogState
{
    public string dialogID;  // �Ի���Ψһ��ʶ
    public string dialogState; // �Ի���״̬������ "completed" �� "in_progress"
}

[Serializable]
public class DialogDataList
{
    public List<DialogState> dialogStates = new List<DialogState>(); // �洢���жԻ���״̬
    public string currentDialogAddress;  // ��ǰ�����еĶԻ�·��
}

public interface IDialogHandler
{
    void SaveDialogState(List<DialogState> dialogStates);  // ����Ի�״̬
    List<DialogState> LoadDialogState();                   // ���ضԻ�״̬
    void SaveDialogState(string dialogName, string dialogState);
    void SaveCurrentDialogAddress(string currentDialogAddress);  // ���浱ǰ�Ի���ַ
    string GetCurrentDialogAddress();                       // ��ȡ��ǰ�Ի���ַ
    void ClearDialogState();                              // ����Ի�״̬
    bool IsDialogCompleted(string dialogName);
}

// ʵ�ֶԻ����ݱ���ͼ��ص���
public class DialogToJson : MonoBehaviour, IDialogHandler
{
    private const string jsonFileName = "dialog_data.json"; // �Ի�״̬ JSON �ļ���

    // ��ȡ�Ի������ļ�·��
    private string GetJsonFilePath()
    {
        string persistentDataPath = Application.persistentDataPath;
        return Path.Combine(persistentDataPath, jsonFileName);
    }

    // ����Ի�״̬�� JSON �ļ�
    public void SaveDialogState(List<DialogState> dialogStates)
    {
        string filePath = GetJsonFilePath();

        try
        {
            DialogDataList dialogDataList = LoadDialogDataList();  // ��ȡ���еĶԻ�����
            dialogDataList.dialogStates = dialogStates;  // ���¶Ի�״̬

            string json = JsonUtility.ToJson(dialogDataList, true);
            File.WriteAllText(filePath, json);
            Debug.Log("�Ի����ݱ�����: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"����Ի����ݳ���: {e.Message}");
        }
    }

    // ���浥���Ի�״̬
    public void SaveDialogState(string dialogName, string dialogState)
    {
        List<DialogState> dialogStates = LoadDialogState();  // ��ȡ��ǰ�ĶԻ�״̬

        // ���ҶԻ�״̬
        DialogState existingDialogState = dialogStates.Find(d => d.dialogID == dialogName);

        if (existingDialogState == null)
        {
            // ����Ҳ����öԻ�������һ���Ի�״̬
            existingDialogState = new DialogState { dialogID = dialogName, dialogState = dialogState };
            dialogStates.Add(existingDialogState);
        }
        else
        {
            // ����Ի��Ѵ��ڣ�����״̬
            existingDialogState.dialogState = dialogState;
        }

        // ������º�ĶԻ�״̬
        SaveDialogState(dialogStates);
    }

    // ��ȡ�Ի�����
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
            // ����ļ������ڣ�����һ���µ� DialogDataList
            return new DialogDataList();
        }
    }

    // ���� JSON �ļ��еĶԻ�״̬����
    public List<DialogState> LoadDialogState()
    {
        return LoadDialogDataList().dialogStates;
    }

    // ���浱ǰ�Ի�·��
    public void SaveCurrentDialogAddress(string currentDialogAddress)
    {
        string filePath = GetJsonFilePath();

        try
        {
            DialogDataList dialogDataList = LoadDialogDataList();  // ��ȡ���еĶԻ�����
            dialogDataList.currentDialogAddress = currentDialogAddress;  // ���µ�ǰ�Ի�·��

            string json = JsonUtility.ToJson(dialogDataList, true);
            File.WriteAllText(filePath, json);  // ������º������
            Debug.Log("��ǰ�Ի�·��������: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"���浱ǰ�Ի�·��ʱ����: {e.Message}");
        }
    }

    // ��ȡ��ǰ�Ի�·��
    public string GetCurrentDialogAddress()
    {
        return LoadDialogDataList().currentDialogAddress;
    }

    // ������жԻ�״̬����
    public void ClearDialogState()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log("�Ի�������������ļ���ɾ��: " + filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"����Ի����ݳ���: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("δ�ҵ��Ի������ļ����޷�������ݡ�");
        }
    }

    // ���ݶԻ����Ƽ��ָ���Ի��Ƿ����
    public bool IsDialogCompleted(string dialogName)
    {
        List<DialogState> dialogStates = LoadDialogState();  // ��ȡ���жԻ���״̬

        // ����ָ���Ի���״̬
        DialogState dialogState = dialogStates.Find(d => d.dialogID == dialogName);

        // ����Ի����ڣ�������״̬����������ڣ����� false
        return dialogState != null && dialogState.dialogState == "completed";
    }
}
