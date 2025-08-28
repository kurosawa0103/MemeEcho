using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "新的对话", menuName = "创建配置/对话")]
public class DialogData : ScriptableObject
{
    [LabelText("对话文本"),TextArea(3,8)]
    public string dialogText; // 事件名称

    [LabelText("对话路径")]
    public string dialogAddress; // 事件路径

    public Vector3 dialogOffset; // 新增偏移量

    [LabelText("可触发时间段")]
    public List<DayState> availableTimes; // 支持多个时间段

    [LabelText("解锁条件")]
    public List<UnlockConditionGroup> unlockConditionGroups = new List<UnlockConditionGroup>(); // 事件解锁条件

    [LabelText("对话后执行")]
    public List<DialogAction> dialogActions = new List<DialogAction>(); // 事件发生后执行的动作

    [LabelText("回复选项")]
    public List<DialogReply> dialogReplies = new List<DialogReply>(); // 可选的回复

    // **回复选项**
    [System.Serializable]
    public class DialogReply
    {
        [LabelText("回复配置")]
        public DialogData replydata; // 玩家可选的回复

        [LabelText("等待时间（秒）")]
        public float waitTime = 1.5f; // 默认等待1.5秒

        [LabelText("跳转对话")]
        public DialogData nextReply; // 选择该回复后，跳转的对话
    }

    // 解锁条件的枚举
    public enum UnlockCondition
    {
        eventState,
        dialogComplete,      // 对话完成
        eventComplete,      // 事件完成
        characterAttribute, // 角色属性值
    }
    
    // 条件之间的逻辑关系
    public enum ConditionLogic
    {
        AND,  // 且
        OR,   // 或
    }
    // 事件触发动作的枚举
    public enum DialogActionType
    {
        ModifyCharacterStats, // 修改人物属性
        UpdateEventState,  //修改状态
        ChangeCharacterState
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
        [ShowIf("unlockCondition", UnlockCondition.eventState)]
        [LabelText("事件状态名")]
        public string eventStateName; // 关联的对话
        [ShowIf("unlockCondition", UnlockCondition.eventState)]
        [LabelText("事件状态值")]
        public string eventStateValue; // 关联的对话

        [ShowIf("unlockCondition", UnlockCondition.dialogComplete)]
        [LabelText("完成的对话")]
        public DialogData completedDialog; // 关联的对话
        [ShowIf("unlockCondition", UnlockCondition.dialogComplete)]
        [LabelText("完成与否")]
        public bool isDialogCompleted; // 关联的对话

        [ShowIf("unlockCondition", UnlockCondition.eventComplete)]
        [LabelText("完成的事件")]
        public EventData completedEvent; // 关联的事件
        [ShowIf("unlockCondition", UnlockCondition.eventComplete)]
        [LabelText("完成与否")]
        public bool isEventCompleted; // 关联的事件

        [ShowIf("unlockCondition", UnlockCondition.characterAttribute)]
        [LabelText("属性类型")]
        public CharacterManager.CharacterAttribute characterAttribute; // 角色属性类型

        [ShowIf("unlockCondition", UnlockCondition.characterAttribute)]
        [LabelText("属性值范围")]
        public Vector2 attributeRange; // 需要满足的属性值范围（最小值，最大值）

    }

    // **事件发生时的动作**
    [System.Serializable]
    public class DialogAction
    {
        [LabelText("触发类型")]
        public DialogActionType dialogActionType;

        [ShowIf("dialogActionType", DialogActionType.ModifyCharacterStats)]
        [LabelText("人物属性类型")]
        public CharacterManager.CharacterAttribute characterAttribute;

        [ShowIf("dialogActionType", DialogActionType.ModifyCharacterStats)]
        [LabelText("修改值")]
        public float attributeValue;

        [ShowIf("dialogActionType", DialogActionType.UpdateEventState)]
        [LabelText("状态")]
        public string eventName;

        [ShowIf("dialogActionType", DialogActionType.UpdateEventState)]
        [LabelText("修改值")]
        public string eventKey;

        [ShowIf("dialogActionType", DialogActionType.ChangeCharacterState)]
        [LabelText("目标action")]
        public CharacterState.CharAction newAction; // 角色的新状态
    }
}
