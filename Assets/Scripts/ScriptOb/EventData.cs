using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "�µ��¼�", menuName = "��������/�¼�")]
public class EventData : ScriptableObject
{
    //public EventType eventType;


    [LabelText("�¼�·��")]
    public string eventAddress; // �¼�·��

    [LabelText("�¼�Ԥ����")]
    public GameObject eventPrefab; // �¼�Ԥ����

    //[LabelText("�ɴ���ʱ���")]
    //public List<DayState> availableTimes; // ֧�ֶ��ʱ���


    //[LabelText("�¼�ִ��")]
    //public List<EventAction> eventActions = new List<EventAction>(); // �¼�������ִ�еĶ���

    // ����������ö��
    public enum UnlockCondition
    {
        eventComplete,      // �¼����
        currentMapPoint,      // ��ǰ��ͼ��λ
        characterAttribute, // ��ɫ����ֵ
    }
    public enum DiaryUnlockCondition
    {
        eventComplete,      // �¼����
        CharacterAttribute, // ��ɫ����ֵ
    }
    
    // ����֮����߼���ϵ
    public enum ConditionLogic
    {
        AND,  // ��
        OR,   // ��
    }
    public enum EventType
    {
        nomal,      
        special,
        land,
        wait
    }
    // �¼�����������ö��
    public enum EventActionType
    {
        ModifyCharacterStats, // �޸���������
        unlockSwitchButton  //������ť
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

        [ShowIf("unlockCondition", UnlockCondition.eventComplete)]
        [LabelText("��ɵ��¼�")]
        public EventData isCompletedEvent; // �������¼�
        [ShowIf("unlockCondition", UnlockCondition.eventComplete)]
        [LabelText("������")]
        public bool isCompleted; // �������¼�

        [ShowIf("unlockCondition", UnlockCondition.characterAttribute)]
        [LabelText("��������")]
        public CharacterManager.CharacterAttribute characterAttribute; // ��ɫ��������

        [ShowIf("unlockCondition", UnlockCondition.characterAttribute)]
        [LabelText("����ֵ��Χ")]
        public Vector2 attributeRange; // ��Ҫ���������ֵ��Χ����Сֵ�����ֵ��

        [ShowIf("unlockCondition", UnlockCondition.currentMapPoint)]
        [LabelText("��ǰ���ڵ�ͼ��λ")]
        public LandData currentLand; 
    }

    // **�ռǼ�¼������**
    [System.Serializable]
    public class DiaryUnlockConditionGroup
    {
        [LabelText("��¼���ռ��ı�"),TextArea (5,8)]
        public string diaryText; // ������������ʱ��¼���ռ�
        [LabelText("�߼�����")]
        public ConditionLogic conditionLogic;

        [LabelText("�����б�")]
        public List<UnlockConditionDetail> conditions;

        public DiaryUnlockConditionGroup(ConditionLogic logic, string text)
        {
            conditionLogic = logic;
            conditions = new List<UnlockConditionDetail>();
            diaryText = text;
        }
    }

    // **�¼�����ʱ�Ķ���**
    [System.Serializable]
    public class EventAction
    {
        [LabelText("��������")]
        public EventActionType eventActionType;

        [ShowIf("eventActionType", EventActionType.ModifyCharacterStats)]
        [LabelText("������������")]
        public CharacterManager.CharacterAttribute characterAttribute;

        [ShowIf("eventActionType", EventActionType.ModifyCharacterStats)]
        [LabelText("�޸�ֵ")]
        public float attributeValue;
    }
}
