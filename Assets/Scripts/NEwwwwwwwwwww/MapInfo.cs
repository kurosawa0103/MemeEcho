using UnityEngine;

[CreateAssetMenu(fileName = "NewMapInfo", menuName = "����/Map Info", order = 1)]
public class MapInfo : ScriptableObject
{
    [Header("��ͼ����")]
    public string mapName;

    [Header("��ͼ����")]
    [TextArea(8, 8)]
    public string mapDescription;

    [Header("��ť����")]
    public string buttonText;

    [Header("�л���������")]
    public string sceneName;

    [Header("memeê������")]
    public string memeMapName;
}
