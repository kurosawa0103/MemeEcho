using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class ReadBookAction : Action
{
    public float readDuration = 3f; // 读书持续时间
    public Vector3 targetPosition;  // 目标位置，可以在Inspector中配置
    public float moveSpeed = 3f;    // 物理移动速度

    private float timer;
    private bool isInterrupted = false;
    private bool hasArrivedAtTarget = false; // 标记是否到达目标位置
    private CharacterState characterState;
    private Transform characterTransform; // 使用Transform来操作localPosition

    public override void OnStart()
    {
        // 获取角色状态组件和Transform组件
        characterState = GetComponent<CharacterState>();
        characterTransform = gameObject.transform;  // 获取角色的Transform组件

        // 更改为行走状态
        characterState.ChangeAction(CharacterState.CharAction.ReadAction);
        characterState.ChangeState(CharacterState.CharState.Walk); // 设置为Walk状态（走向目标）

        // 重置计时器和标记
        timer = 0f;
        isInterrupted = false;
        hasArrivedAtTarget = false;

        Debug.Log("开始移动到目标...");
    }

    public override TaskStatus OnUpdate()
    {
        // 如果被打断，直接返回失败
        if (isInterrupted)
        {
            Debug.Log("读书被打断！");
            return TaskStatus.Failure;
        }

        // 如果目标位置没有到达，继续物理移动
        if (!hasArrivedAtTarget)
        {
            MoveToTarget();

            // 如果到达目标位置，切换为读书状态
            if (Vector3.Distance(characterTransform.localPosition, targetPosition) < 0.1f)
            {
                hasArrivedAtTarget = true;
                characterState.ChangeState(CharacterState.CharState.Read); // 切换到读书状态
                Debug.Log("到达目标位置，开始读书...");
            }

            return TaskStatus.Running;
        }

        // 如果已经到达目标，开始计时读书过程
        timer += Time.deltaTime;

        // 如果达到读书持续时间，返回成功
        if (timer >= readDuration)
        {
            Debug.Log("读书完成！");
            characterState.ChangeState(CharacterState.CharState.Idle); // 切换回Idle状态
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    // 移动到目标位置（使用localPosition）
    private void MoveToTarget()
    {
        Vector3 direction = (targetPosition - characterTransform.localPosition).normalized;
        characterTransform.localPosition += direction * moveSpeed * Time.deltaTime; // 使用localPosition移动

        // 移动时保持 Walk 状态
        if (characterState != null && characterState.currentState != CharacterState.CharState.Walk)
        {
            characterState.ChangeState(CharacterState.CharState.Walk); // 保持 Walk 状态
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
