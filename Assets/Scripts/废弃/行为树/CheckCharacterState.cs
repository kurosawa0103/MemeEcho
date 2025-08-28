using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;

[TaskCategory("Custom Tasks")]
public class CheckCharacterState : Conditional
{
    private CharacterState characterState; // ���� BeetleState ���
    public CharacterState.CharAction notThisAction; // ��Ҫƥ���״̬
    [LabelText("ȡ��")]
    public bool invertCondition; // ȡ�������Ĳ���ֵ

    public override void OnAwake()
    {
        // ��ȡ BeetleState ���
        characterState = GetComponent<CharacterState>();
        if (characterState == null)
        {
            Debug.LogError("δ�ҵ� characterState �������ȷ������������ characterState �����");
        }
    }

    public override TaskStatus OnUpdate()
    {
        // ���� invertCondition ȡ��������������ֵ
        bool isStateMatched = characterState.currentAction != notThisAction;

        if (invertCondition)
        {
            // ȡ������
            if (isStateMatched)
            {
                return TaskStatus.Failure;
            }
            else
            {
                return TaskStatus.Success;
            }
        }
        else
        {
            // ��������
            if (isStateMatched)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
    }
}
