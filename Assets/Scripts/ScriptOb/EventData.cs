using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "新的事件", menuName = "创建配置/事件")]
public class EventData : ScriptableObject
{
    //public EventType eventType;


    [LabelText("事件路径")]
    public string eventAddress; // 事件路径

    [LabelText("事件预制体")]
    public GameObject eventPrefab; // 事件预制体

    //[LabelText("可触发时间段")]
    //public List<DayState> availableTimes; // 支持多个时间段


    //[LabelText("事件执行")]
    //public List<EventAction> eventActions = new List<EventAction>(); // 事件发生后执行的动作

    // 解锁条件的枚举
    public enum UnlockCondition
    {
        eventComplete,      // 事件完成
        currentMapPoint,      // 当前地图点位
        characterAttribute, // 角色属性值
    }
    public enum DiaryUnlockCondition
    {
        eventComplete,      // 事件完成
        CharacterAttribute, // 角色属性值
    }
    
    // 条件之间的逻辑关系
    public enum ConditionLogic
    {
        AND,  // 且
        OR,   // 或
    }
    public enum EventType
    {
        nomal,      
        special,
        land,
        wait
    }
    // 事件触发动作的枚举
    public enum EventActionType
    {
        ModifyCharacterStats, // 修改人物属性
        unlockSwitchButton  //解锁按钮
    }

    // **解锁条件组**
    [System.Serializable]
    public class UnlockConditionGroup
    {
        [LabelText("逻辑操作")]
        public ConditionLogic conditionLogic;

        [LabelText("条件列表")]
        public List<UnlockConditionDetail> conditions;

        public UnlockConditionGroup(ConditionLogic logic)
        {
            conditionLogic = logic;
            conditions = new List<UnlockConditionDetail>(); // 初始化条件列表
        }
    }

    // **解锁条件的详细信息**
    [System.Serializable]
    public class UnlockConditionDetail
    {
        [LabelText("解锁条件")]
        public UnlockCondition unlockCondition;

        [ShowIf("unlockCondition", UnlockCondition.eventComplete)]
        [LabelText("完成的事件")]
        public EventData isCompletedEvent; // 关联的事件
        [ShowIf("unlockCondition", UnlockCondition.eventComplete)]
        [LabelText("完成与否")]
        public bool isCompleted; // 关联的事件

        [ShowIf("unlockCondition", UnlockCondition.characterAttribute)]
        [LabelText("属性类型")]
        public CharacterManager.CharacterAttribute characterAttribute; // 角色属性类型

        [ShowIf("unlockCondition", UnlockCondition.characterAttribute)]
        [LabelText("属性值范围")]
        public Vector2 attributeRange; // 需要满足的属性值范围（最小值，最大值）

        [ShowIf("unlockCondition", UnlockCondition.currentMapPoint)]
        [LabelText("当前所在地图点位")]
        public LandData currentLand; 
    }

    // **日记记录条件组**
    [System.Serializable]
    public class DiaryUnlockConditionGroup
    {
        [LabelText("记录的日记文本"),TextArea (5,8)]
        public string diaryText; // 该组条件满足时记录的日记
        [LabelText("逻辑操作")]
        public ConditionLogic conditionLogic;

        [LabelText("条件列表")]
        public List<UnlockConditionDetail> conditions;

        public DiaryUnlockConditionGroup(ConditionLogic logic, string text)
        {
            conditionLogic = logic;
            conditions = new List<UnlockConditionDetail>();
            diaryText = text;
        }
    }

    // **事件发生时的动作**
    [System.Serializable]
    public class EventAction
    {
        [LabelText("触发类型")]
        public EventActionType eventActionType;

        [ShowIf("eventActionType", EventActionType.ModifyCharacterStats)]
        [LabelText("人物属性类型")]
        public CharacterManager.CharacterAttribute characterAttribute;

        [ShowIf("eventActionType", EventActionType.ModifyCharacterStats)]
        [LabelText("修改值")]
        public float attributeValue;
    }
}
