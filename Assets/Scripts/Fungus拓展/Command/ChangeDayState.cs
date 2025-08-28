using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Custom", "Change Day State", "Changes the day state of the game to a specified state.")]
public class ChangeDayState : Command
{
    [Tooltip("The target DayState to set.")]
    public DayState newDayState;

    public float fadeTime=2;
    public override void OnEnter()
    {
        TimeSystem timeSystem = FindObjectOfType<TimeSystem>();  // 获取 TimeSystem 实例

        if (timeSystem != null)
        {
            timeSystem.SetDayState(newDayState, fadeTime);  // 调用 SetDayState 方法切换昼夜状态
        }
        else
        {
            Debug.LogWarning("TimeSystem not found in the scene.");
        }

        Continue();  // 继续执行接下来的命令
    }

    // 获取命令的总结信息
    public override string GetSummary()
    {
        return $"Set day state to {newDayState.ToString()}";  // 返回选定的昼夜状态作为总结
    }
}
