using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName ="新的性格",menuName = "创建/性格")]
public class Personality : ScriptableObject
{
    public enum PersonalityType
    {
        Lazy,//懒惰
        Irritable,//急躁
        Music,//音乐
        Vitality,//有活力
        Timid
    }
    public string personalityName;       //名称

    [LabelText("性格类型")]
    public PersonalityType personalityType;
}
