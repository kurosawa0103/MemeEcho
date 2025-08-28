using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// �����л��ı��������࣬�������������Ǻ͸�����
[Serializable]
public class InventoryData
{
    public List<string> packItems = new List<string>(); // ������Ʒ��ַ�б�
}

// ��Ʒ���ݴ���ӿ�
public interface IItemHandler
{
    void SaveInventory(InventoryData data); // ����������������
    InventoryData LoadInventory(); // ����������������
    void ClearInventory(); // ������б�������
}

// ʵ����Ʒ���ݱ���ͼ��ص���
public class ItemToJson : IItemHandler
{
    private const string jsonFileName = "inventory_data.json"; // JSON �ļ���

    // ��ȡ JSON �ļ�·��
    private string GetJsonFilePath()
    {
        return Path.Combine(Application.persistentDataPath, jsonFileName); // ��ȡ�־û�·��
    }

    // ���汳�����ݵ� JSON �ļ�
    public void SaveInventory(InventoryData data)
    {
        string filePath = GetJsonFilePath(); // ��ȡ JSON �ļ�·��

        try
        {
            string json = JsonUtility.ToJson(data, true); // ת��Ϊ JSON �ַ���
            File.WriteAllText(filePath, json); // �� JSON д���ļ�
            Debug.Log("���������ѱ�����: " + filePath); // ��־��¼����ɹ�
        }
        catch (Exception e)
        {
            Debug.LogError($"���汳������ʱ����: {e.Message}"); // ��¼����ʧ�ܵĴ�����Ϣ
        }
    }

    // �� JSON �ļ����ر�������
    public InventoryData LoadInventory()
    {
        string filePath = GetJsonFilePath(); // ��ȡ JSON �ļ�·��

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath); // ��ȡ JSON �ļ�����
                var data = JsonUtility.FromJson<InventoryData>(json); // ת��Ϊ����
                if (data == null)
                {
                    Debug.LogWarning("���� JSON ʧ�ܣ�����Ĭ�Ͽ����ݡ�");
                    return new InventoryData();
                }
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"���ر�������ʱ����: {e.Message}");
                return new InventoryData(); // ����Ĭ�Ͽ�����
            }
        }
        else
        {
            Debug.LogWarning("δ�ҵ����������ļ�������Ĭ�Ͽ����ݡ�");
            return new InventoryData(); // ����Ĭ�Ͽ�����
        }
    }


    // ������б�������
    public void ClearInventory()
    {
        string filePath = GetJsonFilePath(); // ��ȡ JSON �ļ�·��

        if (File.Exists(filePath))
        {
            try
            {
                // ����һ���յ� InventoryData ʵ��
                InventoryData emptyData = new InventoryData
                {
                    packItems = new List<string>()
                };

                SaveInventory(emptyData); // ����յı�������
                Debug.Log("���������������������Ʒ�����á�");
            }
            catch (Exception e)
            {
                Debug.LogError($"�����������ʱ����: {e.Message}"); // ��¼ɾ��ʧ�ܵĴ�����Ϣ
            }
        }
        else
        {
            Debug.LogWarning("δ�ҵ����������ļ����޷�������ݡ�"); // ��־��¼�ļ�δ�ҵ�
        }
    }
}
