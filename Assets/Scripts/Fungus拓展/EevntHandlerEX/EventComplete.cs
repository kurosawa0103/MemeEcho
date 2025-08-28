using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System.Linq;

[EventHandlerInfo("拓展",
                  "事件完成监听",
                  "当玩家完成指定事件时触发")]
public class EventComplete : EventHandler
{
    [System.Serializable]
    public class EventCondition
    {
        public string eventKey;       // 事件的 Key
        public string expectedValue;  // 期望的值
        public bool isNegate = false; // 是否取反
    }

    public enum LogicType { AND, OR } // 只需要一个 AND/OR 逻辑

    private IEventStateDataHandler eventStateHandler = new JsonGenerator();

    [Header("条件的逻辑类型 (AND / OR)")]
    [SerializeField] private LogicType conditionsLogicType = LogicType.AND; // 控制所有条件的逻辑

    [Header("判断的值 (多个条件)")]
    [SerializeField] private List<EventCondition> conditions = new List<EventCondition>();

    [Header("延迟执行的帧数")]
    [SerializeField] private int delayFrames = 0;  // 帧延迟



    private void OnEnable()
    {
        JsonGenerator.OnEventStatusChanged += HandleEventStatusChanged;
    }

    private void OnDisable()
    {
        JsonGenerator.OnEventStatusChanged -= HandleEventStatusChanged;
    }

    private void HandleEventStatusChanged(string eventName, string newStatus)
    {
        // 只处理 conditions 里配置的事件
        if (!conditions.Any(c => c.eventKey == eventName)) return;

        if (EvaluateConditions())
        {
            Debug.Log($"事件 '{eventName}' 状态变更为: {newStatus},  开始执行");
            StartCoroutine(ExecuteBlockWithDelay());
        }
    }

    private bool EvaluateConditions()
    {
        if (conditions.Count == 0) return false;

        // 根据 conditionsLogicType 选择初始值
        bool result = conditionsLogicType == LogicType.OR ? false : true;

        foreach (var condition in conditions)
        {
            bool conditionResult = condition.isNegate
                ? !CheckCondition(condition)
                : CheckCondition(condition);

            if (conditionsLogicType == LogicType.AND)
            {
                result &= conditionResult; // AND 逻辑
            }
            else if (conditionsLogicType == LogicType.OR)
            {
                result |= conditionResult; // OR 逻辑
            }
        }

        return result;
    }

    private bool CheckCondition(EventCondition condition)
    {

        // 获取事件的当前状态值
        string eventValue = eventStateHandler.GetEventStatus(condition.eventKey);

        // 判断是否匹配
        bool isMatch = eventValue == condition.expectedValue;

        // 处理 NOT 逻辑（如果 isNegate 为 true，则取反结果）
        return condition.isNegate ? !isMatch : isMatch;
    }

    private IEnumerator ExecuteBlockWithDelay()
    {
        for (int i = 0; i < delayFrames; i++)
        {
            yield return null;
        }
        ExecuteBlock();
    }
}
