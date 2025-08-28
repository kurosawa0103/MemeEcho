using UnityEngine;
using Fungus;

[CommandInfo("Custom", "Set Tuner Target Point", "设置 TunerController 的目标点和滑块目标值")]
public class SetTunerTargetPoint : Command
{
    [Tooltip("录音目标点，挂载有 WaveTargetParam")]
    public GameObject targetPoint;

    [Tooltip("TunerController 组件引用")]
    public TunerController tunerController;

    public override void OnEnter()
    {
        if (targetPoint == null || tunerController == null)
        {
            Debug.LogWarning("[SetTunerTargetPoint] 参数未设置！");
            Continue();
            return;
        }

        WaveTargetParam param = targetPoint.GetComponent<WaveTargetParam>();
        if (param == null)
        {
            Debug.LogWarning("[SetTunerTargetPoint] 目标点没有 WaveTargetParam 组件！");
            Continue();
            return;
        }

        // 设置 TunerController 的目标区域 Transform
        tunerController.targetArea = targetPoint.transform;

        // 设置滑块目标值
        tunerController.targetA = param.targetSliderValueA;
        tunerController.hasReportedSuccess = false;
        Continue();
    }
}
