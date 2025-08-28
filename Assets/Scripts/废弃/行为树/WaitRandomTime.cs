using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class WaitRandomTime : Action
{
    public float minWaitTime = 1f;   // ��С�ȴ�ʱ��
    public float maxWaitTime = 3f;   // ���ȴ�ʱ��

    private float waitTime;          // ��ǰ�ȴ���ʱ��
    private float timer;             // ��ʱ��
    private bool isWaiting;          // �Ƿ��ڵȴ�״̬
    public bool canInterrupted;      // �Ƿ񱻴��
    private CharacterState characterState;

    public override void OnStart()
    {
        // ��ɫ�л��� Idle ״̬
        characterState = GetComponent<CharacterState>();
        characterState.ChangeState(CharacterState.CharState.Idle);
        characterState.ChangeAction(CharacterState.CharAction.WaitAction);

        // ��������ȴ�ʱ��
        waitTime = Random.Range(minWaitTime, maxWaitTime);
        timer = 0f;
        isWaiting = true;
    }

    public override TaskStatus OnUpdate()
    {
        // **�������ϣ��������� Failure**
        if (canInterrupted && characterState.currentAction == CharacterState.CharAction.ChatWaitAction|| characterState.currentAction == CharacterState.CharAction.ChatAction)
        {
            return TaskStatus.Failure;
        }

        // **�����ȴ��߼�**
        if (isWaiting)
        {
            timer += Time.deltaTime;

            // **�ȴ�ʱ�䵽�ˣ������ȴ�**
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
