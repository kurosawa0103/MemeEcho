using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "新的物品", menuName = "创建配置/物品")]
public class Item : ScriptableObject
{
    //[LabelText("切换action类型")]
    //public CharacterState.CharAction charAction;
    [LabelText("物品名称")]
    public string itemName;
    [LabelText("物品图片")]
    public Sprite itemImage;
    [LabelText("物品预制体")]
    public GameObject itemPrefab;
    //[LabelText("触发事件list")]
    //public EventData itemEvent;
    [LabelText("物品data路径")]
    public string itemAddress;
    [LabelText("物品描述")]
    [TextArea] public string itemDesc;
}
