using UnityEngine;
using Fungus;

[CommandInfo("Custom", "Set Tuner Target Point", "���� TunerController ��Ŀ���ͻ���Ŀ��ֵ")]
public class SetTunerTargetPoint : Command
{
    [Tooltip("¼��Ŀ��㣬������ WaveTargetParam")]
    public GameObject targetPoint;

    [Tooltip("TunerController �������")]
    public TunerController tunerController;

    public override void OnEnter()
    {
        if (targetPoint == null || tunerController == null)
        {
            Debug.LogWarning("[SetTunerTargetPoint] ����δ���ã�");
            Continue();
            return;
        }

        WaveTargetParam param = targetPoint.GetComponent<WaveTargetParam>();
        if (param == null)
        {
            Debug.LogWarning("[SetTunerTargetPoint] Ŀ���û�� WaveTargetParam �����");
            Continue();
            return;
        }

        // ���� TunerController ��Ŀ������ Transform
        tunerController.targetArea = targetPoint.transform;

        // ���û���Ŀ��ֵ
        tunerController.targetA = param.targetSliderValueA;
        tunerController.hasReportedSuccess = false;
        Continue();
    }
}
