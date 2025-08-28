using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "�µĶԻ�", menuName = "��������/�Ի�")]
public class DialogData : ScriptableObject
{
    [LabelText("�Ի��ı�"),TextArea(3,8)]
    public string dialogText; // �¼�����

    [LabelText("�Ի�·��")]
    public string dialogAddress; // �¼�·��

    public Vector3 dialogOffset; // ����ƫ����

    [LabelText("�ɴ���ʱ���")]
    public List<DayState> availableTimes; // ֧�ֶ��ʱ���

    [LabelText("��������")]
    public List<UnlockConditionGroup> unlockConditionGroups = new List<UnlockConditionGroup>(); // �¼���������

    [LabelText("�Ի���ִ��")]
    public List<DialogAction> dialogActions = new List<DialogAction>(); // �¼�������ִ�еĶ���

    [LabelText("�ظ�ѡ��")]
    public List<DialogReply> dialogReplies = new List<DialogReply>(); // ��ѡ�Ļظ�

    // **�ظ�ѡ��**
    [System.Serializable]
    public class DialogReply
    {
        [LabelText("�ظ�����")]
        public DialogData replydata; // ��ҿ�ѡ�Ļظ�

        [LabelText("�ȴ�ʱ�䣨�룩")]
        public float waitTime = 1.5f; // Ĭ�ϵȴ�1.5��

        [LabelText("��ת�Ի�")]
        public DialogData nextReply; // ѡ��ûظ�����ת�ĶԻ�
    }

    // ����������ö��
    public enum UnlockCondition
    {
        eventState,
        dialogComplete,      // �Ի����
        eventComplete,      // �¼����
        characterAttribute, // ��ɫ����ֵ
    }
    
    // ����֮����߼���ϵ
    public enum ConditionLogic
    {
        AND,  // ��
        OR,   // ��
    }
    // �¼�����������ö��
    public enum DialogActionType
    {
        ModifyCharacterStats, // �޸���������
        UpdateEventState,  //�޸�״̬
        ChangeCharacterState
    }

    // **����������**
    [System.Serializable]
    public class UnlockConditionGroup
    {
        [LabelText("�߼�����")]
        public ConditionLogic conditionLogic;

        [LabelText("�����б�")]
        public List<UnlockConditionDetail> conditions;

        public UnlockConditionGroup(ConditionLogic logic)
        {
            conditionLogic = logic;
            conditions = new List<UnlockConditionDetail>(); // ��ʼ�������б�
        }
    }

    // **������������ϸ��Ϣ**
    [System.Serializable]
    public class UnlockConditionDetail
    {
        [LabelText("��������")]
        public UnlockCondition unlockCondition;
        [ShowIf("unlockCondition", UnlockCondition.eventState)]
        [LabelText("�¼�״̬��")]
        public string eventStateName; // �����ĶԻ�
        [ShowIf("unlockCondition", UnlockCondition.eventState)]
        [LabelText("�¼�״ֵ̬")]
        public string eventStateValue; // �����ĶԻ�

        [ShowIf("unlockCondition", UnlockCondition.dialogComplete)]
        [LabelText("��ɵĶԻ�")]
        public DialogData completedDialog; // �����ĶԻ�
        [ShowIf("unlockCondition", UnlockCondition.dialogComplete)]
        [LabelText("������")]
        public bool isDialogCompleted; // �����ĶԻ�

        [ShowIf("unlockCondition", UnlockCondition.eventComplete)]
        [LabelText("��ɵ��¼�")]
        public EventData completedEvent; // �������¼�
        [ShowIf("unlockCondition", UnlockCondition.eventComplete)]
        [LabelText("������")]
        public bool isEventCompleted; // �������¼�

        [ShowIf("unlockCondition", UnlockCondition.characterAttribute)]
        [LabelText("��������")]
        public CharacterManager.CharacterAttribute characterAttribute; // ��ɫ��������

        [ShowIf("unlockCondition", UnlockCondition.characterAttribute)]
        [LabelText("����ֵ��Χ")]
        public Vector2 attributeRange; // ��Ҫ���������ֵ��Χ����Сֵ�����ֵ��

    }

    // **�¼�����ʱ�Ķ���**
    [System.Serializable]
    public class DialogAction
    {
        [LabelText("��������")]
        public DialogActionType dialogActionType;

        [ShowIf("dialogActionType", DialogActionType.ModifyCharacterStats)]
        [LabelText("������������")]
        public CharacterManager.CharacterAttribute characterAttribute;

        [ShowIf("dialogActionType", DialogActionType.ModifyCharacterStats)]
        [LabelText("�޸�ֵ")]
        public float attributeValue;

        [ShowIf("dialogActionType", DialogActionType.UpdateEventState)]
        [LabelText("״̬")]
        public string eventName;

        [ShowIf("dialogActionType", DialogActionType.UpdateEventState)]
        [LabelText("�޸�ֵ")]
        public string eventKey;

        [ShowIf("dialogActionType", DialogActionType.ChangeCharacterState)]
        [LabelText("Ŀ��action")]
        public CharacterState.CharAction newAction; // ��ɫ����״̬
    }
}
