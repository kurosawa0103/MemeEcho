using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class ChatAction : Action
{
    private CharacterState characterState;



    public override void OnStart()
    {
        characterState = GetComponent<CharacterState>();

        // 切换到聊天状态
        characterState.ChangeAction(CharacterState.CharAction.ChatAction); // 这个行为是暂时作为聊天动作的占位
        characterState.ChangeState(CharacterState.CharState.Idle); // 你可以设置适合的聊天状态动画，假设是EAT状态作为占位


        Debug.Log("开始聊天,等待...");
    }

    public override TaskStatus OnUpdate()
    {
        if (characterState.currentAction!=CharacterState.CharAction.ChatAction)
        {
            Debug.Log("聊天完成！");
            return TaskStatus.Success; // 自动成功
        }

        return TaskStatus.Running; // 继续运行聊天任务
    }

   
    public override void OnReset()
    {

    }

}
