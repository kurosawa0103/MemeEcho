using Fungus;
using UnityEngine;
using System.Collections.Generic;

[CommandInfo("Custom", "Set Screenshot Conditions", "设置截图条件", 0)]
public class SetScreenShot : Command
{
    [Tooltip("条件列表")]
    public List<PhotoCaptureCondition> conditionConfigs;

    private MaskedScreenshot maskedScreenshot;

    public override void OnEnter()
    {
        // 获取 MaskedScreenshot 脚本实例
        maskedScreenshot = FindObjectOfType<MaskedScreenshot>();

        if (maskedScreenshot != null)
        {
            // 将配置的条件列表赋值给 MaskedScreenshot 脚本
            maskedScreenshot.conditionConfigs = conditionConfigs;
            Debug.Log("截图条件已配置完成");
        }
        else
        {
            Debug.LogWarning("未找到 MaskedScreenshot 脚本");
        }

        Continue();
    }
}
