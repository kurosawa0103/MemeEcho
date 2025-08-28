using UnityEngine;
using Fungus;
using System.Collections.Generic;

[CommandInfo("角色检测", "CheckIsRecord", "检测指定角色的记录状态，支持全部满足(AND) 或 任意满足(OR) 的判断")]
public class CheckIsRecord : Command
{
    public enum CheckMode
    {
        AND, // AND 逻辑
        OR   // OR 逻辑
    }
    [Tooltip("检测模式：全部满足 or 任意满足")]
    public CheckMode checkMode = CheckMode.AND;

    [Tooltip("需要检测的角色列表")]
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
            return "<无目标角色>";

        string modeStr = checkMode == CheckMode.AND ? "全部满足" : "任意满足";
        string summary = $"模式: {modeStr} | 检测角色: ";

        foreach (var character in targetCharacters)
        {
            if (character != null)
                summary += character.name + ", ";
        }

        return summary.TrimEnd(',', ' ');
    }
}
