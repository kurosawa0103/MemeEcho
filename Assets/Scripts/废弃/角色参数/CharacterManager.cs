using System;
using UnityEngine;
using Sirenix.OdinInspector;  // 引入 Odin Inspector

public class CharacterManager : MonoBehaviour
{
    private ICharacterAttributesHandler characterAttributesHandler;

    [Title("角色属性")]
    [LabelText("好感度")]
    public float liking;

    [LabelText("可爱心")]
    public float cuteHeart;

    [LabelText("感性心")]
    public float emotionalHeart;

    [LabelText("理性心")]
    public float rationalHeart;

    [LabelText("活力心")]
    public float vitalityHeart;

    public event Action OnAttributesChanged;

    // 在启动时，初始化 ICharacterAttributesHandler
    private void Awake()
    {
        characterAttributesHandler = new CharacterAttributesToJson();

        // 可以选择加载角色属性，若没有文件则加载默认值
        CharacterAttributes attributes = characterAttributesHandler.LoadCharacterAttributes();
        cuteHeart = attributes.cuteHeart;
        emotionalHeart = attributes.emotionalHeart;
        rationalHeart = attributes.rationalHeart;
        vitalityHeart = attributes.vitalityHeart;
    }


    // 获取角色属性
    public CharacterAttributes GetCharacterAttributes()
    {
        return characterAttributesHandler.LoadCharacterAttributes();
    }

    // 修改角色的属性
    public void ModifyCharacterAttribute(CharacterAttribute attributeType, float value)
    {
        characterAttributesHandler.ModifyCharacterAttribute(attributeType, value);

        switch (attributeType)
        {
            case CharacterAttribute.Cute:
                cuteHeart = value;
                break;
            case CharacterAttribute.Emotional:
                emotionalHeart = value;
                break;
            case CharacterAttribute.Rational:
                rationalHeart = value;
                break;
            case CharacterAttribute.Vitality:
                vitalityHeart = value;
                break;
        }

        OnAttributesChanged?.Invoke(); // 触发事件
    }

    // 保存角色属性（可以在修改后手动保存）
    public void SaveCharacterAttributes(CharacterAttributes attributes)
    {
        characterAttributesHandler.SaveCharacterAttributes(attributes);
    }

    public enum CharacterAttribute
    {
        Liking,
        Cute,
        Emotional,
        Rational,
        Vitality
    }
}
