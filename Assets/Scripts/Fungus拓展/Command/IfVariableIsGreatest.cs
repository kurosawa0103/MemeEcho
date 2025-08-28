using UnityEngine;
using Fungus;
using System.Collections.Generic;

[CommandInfo("逻辑判断", "If Variable Name Is Greatest", "如果指定变量名称对应的变量值是最大，则继续执行，否则中断")]
public class IfVariableNameIsGreatest : Command
{
    [Tooltip("Flowchart中要比较的多个Int变量名")]
    public List<string> variableNamesToCompare = new List<string>();

    [Tooltip("要判断是否为最大值的变量名")]
    public string targetVariableName;

    public override void OnEnter()
    {
        Flowchart flowchart = GetFlowchart();

        if (flowchart == null || string.IsNullOrEmpty(targetVariableName) || variableNamesToCompare.Count == 0)
        {
            Debug.LogWarning("变量名设置不完整");
            Continue();
            return;
        }

        if (!flowchart.HasVariable(targetVariableName))
        {
            Debug.LogError($"找不到目标变量名: {targetVariableName}");
            Continue();
            return;
        }

        int targetValue = flowchart.GetIntegerVariable(targetVariableName);
        bool isGreatest = true;

        foreach (string varName in variableNamesToCompare)
        {
            if (string.IsNullOrEmpty(varName) || varName == targetVariableName)
                continue;

            if (!flowchart.HasVariable(varName))
            {
                Debug.LogWarning($"变量 {varName} 不存在，跳过");
                continue;
            }

            int val = flowchart.GetIntegerVariable(varName);
            if (val > targetValue)
            {
                isGreatest = false;
                break;
            }
        }

        if (isGreatest)
        {
            Continue(); // 是最大值，继续执行
        }
        else
        {
            Debug.Log($"变量 {targetVariableName} 不是最大值，终止执行");
            return; // 中断，不调用 Continue() 就会终止 Block
        }
    }

    public override string GetSummary()
    {
        string compareList = string.Join(", ", variableNamesToCompare);
        return $"判断变量 '{targetVariableName}' 是否为 [{compareList}] 中最大值";
    }
}
