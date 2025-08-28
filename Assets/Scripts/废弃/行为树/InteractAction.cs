using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class InteractAction : Action
{
    public Vector3 targetPosition;  // Ŀ��λ��
    public float moveSpeed = 3f;    // �ƶ��ٶ�
    public float interactDuration = 3f; // ��������ʱ��

    private float timer;
    private bool isInterrupted = false;
    private bool hasArrivedAtTarget = false;
    private CharacterState characterState;
    private Transform characterTransform;

    public override void OnStart()
    {
        // ��ȡ��ɫ״̬�� Transform ���
        characterState = GetComponent<CharacterState>();
        characterTransform = gameObject.transform;

        // ����Ϊ����״̬
        characterState.ChangeAction(CharacterState.CharAction.InteractAction);
        characterState.ChangeState(CharacterState.CharState.Walk);

        // ���ü�ʱ����״̬
        timer = 0f;
        isInterrupted = false;
        hasArrivedAtTarget = false;

        Debug.Log("��ʼ�ƶ�������Ŀ��...");
    }

    public override TaskStatus OnUpdate()
    {
        // �������ϣ�ֱ�ӷ���ʧ��
        if (isInterrupted)
        {
            Debug.Log("��������ϣ�");
            return TaskStatus.Failure;
        }

        // ���δ����Ŀ�꣬�����ƶ�
        if (!hasArrivedAtTarget)
        {
            MoveToTarget();

            if (Vector3.Distance(characterTransform.localPosition, targetPosition) < 0.1f)
            {
                hasArrivedAtTarget = true;
                characterState.ChangeState(CharacterState.CharState.Read); // �л�������״̬
                Debug.Log("����Ŀ��λ�ã���ʼ����...");
            }

            return TaskStatus.Running;
        }

        // ��ʱ��������
        timer += Time.deltaTime;

        if (timer >= interactDuration)
        {
            Debug.Log("������ɣ�");
            characterState.ChangeState(CharacterState.CharState.Idle); // �ص�Idle״̬
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    // �ƶ���Ŀ���
    private void MoveToTarget()
    {
        Vector3 direction = (targetPosition - characterTransform.localPosition).normalized;
        characterTransform.localPosition += direction * moveSpeed * Time.deltaTime;

        if (characterState.currentState != CharacterState.CharState.Walk)
        {
            characterState.ChangeState(CharacterState.CharState.Walk);
        }
    }

    // �������
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
