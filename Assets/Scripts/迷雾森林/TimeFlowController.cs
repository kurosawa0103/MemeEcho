using UnityEngine;

public class TimeFlowController : MonoBehaviour
{
    [Header("角度来源")]
    [SerializeField] private UIOrbitAngleTracker angleTracker;

    [Header("受控对象")]
    [SerializeField] private MonoBehaviour[] targets;   // 拖脚本即可，必须实现 ITimeAgent

    [Tooltip("角度 → 时间比例，°/s  → s。示例：1° = 0.05 秒")]
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

    // 可在 Start 或 Inspector 校验 targets 都实现了 ITimeAgent
}
