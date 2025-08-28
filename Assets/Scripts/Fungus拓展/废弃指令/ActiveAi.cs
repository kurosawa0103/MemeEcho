using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using BehaviorDesigner.Runtime; // Behavior Tree 所需的命名空间

[CommandInfo("AI", "Active AI", "启用或禁用目标对象上的行为树，并可选择切换 AI 行为树")]
public class ActiveAi : Command
{
    public enum TargetType
    {
        Player,  // 目标是场景中的 Player
        Custom   // 目标是手动指定的对象
    }

    [SerializeField] private bool enableAI = true; // 勾选则启用，取消则禁用
    [SerializeField] private TargetType targetType = TargetType.Player; // 选择目标类型
    [SerializeField] private GameObject customTarget; // 自定义目标（仅在 TargetType = Custom 时使用）

    [Header("AI 切换功能")]
    [SerializeField] private bool switchAI = false; // 是否切换 AI
    [SerializeField] private ExternalBehaviorTree newExternalBehaviorTree; // 新的外部 AI 行为树

    public override void OnEnter()
    {
        GameObject targetObject = null;

        if (targetType == TargetType.Player)
        {
            targetObject = GameObject.FindGameObjectWithTag("Player"); // 获取 Player
        }
        else if (targetType == TargetType.Custom && customTarget != null)
        {
            targetObject = customTarget;
        }

        if (targetObject != null)
        {
            // **获取 BehaviorTree 组件**
            BehaviorTree behaviorTree = targetObject.GetComponent<BehaviorTree>();

            if (behaviorTree != null)
            {
                if (switchAI && newExternalBehaviorTree != null)
                {
                    // 先禁用 BehaviorTree 避免错误
                    behaviorTree.enabled = false;

                    // 切换到新的 External Behavior
                    Debug.Log($"切换 AI: {behaviorTree.ExternalBehavior} -> {newExternalBehaviorTree}");
                    behaviorTree.ExternalBehavior = newExternalBehaviorTree;

                    // 重新启用 BehaviorTree，让新行为树生效
                    behaviorTree.enabled = true;
                }


                // **启用或禁用 AI**
                behaviorTree.enabled = enableAI;
            }
            else
            {
                Debug.LogWarning("目标对象上没有找到 BehaviorTree 组件！");
            }

            // **获取 CharacterState 组件**
            CharacterState characterState = targetObject.GetComponent<CharacterState>();

            if (characterState != null)
            {
                if (!enableAI)
                {
                    // **AI 关闭时，强制进入 Idle 状态**
                    characterState.ChangeState(CharacterState.CharState.Idle);
                }
            }
            else
            {
                Debug.LogWarning("目标对象上没有找到 CharacterState 组件！");
            }
        }
        else
        {
            Debug.LogWarning("未找到目标对象，请检查设置！");
        }

        Continue(); // 继续执行下一个命令
    }

    public override string GetSummary()
    {
        string targetName = targetType == TargetType.Player ? "Player" : (customTarget != null ? customTarget.name : "未指定");
        string aiStatus = enableAI ? "启用" : "禁用";
        string aiSwitchInfo = switchAI ? $"切换到 {newExternalBehaviorTree?.name}" : "保持当前 AI";

        return $"目标: {targetName} | AI状态: {aiStatus} | {aiSwitchInfo}";
    }
}
