using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class WaitRandomTime : Action
{
    public float minWaitTime = 1f;   // 最小等待时间
    public float maxWaitTime = 3f;   // 最大等待时间

    private float waitTime;          // 当前等待的时间
    private float timer;             // 计时器
    private bool isWaiting;          // 是否处于等待状态
    public bool canInterrupted;      // 是否被打断
    private CharacterState characterState;

    public override void OnStart()
    {
        // 角色切换到 Idle 状态
        characterState = GetComponent<CharacterState>();
        characterState.ChangeState(CharacterState.CharState.Idle);
        characterState.ChangeAction(CharacterState.CharAction.WaitAction);

        // 生成随机等待时间
        waitTime = Random.Range(minWaitTime, maxWaitTime);
        timer = 0f;
        isWaiting = true;
    }

    public override TaskStatus OnUpdate()
    {
        // **如果被打断，立即返回 Failure**
        if (canInterrupted && characterState.currentAction == CharacterState.CharAction.ChatWaitAction|| characterState.currentAction == CharacterState.CharAction.ChatAction)
        {
            return TaskStatus.Failure;
        }

        // **正常等待逻辑**
        if (isWaiting)
        {
            timer += Time.deltaTime;

            // **等待时间到了，结束等待**
            if (timer >= waitTime)
            {
                isWaiting = false;
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Running;
    }

    public override void OnReset()
    {
        timer = 0f;
        isWaiting = false;
    }

}
