using UnityEngine;
using Fungus;
using Sirenix.OdinInspector;

[CommandInfo("Character", "Change State", "�޸�TagΪ Player �Ľ�ɫ��״̬���ж�")]
public class ChangeCharacterState : Command
{
    [Header("��ɫ�Զ���ȡ (Tag = Player)")]
    [ReadOnly]
    public CharacterState targetCharacter; // Ŀ���ɫ

    [Header("�Ƿ��޸�״̬")]
    [LabelText("�޸�״̬")]
    public bool changeState; // �Ƿ��޸�״̬
    [ShowIf("changeState")]
    [LabelText("��״̬")]
    public CharacterState.CharState newState; // ѡ���µĽ�ɫ״̬

    [Header("�Ƿ��޸��ж�")]
    [LabelText("�޸��ж�")]
    public bool changeAction; // �Ƿ��޸��ж�
    [ShowIf("changeAction")]
    [LabelText("���ж�")]
    public CharacterState.CharAction newAction; // ѡ���µ��ж�

    public float fadeInTime=0.3f;
    public float fadeOutTime= 0.3f;
    public override void OnEnter()
    {
        // �Զ����� Tag Ϊ "Player" �Ľ�ɫ
        if (targetCharacter == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                targetCharacter = player.GetComponent<CharacterState>();
            }
        }

        if (targetCharacter == null)
        {
            Debug.LogWarning("ChangeCharacterState: �Ҳ������� Tag 'Player' ���� CharacterState ����Ķ���");
            Continue();
            return;
        }

        // �޸�״̬
        if (changeState)
        {
            targetCharacter.ChangeState(newState, fadeInTime,fadeOutTime);
        }

        // �޸��ж�
        if (changeAction)
        {
            targetCharacter.ChangeAction(newAction);
        }

        Continue(); // ����ִ����һ�� Fungus ����
    }

    public override string GetSummary()
    {
        string summary = "";

        // ���û�����ý�ɫ��������ʾ�����ʾ
        if (targetCharacter == null)
        {
            summary = "δ�ҵ� Player ��ɫ";
        }
        else
        {
            summary = targetCharacter.name;
        }

        // ������޸�״̬����ʾ״̬��Ϣ
        if (changeState)
        {
            summary += $" | ״̬: {newState}";
        }

        // ������޸��ж�����ʾ�ж���Ϣ
        if (changeAction)
        {
            summary += $" | �ж�: {newAction}";
        }

        return summary;
    }
}
