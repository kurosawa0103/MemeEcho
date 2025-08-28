using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;

[TaskCategory("Custom Tasks")]
public class CheckCharacterState : Conditional
{
    private CharacterState characterState; // 引用 BeetleState 组件
    public CharacterState.CharAction notThisAction; // 需要匹配的状态
    [LabelText("取反")]
    public bool invertCondition; // 取反条件的布尔值

    public override void OnAwake()
    {
        // 获取 BeetleState 组件
        characterState = GetComponent<CharacterState>();
        if (characterState == null)
        {
            Debug.LogError("未找到 characterState 组件，请确保该物体上有 characterState 组件。");
        }
    }

    public override TaskStatus OnUpdate()
    {
        // 根据 invertCondition 取反条件决定返回值
        bool isStateMatched = characterState.currentAction != notThisAction;

        if (invertCondition)
        {
            // 取反条件
            if (isStateMatched)
            {
                return TaskStatus.Failure;
            }
            else
            {
                return TaskStatus.Success;
            }
        }
        else
        {
            // 正常条件
            if (isStateMatched)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
    }
}
