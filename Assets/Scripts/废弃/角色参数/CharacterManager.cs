using System;
using UnityEngine;
using Sirenix.OdinInspector;  // ���� Odin Inspector

public class CharacterManager : MonoBehaviour
{
    private ICharacterAttributesHandler characterAttributesHandler;

    [Title("��ɫ����")]
    [LabelText("�øж�")]
    public float liking;

    [LabelText("�ɰ���")]
    public float cuteHeart;

    [LabelText("������")]
    public float emotionalHeart;

    [LabelText("������")]
    public float rationalHeart;

    [LabelText("������")]
    public float vitalityHeart;

    public event Action OnAttributesChanged;

    // ������ʱ����ʼ�� ICharacterAttributesHandler
    private void Awake()
    {
        characterAttributesHandler = new CharacterAttributesToJson();

        // ����ѡ����ؽ�ɫ���ԣ���û���ļ������Ĭ��ֵ
        CharacterAttributes attributes = characterAttributesHandler.LoadCharacterAttributes();
        cuteHeart = attributes.cuteHeart;
        emotionalHeart = attributes.emotionalHeart;
        rationalHeart = attributes.rationalHeart;
        vitalityHeart = attributes.vitalityHeart;
    }


    // ��ȡ��ɫ����
    public CharacterAttributes GetCharacterAttributes()
    {
        return characterAttributesHandler.LoadCharacterAttributes();
    }

    // �޸Ľ�ɫ������
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

        OnAttributesChanged?.Invoke(); // �����¼�
    }

    // �����ɫ���ԣ��������޸ĺ��ֶ����棩
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
