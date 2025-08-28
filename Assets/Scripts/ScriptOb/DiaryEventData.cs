using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "新的日志事件", menuName = "创建配置/日志事件")]
public class DiaryEventData : ScriptableObject
{
    [LabelText("事件名称")]
    public string diaryEventName; // 事件名称

    [LabelText("事件路径")]
    public string diaryEventAddress; // 事件路径

    [LabelText("事件预制体")]
    public GameObject diaryEventPrefab; // 事件预制体

    public float requiredLockTime;


}
