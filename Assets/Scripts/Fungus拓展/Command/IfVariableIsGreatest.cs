using UnityEngine;
using Fungus;
using System.Collections.Generic;

[CommandInfo("�߼��ж�", "If Variable Name Is Greatest", "���ָ���������ƶ�Ӧ�ı���ֵ����������ִ�У������ж�")]
public class IfVariableNameIsGreatest : Command
{
    [Tooltip("Flowchart��Ҫ�ȽϵĶ��Int������")]
    public List<string> variableNamesToCompare = new List<string>();

    [Tooltip("Ҫ�ж��Ƿ�Ϊ���ֵ�ı�����")]
    public string targetVariableName;

    public override void OnEnter()
    {
        Flowchart flowchart = GetFlowchart();

        if (flowchart == null || string.IsNullOrEmpty(targetVariableName) || variableNamesToCompare.Count == 0)
        {
            Debug.LogWarning("���������ò�����");
            Continue();
            return;
        }

        if (!flowchart.HasVariable(targetVariableName))
        {
            Debug.LogError($"�Ҳ���Ŀ�������: {targetVariableName}");
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
                Debug.LogWarning($"���� {varName} �����ڣ�����");
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
            Continue(); // �����ֵ������ִ��
        }
        else
        {
            Debug.Log($"���� {targetVariableName} �������ֵ����ִֹ��");
            return; // �жϣ������� Continue() �ͻ���ֹ Block
        }
    }

    public override string GetSummary()
    {
        string compareList = string.Join(", ", variableNamesToCompare);
        return $"�жϱ��� '{targetVariableName}' �Ƿ�Ϊ [{compareList}] �����ֵ";
    }
}
