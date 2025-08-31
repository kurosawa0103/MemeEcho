using UnityEngine;
using Fungus;

[CommandInfo("Player", "Toggle Player Movement", "启用或禁用玩家移动（自动查找 MovementSystem）")]
public class TogglePlayerMovement : Command
{
    [Tooltip("勾选 = 可以移动，取消勾选 = 禁止移动")]
    public bool canMove = true;

    private MovementSystem player;

    public override void OnEnter()
    {
        // 自动查找场景中的 MovementSystem
        if (player == null)
        {
            player = GameObject.FindObjectOfType<MovementSystem>();
        }

        if (player != null)
        {
            player.SetMovementEnabled(canMove);
        }
        else
        {
            Debug.LogWarning("TogglePlayerMovement: 场景中找不到 MovementSystem！");
        }

        Continue();
    }

    public override string GetSummary()
    {
        return (canMove ? "允许移动" : "禁止移动");
    }
}
