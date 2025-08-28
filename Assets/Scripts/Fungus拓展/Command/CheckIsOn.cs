using Fungus;
using UnityEngine;
using System.Collections;

public enum CheckType { Diary, Map }

[CommandInfo("Narrative",
             "CheckIsOn",
             "Waits until diary or map changes to the target state.")]
public class CheckIsOn : Command
{
    [Tooltip("Select what to check (Diary or Map)")]
    [SerializeField] private CheckType checkType = CheckType.Diary;

    [Tooltip("Target state: true = wait until it becomes ON, false = becomes OFF")]
    [SerializeField] private bool isOn = true;

    private OpenDiary openDiary;
    private OpenMap openMap;

    private TimeSystem timeSystem;
    private Sailor sailor;

    [Tooltip("暂停 TimeSystem 的计时")]
    public bool pauseTimeSystem = false;

    public override void OnEnter()
    {
        //暂停时间
        if (pauseTimeSystem)
        {
            if (timeSystem != null)
            {
                timeSystem.StopDayNightCycle();
            }
        }

        StopSailAndTime();
        switch (checkType)
        {
            case CheckType.Diary:
                openDiary = FindObjectOfType<OpenDiary>();
                if (openDiary != null)
                {
                    GetFlowchart().StartCoroutine(WaitForDiaryChangeToTarget());
                    return;
                }
                break;

            case CheckType.Map:
                openMap = FindObjectOfType<OpenMap>();
                if (openMap != null)
                {
                    GetFlowchart().StartCoroutine(WaitForMapChangeToTarget());
                    return;
                }
                break;
        }

        Continue(); // 如果找不到对象就直接继续
    }

    private IEnumerator WaitForDiaryChangeToTarget()
    {
        bool prev = openDiary.diaryIsOn;

        while (openDiary != null)
        {
            bool current = openDiary.diaryIsOn;

            // 如果状态发生变化，并且变成目标状态
            if (current != prev && current == isOn)
            {
                break;
            }

            prev = current;
            yield return null;
        }
        ResumeSailAndTime();
        Continue();
    }

    private IEnumerator WaitForMapChangeToTarget()
    {
        bool prev = openMap.mapIsOn;

        while (openMap != null)
        {
            bool current = openMap.mapIsOn;

            if (current != prev && current == isOn)
            {
                break;
            }

            prev = current;
            yield return null;
        }
        ResumeSailAndTime();
        Continue();
    }
    void StopSailAndTime()
    {
        if (pauseTimeSystem)
        {
            if (timeSystem != null)
            {
                timeSystem.ResumeDayNightCycle();
            }
        }
       
    }
    void ResumeSailAndTime()
    {
        if (pauseTimeSystem)
        {
            if (timeSystem != null)
            {
                timeSystem.ResumeDayNightCycle();
            }
        }
    }
    public override Color GetButtonColor()
    {
        return new Color32(200, 100, 150, 255);
    }
    public override string GetSummary()
    {
        string target = isOn ? "ON" : "OFF";
        return $"等待 {checkType}界面   变为 {target}";
    }
}
