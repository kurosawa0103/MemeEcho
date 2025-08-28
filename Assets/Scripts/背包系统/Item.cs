using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "�µ���Ʒ", menuName = "��������/��Ʒ")]
public class Item : ScriptableObject
{
    //[LabelText("�л�action����")]
    //public CharacterState.CharAction charAction;
    [LabelText("��Ʒ����")]
    public string itemName;
    [LabelText("��ƷͼƬ")]
    public Sprite itemImage;
    [LabelText("��ƷԤ����")]
    public GameObject itemPrefab;
    //[LabelText("�����¼�list")]
    //public EventData itemEvent;
    [LabelText("��Ʒdata·��")]
    public string itemAddress;
    [LabelText("��Ʒ����")]
    [TextArea] public string itemDesc;
}
