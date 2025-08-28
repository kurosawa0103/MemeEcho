using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCaptureCondition", menuName = "PhotoSystem/Capture Condition")]
public class PhotoCaptureCondition : ScriptableObject
{
    public string conditionID;
    public int priority = 0; // 新增：优先级，越高越优先
    public List<string> requiredObjectsName;
    [TextArea]
    public string description;

}
