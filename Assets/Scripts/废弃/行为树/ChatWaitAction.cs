using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class ChatWaitAction : Action
{
    public float chatDuration = 5f; // 聊天持续时间
    private float timer;           // 计时器
    private CharacterState characterState;


    public override void OnStart()
    {
        characterState = GetComponent<CharacterState>();

        // 切换到聊天状态
        characterState.ChangeAction(CharacterState.CharAction.ChatWaitAction); // 这个行为是暂时作为聊天动作的占位
        characterState.ChangeState(CharacterState.CharState.Idle); // 你可以设置适合的聊天状态动画，假设是EAT状态作为占位

        timer = 0f;      // 重置计时器
        Debug.Log("开始聊天,等待...");
    }

    public override TaskStatus OnUpdate()
    {
        // 检查玩家是否正在输入聊天，若是，重置计时器
        if (characterState.currentAction == CharacterState.CharAction.ChatAction)
        {
            timer = 0f;  // 重置聊天计时器
            Debug.Log("打断进入聊天");
            return TaskStatus.Failure;
        }

        // 更新聊天计时器
        timer += Time.deltaTime;

        // 如果聊天时间达到设定时间，返回成功并切换行为
        if (timer >= chatDuration)
        {
            Debug.Log("等待完成，没聊天！");
            characterState.ChangeAction(CharacterState.CharAction.WaitAction); // 切换到其他状态，假设是Idle状态

            DialogManager dialogManager = GameObject.FindObjectOfType<DialogManager>();
            if (dialogManager != null && dialogManager.selectDialogData != null)
            {
                dialogManager.selectDialogData = null;
                Debug.Log("对话超时，取消选中状态！");
            }

            return TaskStatus.Success; // 自动成功
        }

        return TaskStatus.Running; // 继续运行聊天任务
    }

   
    public override void OnReset()
    {
        timer = 0f;

    }

}
