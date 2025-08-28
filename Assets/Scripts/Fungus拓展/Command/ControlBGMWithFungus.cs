using UnityEngine;
using Fungus;

[CommandInfo("Audio", "����BGM", "���š���ͣ��ֹͣ�����ǲ��Ż��滻��ҹ BGM")]
public class ControlBGMWithFungus : Command
{
    public enum BGMAction
    {
        Play,
        Pause,
        Stop,
        OverrideClip,
        ReplaceDayClip,
        ReplaceNightClip
    }

    [Tooltip("ѡ��Ҫִ�еĶ���")]
    public BGMAction action = BGMAction.Play;

    [Tooltip("��ѡ������ OverrideClip ���ŵ���Ƶ����")]
    public AudioClip overrideClip;

    [Tooltip("OverrideClip �Ƿ�ѭ������")]
    public bool loopOverrideClip = true;

    [Tooltip("OverrideClip ��������Ƿ񷵻��Զ� BGM �л�")]
    public bool returnToAuto = true;

    [Tooltip("ֹͣ����ͣʱ�Ƿ񵭳�")]
    public bool fadeOut = true;

    [Tooltip("PlayAuto ʱ�Ƿ����̲��ţ��������룩")]
    public bool instantPlay = false;

    [Tooltip("�����滻 Day/Night Clip ������Ƶ������ ReplaceDayClip / ReplaceNightClip��")]
    public AudioClip newDayOrNightClip;

    [Tooltip("�滻 Day/Night Clip ���Ƿ���������")]
    public bool playNowAfterReplace = true;

    public override void OnEnter()
    {
        GameObject musicObj = GameObject.FindGameObjectWithTag("music");
        if (musicObj == null)
        {
            Debug.LogWarning("δ�ҵ����� 'music' ��ǩ�����壡");
            Continue();
            return;
        }

        BGMController bgm = musicObj.GetComponent<BGMController>();
        if (bgm == null)
        {
            Debug.LogWarning("δ�ҵ� BGMController �ű���");
            Continue();
            return;
        }

        switch (action)
        {
            case BGMAction.Play:
                if (bgm.isPaused)
                    bgm.ResumeBGM();
                else
                    bgm.PlayAutoBGM(instantPlay);
                break;

            case BGMAction.Pause:
                bgm.PauseBGM();
                break;

            case BGMAction.Stop:
                bgm.StopBGM(fadeOut);
                break;

            case BGMAction.OverrideClip:
                if (overrideClip != null)
                    bgm.OverrideBGM(overrideClip, loopOverrideClip, returnToAuto);
                else
                    Debug.LogWarning("OverrideClip Ϊ�գ�");
                break;

            case BGMAction.ReplaceDayClip:
                if (newDayOrNightClip != null)
                    bgm.ReplaceDayClip(newDayOrNightClip, playNowAfterReplace);
                else
                    Debug.LogWarning("�� DayClip Ϊ�գ�");
                break;

            case BGMAction.ReplaceNightClip:
                if (newDayOrNightClip != null)
                    bgm.ReplaceNightClip(newDayOrNightClip, playNowAfterReplace);
                else
                    Debug.LogWarning("�� NightClip Ϊ�գ�");
                break;
        }

        Continue();
    }

    public override string GetSummary()
    {
        switch (action)
        {
            case BGMAction.OverrideClip:
                return $"Override BGM [{(overrideClip != null ? overrideClip.name : "��")}]";
            case BGMAction.ReplaceDayClip:
                return $"�滻���� BGM [{(newDayOrNightClip != null ? newDayOrNightClip.name : "��")}]";
            case BGMAction.ReplaceNightClip:
                return $"�滻ҹ�� BGM [{(newDayOrNightClip != null ? newDayOrNightClip.name : "��")}]";
            default:
                return $"BGM {action}";
        }
    }

    public override Color GetButtonColor()
    {
        return new Color32(200, 150, 255, 255);
    }
}
