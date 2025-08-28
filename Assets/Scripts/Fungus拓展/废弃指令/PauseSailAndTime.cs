using Fungus;
using UnityEngine;

[CommandInfo("Narrative",
             "PauseSailAndTime",
             "Pause or resume the sailor counting and the day-night cycle.")]
public class PauseSailAndTime : Command
{
    [Tooltip("暂停 TimeSystem 的计时")]
    public bool pauseTimeSystem = false;

    private TimeSystem timeSystem;

    public override void OnEnter()
    {
        timeSystem = FindObjectOfType<TimeSystem>();

        if (pauseTimeSystem)
        {
            if (timeSystem != null)
            {
                timeSystem.StopDayNightCycle();
            }
        }
        else
        {
            if (timeSystem != null)
            {
                timeSystem.ResumeDayNightCycle();
            }
        }

        Continue();
    }

    public override string GetSummary()
    {
        string pauseDesc = "";
        if (pauseTimeSystem) pauseDesc += "暂停时间 ";
        else pauseDesc += "恢复时间 ";

        return pauseDesc;
    }
}
