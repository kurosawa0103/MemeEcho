using UnityEngine;
using Fungus;
using System;
using System.Reflection;

[CommandInfo("Custom", "Set Script Active By Tag or Target", "����/����ָ������� Tag �¶����ĳ���ű�")]
public class SetScriptActiveByTag : Command
{
    [Tooltip("��ѡ��ֱ��ָ��Ŀ��������ȣ�")]
    public GameObject targetObject;

    [Tooltip("����� Tag����δָ�� targetObject ʱʹ�ã�")]
    public string tagName;

    [Tooltip("Ҫ����/���õĽű����������磺MyScript��")]
    public string scriptClassName;

    [Tooltip("�Ƿ����ýű�")]
    public bool enableScript = true;

    public override void OnEnter()
    {
        if (string.IsNullOrEmpty(scriptClassName))
        {
            Debug.LogWarning("[SetScriptActiveByTag] �ű�����Ϊ�գ�������ֹ");
            Continue();
            return;
        }

        // ��������
        var type = GetTypeByName(scriptClassName);
        if (type == null)
        {
            Debug.LogError($"[SetScriptActiveByTag] �Ҳ����� \"{scriptClassName}\"�������Ƿ�ƴд��ȷ");
            Continue();
            return;
        }

        int affectedCount = 0;

        if (targetObject != null)
        {
            // ֱ��������ָ������
            var comp = targetObject.GetComponent(type) as MonoBehaviour;
            if (comp != null)
            {
                comp.enabled = enableScript;
                affectedCount++;
            }
            else
            {
                Debug.LogWarning($"[SetScriptActiveByTag] �� {targetObject.name} ���Ҳ����ű� {scriptClassName}");
            }
        }
        else if (!string.IsNullOrEmpty(tagName))
        {
            // ����������ָ�� Tag �Ķ���
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tagName);
            foreach (GameObject obj in objects)
            {
                var comp = obj.GetComponent(type) as MonoBehaviour;
                if (comp != null)
                {
                    comp.enabled = enableScript;
                    affectedCount++;
                }
            }
        }
        else
        {
            Debug.LogWarning("[SetScriptActiveByTag] δָ��Ŀ������ Tag���޷�ִ�в���");
        }

        Debug.Log($"[SetScriptActiveByTag] �ű� {scriptClassName} ������Ϊ {(enableScript ? "����" : "����")}����Ӱ�� {affectedCount} ������");
        Continue();
    }

    public override string GetSummary()
    {
        string state = enableScript ? "����" : "����";
        if (targetObject != null)
            return $"���ö��� '{targetObject.name}' �Ľű� '{scriptClassName}' Ϊ {state}";
        else
            return $"���� Tag Ϊ '{tagName}' �Ķ���Ľű� '{scriptClassName}' Ϊ {state}";
    }

    public override Color GetButtonColor()
    {
        return new Color32(180, 255, 200, 255); // ����ɫ
    }

    private Type GetTypeByName(string className)
    {
        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = asm.GetType(className);
            if (type != null)
                return type;
        }
        return null;
    }
}
