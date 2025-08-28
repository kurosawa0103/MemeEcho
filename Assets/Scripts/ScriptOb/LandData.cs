using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "�µ�Ŀ�ĵ�", menuName = "��������/Ŀ�ĵ�")]

public class LandData : ScriptableObject
{
    [LabelText("��ͼID")]
    public int landID;
    [LabelText("��ͼ����")]
    public string landName;         //��ͼ��λ����
    [LabelText("��ͼ·��")]
    public string landAddress;         //��ͼ��λ����
    [LabelText("��ͼ����")]
    public float landDistance;      //��ͼ����
    [LabelText("Ŀ�ĵ�Ԥ�����б�")]
    public List<GameObject> prefabList = new List<GameObject>();
    [LabelText("��ͼ�¼�")]
    public EventData mapEvent;
}
