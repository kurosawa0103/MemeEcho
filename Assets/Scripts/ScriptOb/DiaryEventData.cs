using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "�µ���־�¼�", menuName = "��������/��־�¼�")]
public class DiaryEventData : ScriptableObject
{
    [LabelText("�¼�����")]
    public string diaryEventName; // �¼�����

    [LabelText("�¼�·��")]
    public string diaryEventAddress; // �¼�·��

    [LabelText("�¼�Ԥ����")]
    public GameObject diaryEventPrefab; // �¼�Ԥ����

    public float requiredLockTime;


}
