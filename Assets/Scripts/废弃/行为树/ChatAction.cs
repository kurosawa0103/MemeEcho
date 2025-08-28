using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom Tasks")]
public class ChatAction : Action
{
    private CharacterState characterState;



    public override void OnStart()
    {
        characterState = GetComponent<CharacterState>();

        // �л�������״̬
        characterState.ChangeAction(CharacterState.CharAction.ChatAction); // �����Ϊ����ʱ��Ϊ���춯����ռλ
        characterState.ChangeState(CharacterState.CharState.Idle); // ����������ʺϵ�����״̬������������EAT״̬��Ϊռλ


        Debug.Log("��ʼ����,�ȴ�...");
    }

    public override TaskStatus OnUpdate()
    {
        if (characterState.currentAction!=CharacterState.CharAction.ChatAction)
        {
            Debug.Log("������ɣ�");
            return TaskStatus.Success; // �Զ��ɹ�
        }

        return TaskStatus.Running; // ����������������
    }

   
    public override void OnReset()
    {

    }

}
