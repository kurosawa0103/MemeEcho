using UnityEngine;

[CreateAssetMenu(fileName = "NewMapInfo", menuName = "创建/Map Info", order = 1)]
public class MapInfo : ScriptableObject
{
    [Header("地图名称")]
    public string mapName;

    [Header("地图描述")]
    [TextArea(8, 8)]
    public string mapDescription;

    [Header("按钮文字")]
    public string buttonText;

    [Header("切换场景名称")]
    public string sceneName;

    [Header("meme锚点名称")]
    public string memeMapName;
}
