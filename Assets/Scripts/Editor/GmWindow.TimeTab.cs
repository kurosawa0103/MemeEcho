#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
public partial class GmWindow
{

    // ---------- 时间系统 ----------
    [TabGroup("页签", "昼夜系统"), LabelText("时间系统引用"), Required]
    public TimeSystem timeSystem;
    public void AutoFindTimeSystem()
    {
        if (timeSystem == null)
        {
            timeSystem = GameObject.FindObjectOfType<TimeSystem>();
            if (timeSystem != null)
                Debug.Log("[GmWindow] 已自动找到 timeSystem。");
        }
    }
    [TabGroup("页签", "昼夜系统"), Button("切换为 白天", ButtonSizes.Large)]
    private void BtnSwitchToDay() => TrySetDayState(DayState.DAY);

    [TabGroup("页签", "昼夜系统"), Button("切换为 黄昏", ButtonSizes.Large)]
    private void BtnSwitchToDusk() => TrySetDayState(DayState.DUSK);

    [TabGroup("页签", "昼夜系统"), Button("切换为 夜晚", ButtonSizes.Large)]
    private void BtnSwitchToNight() => TrySetDayState(DayState.NIGHT);

    [TabGroup("页签", "昼夜系统"), Button("恢复 昼夜自动切换", ButtonSizes.Medium)]
    private void BtnResumeCycle()
    {
        if (timeSystem == null)
        {
            Debug.LogWarning("[GmWindow.TimeTab] 未分配 TimeSystem！");
            return;
        }

        timeSystem.ResumeDayNightCycle();
        Debug.Log("[GmWindow.TimeTab] 已恢复昼夜自动循环");
    }

    private void TrySetDayState(DayState newState)
    {
        if (timeSystem == null)
        {
            Debug.LogWarning("[GmWindow.TimeTab] 未分配 TimeSystem！");
            return;
        }

        timeSystem.StopDayNightCycle();
        timeSystem.SetDayState(newState,2);
        Debug.Log($"[GmWindow.TimeTab] 手动切换为：{newState}");
    }

}
#endif
