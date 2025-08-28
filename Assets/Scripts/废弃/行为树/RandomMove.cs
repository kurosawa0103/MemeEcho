using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class RandomMove : Action
{
    public float moveSpeed = 2f;  // 移动速度
    public float minX = -5f;      // 移动范围的最小X值
    public float maxX = 5f;       // 移动范围的最大X值

    public float minWaitTime = 1f;  // 最小等待时间
    public float maxWaitTime = 3f;  // 最大等待时间

    private Vector3 targetPosition; // 目标位置
    private bool isMoving;          // 是否在移动
    private Transform characterTransform; // 角色的Transform
    private CharacterState characterState;
    private float waitTimer;       // 等待计时器
    private float waitDuration;    // 需要等待的时间
    private bool isWaiting;        // 是否在等待
    public bool canInterrupted;    // 是否被打断

    public override void OnStart()
    {
        characterState = GetComponent<CharacterState>();
        characterState.ChangeState(CharacterState.CharState.Walk); // 等待结束后切换回 Walk 状态
        characterState.ChangeAction(CharacterState.CharAction.PatrolAction);
        characterTransform = gameObject.transform;  // 获取角色的Transform
        isWaiting = false;
        SetNewTargetPosition();  // 设置初始目标位置
    }

    public override TaskStatus OnUpdate()
    {
        // 如果被打断，立即返回失败
        if (canInterrupted && characterState.currentAction ==CharacterState.CharAction.ChatWaitAction || characterState.currentAction == CharacterState.CharAction.ChatAction)
        {
            return TaskStatus.Failure;
        }

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration)
            {
                isWaiting = false;
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }

        if (isMoving)
        {
            Vector3 direction = (targetPosition - characterTransform.localPosition).normalized;
            characterTransform.localPosition += direction * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(characterTransform.localPosition, targetPosition) < 0.1f)
            {
                StartWaiting(); // 开始等待
                return TaskStatus.Running; // 等待完成后继续
            }
        }

        return TaskStatus.Running;
    }

    private void SetNewTargetPosition()
    {
        float randomX = Random.Range(minX, maxX);
        targetPosition = new Vector3(randomX, characterTransform.localPosition.y, characterTransform.localPosition.z);

        if (randomX > characterTransform.localPosition.x)
        {
            characterTransform.localScale = new Vector3(Mathf.Abs(characterTransform.localScale.x), characterTransform.localScale.y, characterTransform.localScale.z);
        }
        else
        {
            characterTransform.localScale = new Vector3(-Mathf.Abs(characterTransform.localScale.x), characterTransform.localScale.y, characterTransform.localScale.z);
        }

        isMoving = true;
    }

    private void StartWaiting()
    {
        isMoving = false;
        isWaiting = true;
        characterState.ChangeState(CharacterState.CharState.Idle); // 切换为等待状态
        waitDuration = Random.Range(minWaitTime, maxWaitTime);
        waitTimer = 0f;
    }

    public override void OnReset()
    {
        isMoving = false;
        isWaiting = false;
    }

}
