using Fungus;
using UnityEngine;

[CommandInfo("Narrative",
             "PauseSailAndTime",
             "Pause or resume the sailor counting and the day-night cycle.")]
public class PauseSailAndTime : Command
{
    [Tooltip("��ͣ TimeSystem �ļ�ʱ")]
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
        if (pauseTimeSystem) pauseDesc += "��ͣʱ�� ";
        else pauseDesc += "�ָ�ʱ�� ";

        return pauseDesc;
    }
}
