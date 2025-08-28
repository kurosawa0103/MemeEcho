using UnityEngine;
using Sirenix.OdinInspector; // ����Odin Inspector�����ռ�

[CreateAssetMenu(fileName = "PageDataConfig", menuName = "����/PageDataConfig", order = 1)]
public class Page : ScriptableObject
{
    [Header("ҳ�������б�")]
    public PageData[] pages; // �������ҳ������
}

[System.Serializable]
public class PageData
{
    public GameObject prefab; // ҳ��Ԥ����
    public bool isLocked; // �Ƿ����

    [ShowIf("isLocked", true)] // ��isUnlockedδ��ѡʱ��ʾ
    public string unlockEvent; // ҳ������

    [ShowIf("isLocked", true)] // ��isUnlockedδ��ѡʱ��ʾ
    public string unlockEventKey; // �����������������ĳ������
}
