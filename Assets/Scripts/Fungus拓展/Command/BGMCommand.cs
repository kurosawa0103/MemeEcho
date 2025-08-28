using UnityEngine;
using Fungus;

[CommandInfo("Audio", "Override BGM", "∏≤∏«µ±«∞≤•∑≈µƒ±≥æ∞“Ù¿÷")]
public class BGMCommand : Command
{
    public BGMController bgmController;
    public AudioClip overrideClip;
    public BooleanData loop = new BooleanData(true);
    public BooleanData returnToAuto = new BooleanData(true);

    public override void OnEnter()
    {
        if (bgmController != null && overrideClip != null)
        {
            bgmController.OverrideBGM(overrideClip, loop.Value, returnToAuto.Value);
        }
        Continue();
    }

    public override string GetSummary()
    {
        return overrideClip ? overrideClip.name : "Œ¥÷∏∂®“Ù∆µ";
    }
}
