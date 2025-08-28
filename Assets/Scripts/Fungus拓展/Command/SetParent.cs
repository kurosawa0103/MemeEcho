using UnityEngine;
using Fungus;

[CommandInfo("Custom", "Set Parent", "Sets the parent of a target GameObject.")]
public class SetParent : Command
{
    [Tooltip("The GameObject to change parent.")]
    public GameObject targetObject;

    [Tooltip("The new parent GameObject. Leave empty to set no parent (making it a root object).")]
    public Transform newParent;

    [Tooltip("If true, the target object will retain its world position after changing parent.")]
    public bool worldPositionStays = true;

    [Tooltip("If true, will override the configured parent and assign the object to the Island object (by tag).")]
    public bool overrideWithIsland = false;

    public override void OnEnter()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("SetParent Command: Target object is not assigned.");
            Continue();
            return;
        }

        Transform parentToSet = newParent;

        // ��ѡ���ϵ���ʱ��ǿ���� Island ��Ϊ����
        if (overrideWithIsland)
        {
            GameObject island = GameObject.FindGameObjectWithTag("Island");
            if (island != null)
            {
                parentToSet = island.transform;
            }
            else
            {
                Debug.LogWarning("SetParent Command: No object with tag 'Island' found.");
                parentToSet = null; // ���Ҳ�������Ϊ root
            }
        }

        // ���ø���
        targetObject.transform.SetParent(parentToSet, worldPositionStays);

        Debug.Log($"SetParent Command: {targetObject.name} is now a child of {(parentToSet != null ? parentToSet.name : "null (root)")}");

        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(255, 183, 77, 255); // ��ɫ
    }

    public override string GetSummary()
    {
        if (targetObject == null)
        {
            return "No target object set";
        }

        if (overrideWithIsland)
        {
            return $"{targetObject.name} �� Tag: Island";
        }

        string parentName = newParent != null ? newParent.name : "None (Root)";
        return $"{targetObject.name} �� {parentName}";
    }
}
