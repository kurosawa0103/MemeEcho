using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCaptureCondition", menuName = "PhotoSystem/Capture Condition")]
public class PhotoCaptureCondition : ScriptableObject
{
    public string conditionID;
    public int priority = 0; // ���������ȼ���Խ��Խ����
    public List<string> requiredObjectsName;
    [TextArea]
    public string description;

}
