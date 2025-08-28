using UnityEngine;
using Fungus;

[CommandInfo("Audio", "控制BGM", "播放、暂停、停止、覆盖播放或替换昼夜 BGM")]
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

    [Tooltip("选择要执行的动作")]
    public BGMAction action = BGMAction.Play;

    [Tooltip("可选：用于 OverrideClip 播放的音频剪辑")]
    public AudioClip overrideClip;

    [Tooltip("OverrideClip 是否循环播放")]
    public bool loopOverrideClip = true;

    [Tooltip("OverrideClip 播放完后是否返回自动 BGM 切换")]
    public bool returnToAuto = true;

    [Tooltip("停止或暂停时是否淡出")]
    public bool fadeOut = true;

    [Tooltip("PlayAuto 时是否立刻播放（跳过淡入）")]
    public bool instantPlay = false;

    [Tooltip("用于替换 Day/Night Clip 的新音频（用于 ReplaceDayClip / ReplaceNightClip）")]
    public AudioClip newDayOrNightClip;

    [Tooltip("替换 Day/Night Clip 后是否立即播放")]
    public bool playNowAfterReplace = true;

    public override void OnEnter()
    {
        GameObject musicObj = GameObject.FindGameObjectWithTag("music");
        if (musicObj == null)
        {
            Debug.LogWarning("未找到带有 'music' 标签的物体！");
            Continue();
            return;
        }

        BGMController bgm = musicObj.GetComponent<BGMController>();
        if (bgm == null)
        {
            Debug.LogWarning("未找到 BGMController 脚本！");
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
                    Debug.LogWarning("OverrideClip 为空！");
                break;

            case BGMAction.ReplaceDayClip:
                if (newDayOrNightClip != null)
                    bgm.ReplaceDayClip(newDayOrNightClip, playNowAfterReplace);
                else
                    Debug.LogWarning("新 DayClip 为空！");
                break;

            case BGMAction.ReplaceNightClip:
                if (newDayOrNightClip != null)
                    bgm.ReplaceNightClip(newDayOrNightClip, playNowAfterReplace);
                else
                    Debug.LogWarning("新 NightClip 为空！");
                break;
        }

        Continue();
    }

    public override string GetSummary()
    {
        switch (action)
        {
            case BGMAction.OverrideClip:
                return $"Override BGM [{(overrideClip != null ? overrideClip.name : "空")}]";
            case BGMAction.ReplaceDayClip:
                return $"替换白天 BGM [{(newDayOrNightClip != null ? newDayOrNightClip.name : "空")}]";
            case BGMAction.ReplaceNightClip:
                return $"替换夜晚 BGM [{(newDayOrNightClip != null ? newDayOrNightClip.name : "空")}]";
            default:
                return $"BGM {action}";
        }
    }

    public override Color GetButtonColor()
    {
        return new Color32(200, 150, 255, 255);
    }
}
