using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class ChatWaitAction : Action
{
    public float chatDuration = 5f; // �������ʱ��
    private float timer;           // ��ʱ��
    private CharacterState characterState;


    public override void OnStart()
    {
        characterState = GetComponent<CharacterState>();

        // �л�������״̬
        characterState.ChangeAction(CharacterState.CharAction.ChatWaitAction); // �����Ϊ����ʱ��Ϊ���춯����ռλ
        characterState.ChangeState(CharacterState.CharState.Idle); // ����������ʺϵ�����״̬������������EAT״̬��Ϊռλ

        timer = 0f;      // ���ü�ʱ��
        Debug.Log("��ʼ����,�ȴ�...");
    }

    public override TaskStatus OnUpdate()
    {
        // �������Ƿ������������죬���ǣ����ü�ʱ��
        if (characterState.currentAction == CharacterState.CharAction.ChatAction)
        {
            timer = 0f;  // ���������ʱ��
            Debug.Log("��Ͻ�������");
            return TaskStatus.Failure;
        }

        // ���������ʱ��
        timer += Time.deltaTime;

        // �������ʱ��ﵽ�趨ʱ�䣬���سɹ����л���Ϊ
        if (timer >= chatDuration)
        {
            Debug.Log("�ȴ���ɣ�û���죡");
            characterState.ChangeAction(CharacterState.CharAction.WaitAction); // �л�������״̬��������Idle״̬

            DialogManager dialogManager = GameObject.FindObjectOfType<DialogManager>();
            if (dialogManager != null && dialogManager.selectDialogData != null)
            {
                dialogManager.selectDialogData = null;
                Debug.Log("�Ի���ʱ��ȡ��ѡ��״̬��");
            }

            return TaskStatus.Success; // �Զ��ɹ�
        }

        return TaskStatus.Running; // ����������������
    }

   
    public override void OnReset()
    {
        timer = 0f;

    }

}
