using UnityEngine;
using Fungus;

[CommandInfo("Custom",
             "Set Active By Tag",
             "Set active state of GameObjects with a specified Tag.")]
public class SetActiveByTag : Command
{
    [Tooltip("The tag of the GameObjects you want to modify.")]
    public string targetTag = "Untagged";

    [Tooltip("Whether to set the objects active (true) or inactive (false).")]
    public bool setActiveState = true;

    [Tooltip("If true, only affect the first object found with the tag.")]
    public bool onlyAffectFirst = false;

    public override void OnEnter()
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            Debug.LogWarning("SetActiveByTag: targetTag is empty!");
            Continue();
            return;
        }

        if (onlyAffectFirst)
        {
            GameObject obj = GameObject.FindGameObjectWithTag(targetTag);
            if (obj != null)
            {
                obj.SetActive(setActiveState);
            }
            else
            {
                Debug.LogWarning($"SetActiveByTag: No object found with tag '{targetTag}'!");
            }
        }
        else
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(targetTag);
            if (objs.Length == 0)
            {
                Debug.LogWarning($"SetActiveByTag: No objects found with tag '{targetTag}'!");
            }
            foreach (GameObject obj in objs)
            {
                obj.SetActive(setActiveState);
            }
        }

        Continue();
    }

    public override string GetSummary()
    {
        return $"将标签为 '{targetTag}' 的物体 {(onlyAffectFirst ? "（仅一个）" : "（全部）")} 设置为 {(setActiveState ? "激活" : "禁用")}";
    }
}
