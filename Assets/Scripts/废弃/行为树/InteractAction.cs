using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class InteractAction : Action
{
    public Vector3 targetPosition;  // 目标位置
    public float moveSpeed = 3f;    // 移动速度
    public float interactDuration = 3f; // 互动持续时间

    private float timer;
    private bool isInterrupted = false;
    private bool hasArrivedAtTarget = false;
    private CharacterState characterState;
    private Transform characterTransform;

    public override void OnStart()
    {
        // 获取角色状态和 Transform 组件
        characterState = GetComponent<CharacterState>();
        characterTransform = gameObject.transform;

        // 设置为行走状态
        characterState.ChangeAction(CharacterState.CharAction.InteractAction);
        characterState.ChangeState(CharacterState.CharState.Walk);

        // 重置计时器和状态
        timer = 0f;
        isInterrupted = false;
        hasArrivedAtTarget = false;

        Debug.Log("开始移动到互动目标...");
    }

    public override TaskStatus OnUpdate()
    {
        // 如果被打断，直接返回失败
        if (isInterrupted)
        {
            Debug.Log("互动被打断！");
            return TaskStatus.Failure;
        }

        // 如果未到达目标，进行移动
        if (!hasArrivedAtTarget)
        {
            MoveToTarget();

            if (Vector3.Distance(characterTransform.localPosition, targetPosition) < 0.1f)
            {
                hasArrivedAtTarget = true;
                characterState.ChangeState(CharacterState.CharState.Read); // 切换到互动状态
                Debug.Log("到达目标位置，开始互动...");
            }

            return TaskStatus.Running;
        }

        // 计时互动过程
        timer += Time.deltaTime;

        if (timer >= interactDuration)
        {
            Debug.Log("互动完成！");
            characterState.ChangeState(CharacterState.CharState.Idle); // 回到Idle状态
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    // 移动到目标点
    private void MoveToTarget()
    {
        Vector3 direction = (targetPosition - characterTransform.localPosition).normalized;
        characterTransform.localPosition += direction * moveSpeed * Time.deltaTime;

        if (characterState.currentState != CharacterState.CharState.Walk)
        {
            characterState.ChangeState(CharacterState.CharState.Walk);
        }
    }

    // 触发打断
    public void Interrupt()
    {
        isInterrupted = true;
    }

    public override void OnReset()
    {
        timer = 0f;
        isInterrupted = false;
        hasArrivedAtTarget = false;
    }
}
