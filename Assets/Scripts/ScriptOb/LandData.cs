using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "新的目的地", menuName = "创建配置/目的地")]

public class LandData : ScriptableObject
{
    [LabelText("地图ID")]
    public int landID;
    [LabelText("地图名称")]
    public string landName;         //地图点位名称
    [LabelText("地图路径")]
    public string landAddress;         //地图点位名称
    [LabelText("地图距离")]
    public float landDistance;      //地图距离
    [LabelText("目的地预制体列表")]
    public List<GameObject> prefabList = new List<GameObject>();
    [LabelText("地图事件")]
    public EventData mapEvent;
}
