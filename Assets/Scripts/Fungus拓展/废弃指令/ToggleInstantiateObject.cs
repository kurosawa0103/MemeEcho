using UnityEngine;
using Fungus;

[CommandInfo("Custom",
             "Toggle Instantiate Object",
             "If checked, instantiate prefab under the given parent. Otherwise, destroy the object.")]
public class ToggleInstantiateObject : Command
{
    [Tooltip("If true, instantiate the prefab. If false, destroy the object.")]
    public bool shouldInstantiate;

    [Tooltip("Prefab to instantiate when checked.")]
    public GameObject prefab;

    [Tooltip("The parent transform under which to instantiate or destroy. Leave empty to auto-find by tag 'LandParent'.")]
    public Transform targetParent;

    [Tooltip("Tag")]
    public string parentTag= "LandParent";

    [Tooltip("The name of the instantiated object (used for finding and destroying it).")]
    public string objectName = "CustomInstance";

    [Tooltip("If true, clear all children under the parent before instantiating.")]
    public bool clearBeforeInstantiate = false;


    public override void OnEnter()
    {
        // ���û�����ø�������Ѱ�� tag Ϊ LandParent ������
        if (targetParent == null)
        {
            GameObject taggedParent = GameObject.FindGameObjectWithTag(parentTag);
            if (taggedParent != null)
            {
                targetParent = taggedParent.transform;
            }
            else
            {
                Debug.LogWarning("�Ҳ������б�ǩ 'LandParent' �����壡");
                Continue();
                return;
            }
        }

        if (shouldInstantiate)
        {
            // �������������
            if (clearBeforeInstantiate)
            {
                foreach (Transform child in targetParent)
                {
                    Destroy(child.gameObject);
                }
            }

            // �����µĶ���
            Transform existing = targetParent.Find(objectName);
            if (existing == null && prefab != null)
            {
                GameObject go = Instantiate(prefab, targetParent);
                go.name = objectName;
            }
        }
        else
        {
            // ɾ��ָ�����ƵĶ���
            Transform toDestroy = targetParent.Find(objectName);
            if (toDestroy != null)
            {
                Destroy(toDestroy.gameObject);
            }
        }
        
        Continue();
    }

    public override string GetSummary()
    {
        string parentName = targetParent ? targetParent.name : "[Auto by Tag]";
        return shouldInstantiate
            ? $"Instantiate '{objectName}' under {parentName}" + (clearBeforeInstantiate ? " (Cleared First)" : "")
            : $"Destroy '{objectName}' under {parentName}";
    }
}
