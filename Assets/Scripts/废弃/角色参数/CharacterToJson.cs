using System;
using System.IO;
using UnityEngine;

[Serializable]
public class CharacterAttributes
{
    public float cuteHeart;      // �ɰ���
    public float emotionalHeart; // ������
    public float rationalHeart;  // ������
    public float vitalityHeart;  // ������
    public float liking; // �øж�

}


// �����������͵�ö��


public interface ICharacterAttributesHandler
{
    void SaveCharacterAttributes(CharacterAttributes attributes);  // �����ɫ����
    CharacterAttributes LoadCharacterAttributes();                  // ���ؽ�ɫ����
    void ModifyCharacterAttribute(CharacterManager.CharacterAttribute attributeType, float value); // �޸�ָ�����ԣ�value Ϊ������ʾ�ӣ�������ʾ��
    void ClearAllData(bool deleteFile = true);
}

public class CharacterAttributesToJson : ICharacterAttributesHandler
{
    private const string jsonFileName = "character_attributes.json"; // JSON �ļ���

    // ��ȡ JSON �ļ�·��
    private string GetJsonFilePath()
    {
        string persistentDataPath = Application.persistentDataPath;
        return Path.Combine(persistentDataPath, jsonFileName);
    }

    // �����ɫ���Ե� JSON �ļ�
    public void SaveCharacterAttributes(CharacterAttributes attributes)
    {
        string filePath = GetJsonFilePath();

        try
        {
            string json = JsonUtility.ToJson(attributes, true);
            File.WriteAllText(filePath, json);
            Debug.Log("��ɫ�������ݱ�����: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"�����ɫ�������ݳ���: {e.Message}");
        }
    }

    // ���ؽ�ɫ��������
    public CharacterAttributes LoadCharacterAttributes()
    {
        string filePath = GetJsonFilePath();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<CharacterAttributes>(json);
        }
        else
        {
            //Debug.LogWarning("δ�ҵ���ɫ���������ļ�������Ĭ��ֵ��");
            return new CharacterAttributes();  // ����Ĭ��ֵ
        }
    }

    // �޸�ָ�����ԣ��ӻ����
    public void ModifyCharacterAttribute(CharacterManager.CharacterAttribute attributeType, float value)
    {
        CharacterAttributes attributes = LoadCharacterAttributes();  // ��ȡ��ǰ��ɫ����

        // ����ö��ֵ���޸Ķ�Ӧ������
        switch (attributeType)
        {
            case CharacterManager.CharacterAttribute.Cute:
                attributes.cuteHeart += value;
                break;
            case CharacterManager.CharacterAttribute.Emotional:
                attributes.emotionalHeart += value;
                break;
            case CharacterManager.CharacterAttribute.Rational:
                attributes.rationalHeart += value;
                break;
            case CharacterManager.CharacterAttribute.Vitality:
                attributes.vitalityHeart += value;
                break;
            case CharacterManager.CharacterAttribute.Liking: // �����øж�
                attributes.liking += value;
                break;
            default:
                Debug.LogWarning("δ֪����������: " + attributeType);
                return;
        }

        // ������º�Ľ�ɫ����
        SaveCharacterAttributes(attributes);
        Debug.Log($"�Ѹ��� {attributeType}����ֵΪ: {value}");
    }

    // ������н�ɫ���ݣ�ɾ���ļ�������ΪĬ��ֵ
    public void ClearAllData(bool deleteFile = true)
    {
        string filePath = GetJsonFilePath();

        if (deleteFile)
        {
            // ɾ���洢�� JSON �ļ�
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("��ɫ���������ļ���ɾ��");
            }
            else
            {
                Debug.LogWarning("û���ҵ�Ҫɾ�����ļ�");
            }
        }
        else
        {
            // ���ý�ɫ����ΪĬ��ֵ������
            CharacterAttributes defaultAttributes = new CharacterAttributes();
            SaveCharacterAttributes(defaultAttributes);
            Debug.Log("��ɫ��������������ΪĬ��ֵ");
        }
    }
}
