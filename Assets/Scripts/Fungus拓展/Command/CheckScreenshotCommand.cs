using Fungus;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CommandInfo("Custom",
             "Check Screenshot",
             "检查截图脚本是否完成截图，并根据条件触发")]
public class CheckScreenshotCommand : Command
{
    // Screenshot Capture Mode
    public enum ScreenshotCaptureMode
    {
        AnyImage,           // 检查任意截图
        ConditionBased      // 检查根据条件截图
    }

    [Tooltip("选择截图模式")]
    public ScreenshotCaptureMode captureMode = ScreenshotCaptureMode.AnyImage;

    [Tooltip("MaskedScreenshot 脚本的引用")]
    public MaskedScreenshot maskedScreenshot;

    [Tooltip("条件配置，适用于 ConditionBased 模式")]
    public PhotoCaptureCondition[] conditionConfigs;

    private bool isCheckingCondition = false; // 是否正在检查条件

    public override void OnEnter()
    {
        maskedScreenshot = GameObject.FindObjectOfType<MaskedScreenshot>();
        // 订阅截图完成事件
        if (maskedScreenshot != null)
        {
            maskedScreenshot.OnScreenshotTaken += HandleScreenshotTaken;
        }

        // 根据选择的截图模式启动检测逻辑
        if (captureMode == ScreenshotCaptureMode.ConditionBased)
        {
            if (!isCheckingCondition)
            {
                isCheckingCondition = true;
                Flowchart.BroadcastFungusMessage("StartCheckingScreenshotCondition");
                StartCoroutine(CheckScreenshotBasedOnCondition());
            }
        }
        else if (captureMode == ScreenshotCaptureMode.AnyImage)
        {
            // 检查是否已经有截图完成
            CheckIfScreenshotTaken();
        }
    }

    private void HandleScreenshotTaken(PhotoCaptureCondition condition)
    {
        Debug.Log($"收到 ScreenshotTaken 事件，匹配条件：{condition.conditionID}，广播给 Fungus");

        if (captureMode == ScreenshotCaptureMode.AnyImage ||
            (captureMode == ScreenshotCaptureMode.ConditionBased &&
             System.Array.Exists(conditionConfigs, c => c.conditionID == condition.conditionID)))
        {
            Flowchart.BroadcastFungusMessage("ScreenshotTaken");
            isCheckingCondition = false;
            Continue(); 
        }
    }

    /// <summary>
    /// 持续检查截图条件是否满足
    /// </summary>
    private IEnumerator CheckScreenshotBasedOnCondition()
    {
        while (isCheckingCondition)
        {
            foreach (var config in conditionConfigs)
            {

                // 检查条件
                if (maskedScreenshot.CheckCondition(config))
                {
                    // 条件满足，检查截图是否已完成
                    CheckIfScreenshotTaken();
                    yield break; // 条件已满足，停止检查
                }
            }

            yield return new WaitForSeconds(0.1f); // 延迟一定时间后再检查，减少频率
        }
    }

    /// <summary>
    /// 检查截图是否已完成（由截图脚本触发）
    /// </summary>
    private void CheckIfScreenshotTaken()
    {
        // 判断截图是否完成，截图脚本需要通知此信息
        if (maskedScreenshot != null)
        {
            // 截图完成，执行相关逻辑
            Flowchart.BroadcastFungusMessage("ScreenshotTaken");

            // 可以根据需求执行其他操作
            isCheckingCondition = false; // 停止持续检查
        }
    }
    public override Color GetButtonColor()
    {
        return new Color32(200, 100, 150, 255);
    }
    public override string GetSummary()
    {
        return captureMode == ScreenshotCaptureMode.AnyImage ? "检查任意截图是否完成" : "检查条件截图是否完成";
    }
}
