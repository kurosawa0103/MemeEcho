using UnityEngine;
using Fungus;
using Sirenix.OdinInspector;

[CommandInfo("Character", "Change State", "修改Tag为 Player 的角色的状态或行动")]
public class ChangeCharacterState : Command
{
    [Header("角色自动获取 (Tag = Player)")]
    [ReadOnly]
    public CharacterState targetCharacter; // 目标角色

    [Header("是否修改状态")]
    [LabelText("修改状态")]
    public bool changeState; // 是否修改状态
    [ShowIf("changeState")]
    [LabelText("新状态")]
    public CharacterState.CharState newState; // 选择新的角色状态

    [Header("是否修改行动")]
    [LabelText("修改行动")]
    public bool changeAction; // 是否修改行动
    [ShowIf("changeAction")]
    [LabelText("新行动")]
    public CharacterState.CharAction newAction; // 选择新的行动

    public float fadeInTime=0.3f;
    public float fadeOutTime= 0.3f;
    public override void OnEnter()
    {
        // 自动查找 Tag 为 "Player" 的角色
        if (targetCharacter == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                targetCharacter = player.GetComponent<CharacterState>();
            }
        }

        if (targetCharacter == null)
        {
            Debug.LogWarning("ChangeCharacterState: 找不到带有 Tag 'Player' 且有 CharacterState 组件的对象！");
            Continue();
            return;
        }

        // 修改状态
        if (changeState)
        {
            targetCharacter.ChangeState(newState, fadeInTime,fadeOutTime);
        }

        // 修改行动
        if (changeAction)
        {
            targetCharacter.ChangeAction(newAction);
        }

        Continue(); // 继续执行下一个 Fungus 命令
    }

    public override string GetSummary()
    {
        string summary = "";

        // 如果没有配置角色，可以显示相关提示
        if (targetCharacter == null)
        {
            summary = "未找到 Player 角色";
        }
        else
        {
            summary = targetCharacter.name;
        }

        // 如果有修改状态，显示状态信息
        if (changeState)
        {
            summary += $" | 状态: {newState}";
        }

        // 如果有修改行动，显示行动信息
        if (changeAction)
        {
            summary += $" | 行动: {newAction}";
        }

        return summary;
    }
}
