using UnityEngine;
using Fungus;
using System.Collections.Generic;

[CommandInfo("��ɫ���", "CheckIsRecord", "���ָ����ɫ�ļ�¼״̬��֧��ȫ������(AND) �� ��������(OR) ���ж�")]
public class CheckIsRecord : Command
{
    public enum CheckMode
    {
        AND, // AND �߼�
        OR   // OR �߼�
    }
    [Tooltip("���ģʽ��ȫ������ or ��������")]
    public CheckMode checkMode = CheckMode.AND;

    [Tooltip("��Ҫ���Ľ�ɫ�б�")]
    public List<CharacterObject> targetCharacters = new List<CharacterObject>();

    public override void OnEnter()
    {
        StartCoroutine(CheckUntilSatisfied());
    }

    private System.Collections.IEnumerator CheckUntilSatisfied()
    {
        while (!CheckCondition())
        {
            yield return null;
        }

        Continue();
    }

    private bool CheckCondition()
    {
        if (targetCharacters.Count == 0) return false;

        switch (checkMode)
        {
            case CheckMode.AND:
                foreach (var character in targetCharacters)
                {
                    if (character == null) continue;
                    if (character.needRecord && !character.hasRecorded)
                    {
                        return false;
                    }
                }
                return true;

            case CheckMode.OR:
                foreach (var character in targetCharacters)
                {
                    if (character == null) continue;
                    if (character.needRecord && character.hasRecorded)
                    {
                        return true;
                    }
                }
                return false;

            default:
                return false;
        }
    }
    public override Color GetButtonColor()
    {
        return new Color32(200, 100, 150, 255);
    }
    public override string GetSummary()
    {
        if (targetCharacters == null || targetCharacters.Count == 0)
            return "<��Ŀ���ɫ>";

        string modeStr = checkMode == CheckMode.AND ? "ȫ������" : "��������";
        string summary = $"ģʽ: {modeStr} | ����ɫ: ";

        foreach (var character in targetCharacters)
        {
            if (character != null)
                summary += character.name + ", ";
        }

        return summary.TrimEnd(',', ' ');
    }
}
