using System;
using System.IO;
using UnityEngine;

[Serializable]
public class CharacterAttributes
{
    public float cuteHeart;      // 可爱心
    public float emotionalHeart; // 感性心
    public float rationalHeart;  // 理性心
    public float vitalityHeart;  // 活力心
    public float liking; // 好感度

}


// 定义属性类型的枚举


public interface ICharacterAttributesHandler
{
    void SaveCharacterAttributes(CharacterAttributes attributes);  // 保存角色属性
    CharacterAttributes LoadCharacterAttributes();                  // 加载角色属性
    void ModifyCharacterAttribute(CharacterManager.CharacterAttribute attributeType, float value); // 修改指定属性，value 为正数表示加，负数表示减
    void ClearAllData(bool deleteFile = true);
}

public class CharacterAttributesToJson : ICharacterAttributesHandler
{
    private const string jsonFileName = "character_attributes.json"; // JSON 文件名

    // 获取 JSON 文件路径
    private string GetJsonFilePath()
    {
        string persistentDataPath = Application.persistentDataPath;
        return Path.Combine(persistentDataPath, jsonFileName);
    }

    // 保存角色属性到 JSON 文件
    public void SaveCharacterAttributes(CharacterAttributes attributes)
    {
        string filePath = GetJsonFilePath();

        try
        {
            string json = JsonUtility.ToJson(attributes, true);
            File.WriteAllText(filePath, json);
            Debug.Log("角色属性数据保存至: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"保存角色属性数据出错: {e.Message}");
        }
    }

    // 加载角色属性数据
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
            //Debug.LogWarning("未找到角色属性数据文件，返回默认值。");
            return new CharacterAttributes();  // 返回默认值
        }
    }

    // 修改指定属性（加或减）
    public void ModifyCharacterAttribute(CharacterManager.CharacterAttribute attributeType, float value)
    {
        CharacterAttributes attributes = LoadCharacterAttributes();  // 获取当前角色属性

        // 根据枚举值，修改对应的属性
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
            case CharacterManager.CharacterAttribute.Liking: // 新增好感度
                attributes.liking += value;
                break;
            default:
                Debug.LogWarning("未知的属性类型: " + attributeType);
                return;
        }

        // 保存更新后的角色属性
        SaveCharacterAttributes(attributes);
        Debug.Log($"已更新 {attributeType}，新值为: {value}");
    }

    // 清除所有角色数据：删除文件或重置为默认值
    public void ClearAllData(bool deleteFile = true)
    {
        string filePath = GetJsonFilePath();

        if (deleteFile)
        {
            // 删除存储的 JSON 文件
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("角色属性数据文件已删除");
            }
            else
            {
                Debug.LogWarning("没有找到要删除的文件");
            }
        }
        else
        {
            // 重置角色属性为默认值并保存
            CharacterAttributes defaultAttributes = new CharacterAttributes();
            SaveCharacterAttributes(defaultAttributes);
            Debug.Log("角色属性数据已重置为默认值");
        }
    }
}
