using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class RandomMove : Action
{
    public float moveSpeed = 2f;  // �ƶ��ٶ�
    public float minX = -5f;      // �ƶ���Χ����СXֵ
    public float maxX = 5f;       // �ƶ���Χ�����Xֵ

    public float minWaitTime = 1f;  // ��С�ȴ�ʱ��
    public float maxWaitTime = 3f;  // ���ȴ�ʱ��

    private Vector3 targetPosition; // Ŀ��λ��
    private bool isMoving;          // �Ƿ����ƶ�
    private Transform characterTransform; // ��ɫ��Transform
    private CharacterState characterState;
    private float waitTimer;       // �ȴ���ʱ��
    private float waitDuration;    // ��Ҫ�ȴ���ʱ��
    private bool isWaiting;        // �Ƿ��ڵȴ�
    public bool canInterrupted;    // �Ƿ񱻴��

    public override void OnStart()
    {
        characterState = GetComponent<CharacterState>();
        characterState.ChangeState(CharacterState.CharState.Walk); // �ȴ��������л��� Walk ״̬
        characterState.ChangeAction(CharacterState.CharAction.PatrolAction);
        characterTransform = gameObject.transform;  // ��ȡ��ɫ��Transform
        isWaiting = false;
        SetNewTargetPosition();  // ���ó�ʼĿ��λ��
    }

    public override TaskStatus OnUpdate()
    {
        // �������ϣ���������ʧ��
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
                StartWaiting(); // ��ʼ�ȴ�
                return TaskStatus.Running; // �ȴ���ɺ����
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
        characterState.ChangeState(CharacterState.CharState.Idle); // �л�Ϊ�ȴ�״̬
        waitDuration = Random.Range(minWaitTime, maxWaitTime);
        waitTimer = 0f;
    }

    public override void OnReset()
    {
        isMoving = false;
        isWaiting = false;
    }

}
