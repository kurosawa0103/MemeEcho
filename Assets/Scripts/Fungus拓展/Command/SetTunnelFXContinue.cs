using UnityEngine;
using Fungus;

[CommandInfo("TunnelFX",
             "Auto Continue SpeedUp",
             "�Զ����� TunnelFXAutoAdjust ������ allowContinueSpeedUp = true")]
public class AutoContinueTunnelFX : Command
{
    public float setSpeed=1;
    public override void OnEnter()
    {
        TunnelFXAutoAdjust fx = GameObject.FindObjectOfType<TunnelFXAutoAdjust>();

        if (fx != null)
        {
            fx.allowContinueSpeedUp = true;
            fx.stopAtTargetSpeed = false;
            fx.speedIncreaseRate = setSpeed;
            Debug.Log("TunnelFXAutoAdjust found and allowContinueSpeedUp = true");
        }
        else
        {
            Debug.LogWarning("No TunnelFXAutoAdjust found in scene.");
        }

        Continue(); // ����ִ����һ��ָ��
    }

    public override string GetSummary()
    {
        return "Find & enable TunnelFX continue speed up";
    }

    public override Color GetButtonColor()
    {
        return new Color32(190, 240, 255, 255); // ǳ��
    }
}
