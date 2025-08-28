using UnityEngine;
using Fungus;

[CommandInfo("TunnelFX",
             "Auto Continue SpeedUp",
             "自动查找 TunnelFXAutoAdjust 并设置 allowContinueSpeedUp = true")]
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

        Continue(); // 继续执行下一个指令
    }

    public override string GetSummary()
    {
        return "Find & enable TunnelFX continue speed up";
    }

    public override Color GetButtonColor()
    {
        return new Color32(190, 240, 255, 255); // 浅蓝
    }
}
