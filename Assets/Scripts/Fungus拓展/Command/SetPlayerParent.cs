using UnityEngine;
using Fungus;

[CommandInfo("Custom", "Set Player Parent", "根据玩家是否上岸，设置其父级。")]
public class SetPlayerParent : Command
{
    [Tooltip("勾选后，玩家上岸（设为“临时父级”），否则设为“岛屿整体”")]
    public bool isPlayerLanding;

    [Tooltip("如果启用，则保持世界坐标不变")]
    public bool worldPositionStays = true;

    public override void OnEnter()
    {
        // 查找玩家物体（Tag 为 "Player"）
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("SetPlayerParent Command: 未找到 Player 物体！");
            Continue();
            return;
        }

        // 确定新的父级
        string parentName = isPlayerLanding ? "临时父级" : "岛屿整体";
        Transform newParent = GameObject.Find(parentName)?.transform;

        if (newParent == null)
        {
            Debug.LogWarning($"SetPlayerParent Command: 未找到 '{parentName}' 物体，无法设置父级！");
            Continue();
            return;
        }

        // 设置玩家的父级
        player.transform.SetParent(newParent, worldPositionStays);
        Debug.Log($"SetPlayerParent Command: {player.name} 现在是 {newParent.name} 的子集");

        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(77, 255, 183, 255); // 绿色
    }

    public override string GetSummary()
    {
        return $"Player → {(isPlayerLanding ? "临时父级" : "岛屿整体")}";
    }
}
