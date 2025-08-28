#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
public partial class GmWindow
{

    // ---------- ʱ��ϵͳ ----------
    [TabGroup("ҳǩ", "��ҹϵͳ"), LabelText("ʱ��ϵͳ����"), Required]
    public TimeSystem timeSystem;
    public void AutoFindTimeSystem()
    {
        if (timeSystem == null)
        {
            timeSystem = GameObject.FindObjectOfType<TimeSystem>();
            if (timeSystem != null)
                Debug.Log("[GmWindow] ���Զ��ҵ� timeSystem��");
        }
    }
    [TabGroup("ҳǩ", "��ҹϵͳ"), Button("�л�Ϊ ����", ButtonSizes.Large)]
    private void BtnSwitchToDay() => TrySetDayState(DayState.DAY);

    [TabGroup("ҳǩ", "��ҹϵͳ"), Button("�л�Ϊ �ƻ�", ButtonSizes.Large)]
    private void BtnSwitchToDusk() => TrySetDayState(DayState.DUSK);

    [TabGroup("ҳǩ", "��ҹϵͳ"), Button("�л�Ϊ ҹ��", ButtonSizes.Large)]
    private void BtnSwitchToNight() => TrySetDayState(DayState.NIGHT);

    [TabGroup("ҳǩ", "��ҹϵͳ"), Button("�ָ� ��ҹ�Զ��л�", ButtonSizes.Medium)]
    private void BtnResumeCycle()
    {
        if (timeSystem == null)
        {
            Debug.LogWarning("[GmWindow.TimeTab] δ���� TimeSystem��");
            return;
        }

        timeSystem.ResumeDayNightCycle();
        Debug.Log("[GmWindow.TimeTab] �ѻָ���ҹ�Զ�ѭ��");
    }

    private void TrySetDayState(DayState newState)
    {
        if (timeSystem == null)
        {
            Debug.LogWarning("[GmWindow.TimeTab] δ���� TimeSystem��");
            return;
        }

        timeSystem.StopDayNightCycle();
        timeSystem.SetDayState(newState,2);
        Debug.Log($"[GmWindow.TimeTab] �ֶ��л�Ϊ��{newState}");
    }

}
#endif
