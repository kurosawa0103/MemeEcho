using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class ReadBookAction : Action
{
    public float readDuration = 3f; // �������ʱ��
    public Vector3 targetPosition;  // Ŀ��λ�ã�������Inspector������
    public float moveSpeed = 3f;    // �����ƶ��ٶ�

    private float timer;
    private bool isInterrupted = false;
    private bool hasArrivedAtTarget = false; // ����Ƿ񵽴�Ŀ��λ��
    private CharacterState characterState;
    private Transform characterTransform; // ʹ��Transform������localPosition

    public override void OnStart()
    {
        // ��ȡ��ɫ״̬�����Transform���
        characterState = GetComponent<CharacterState>();
        characterTransform = gameObject.transform;  // ��ȡ��ɫ��Transform���

        // ����Ϊ����״̬
        characterState.ChangeAction(CharacterState.CharAction.ReadAction);
        characterState.ChangeState(CharacterState.CharState.Walk); // ����ΪWalk״̬������Ŀ�꣩

        // ���ü�ʱ���ͱ��
        timer = 0f;
        isInterrupted = false;
        hasArrivedAtTarget = false;

        Debug.Log("��ʼ�ƶ���Ŀ��...");
    }

    public override TaskStatus OnUpdate()
    {
        // �������ϣ�ֱ�ӷ���ʧ��
        if (isInterrupted)
        {
            Debug.Log("���鱻��ϣ�");
            return TaskStatus.Failure;
        }

        // ���Ŀ��λ��û�е�����������ƶ�
        if (!hasArrivedAtTarget)
        {
            MoveToTarget();

            // �������Ŀ��λ�ã��л�Ϊ����״̬
            if (Vector3.Distance(characterTransform.localPosition, targetPosition) < 0.1f)
            {
                hasArrivedAtTarget = true;
                characterState.ChangeState(CharacterState.CharState.Read); // �л�������״̬
                Debug.Log("����Ŀ��λ�ã���ʼ����...");
            }

            return TaskStatus.Running;
        }

        // ����Ѿ�����Ŀ�꣬��ʼ��ʱ�������
        timer += Time.deltaTime;

        // ����ﵽ�������ʱ�䣬���سɹ�
        if (timer >= readDuration)
        {
            Debug.Log("������ɣ�");
            characterState.ChangeState(CharacterState.CharState.Idle); // �л���Idle״̬
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    // �ƶ���Ŀ��λ�ã�ʹ��localPosition��
    private void MoveToTarget()
    {
        Vector3 direction = (targetPosition - characterTransform.localPosition).normalized;
        characterTransform.localPosition += direction * moveSpeed * Time.deltaTime; // ʹ��localPosition�ƶ�

        // �ƶ�ʱ���� Walk ״̬
        if (characterState != null && characterState.currentState != CharacterState.CharState.Walk)
        {
            characterState.ChangeState(CharacterState.CharState.Walk); // ���� Walk ״̬
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
