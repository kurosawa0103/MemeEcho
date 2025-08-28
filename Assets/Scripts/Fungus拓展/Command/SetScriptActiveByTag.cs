using UnityEngine;
using Fungus;
using System;
using System.Reflection;

[CommandInfo("Custom", "Set Script Active By Tag or Target", "启用/禁用指定对象或 Tag 下对象的某个脚本")]
public class SetScriptActiveByTag : Command
{
    [Tooltip("可选：直接指定目标对象（优先）")]
    public GameObject targetObject;

    [Tooltip("对象的 Tag（当未指定 targetObject 时使用）")]
    public string tagName;

    [Tooltip("要启用/禁用的脚本类名（例如：MyScript）")]
    public string scriptClassName;

    [Tooltip("是否启用脚本")]
    public bool enableScript = true;

    public override void OnEnter()
    {
        if (string.IsNullOrEmpty(scriptClassName))
        {
            Debug.LogWarning("[SetScriptActiveByTag] 脚本类名为空，操作终止");
            Continue();
            return;
        }

        // 解析类型
        var type = GetTypeByName(scriptClassName);
        if (type == null)
        {
            Debug.LogError($"[SetScriptActiveByTag] 找不到类 \"{scriptClassName}\"，请检查是否拼写正确");
            Continue();
            return;
        }

        int affectedCount = 0;

        if (targetObject != null)
        {
            // 直接作用于指定对象
            var comp = targetObject.GetComponent(type) as MonoBehaviour;
            if (comp != null)
            {
                comp.enabled = enableScript;
                affectedCount++;
            }
            else
            {
                Debug.LogWarning($"[SetScriptActiveByTag] 在 {targetObject.name} 上找不到脚本 {scriptClassName}");
            }
        }
        else if (!string.IsNullOrEmpty(tagName))
        {
            // 批量作用于指定 Tag 的对象
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
            Debug.LogWarning("[SetScriptActiveByTag] 未指定目标对象或 Tag，无法执行操作");
        }

        Debug.Log($"[SetScriptActiveByTag] 脚本 {scriptClassName} 被设置为 {(enableScript ? "启用" : "禁用")}，共影响 {affectedCount} 个对象");
        Continue();
    }

    public override string GetSummary()
    {
        string state = enableScript ? "启用" : "禁用";
        if (targetObject != null)
            return $"设置对象 '{targetObject.name}' 的脚本 '{scriptClassName}' 为 {state}";
        else
            return $"设置 Tag 为 '{tagName}' 的对象的脚本 '{scriptClassName}' 为 {state}";
    }

    public override Color GetButtonColor()
    {
        return new Color32(180, 255, 200, 255); // 淡绿色
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
