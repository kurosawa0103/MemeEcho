using UnityEngine;

public class TimeFlowController : MonoBehaviour
{
    [Header("�Ƕ���Դ")]
    [SerializeField] private UIOrbitAngleTracker angleTracker;

    [Header("�ܿض���")]
    [SerializeField] private MonoBehaviour[] targets;   // �Ͻű����ɣ�����ʵ�� ITimeAgent

    [Tooltip("�Ƕ� �� ʱ���������/s  �� s��ʾ����1�� = 0.05 ��")]
    public float secondsPerDegree = 0.05f;

    void Awake()
    {
        angleTracker.OnAngleDeltaCW += HandleAngleDelta;
    }

    void HandleAngleDelta(float deltaDegreeCW)
    {
        float timeDelta = deltaDegreeCW * secondsPerDegree;

        foreach (var t in targets)
            (t as ITimeAgent)?.ApplyTime(timeDelta);
    }

    // ���� Start �� Inspector У�� targets ��ʵ���� ITimeAgent
}
