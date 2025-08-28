using UnityEngine;

/// <summary>
/// �� ��ʱ�䡱 ������� minScale �� maxScale��0��1��
/// �ѽű��ҵ���Ҫ����/��С�����壬���� TimeFlowController �� targets ��ע��
/// </summary>
public class GrowthScaleAgent : MonoBehaviour, ITimeAgent
{
    [Header("���ŷ�Χ")]
    [SerializeField] private Vector3 minScale = Vector3.zero;   // progress = 0
    [SerializeField] private Vector3 maxScale = Vector3.one;    // progress = 1

    [Header("��� 0��1 ��������")]
    [SerializeField] private float cycleSeconds = 5f;

    [Range(0f, 1f)]
    [Tooltip("��ǰ���ȣ�0~1��������ʱ�� ApplyTime() �ƽ�/����")]
    public float progress;

    /* ---------- ITimeAgent ---------- */
    public void ApplyTime(float timeDelta)
    {
        if (cycleSeconds <= 0f) return; // ��ֹ���� 0

        // ������ӳ�䵽 progress�������������������
        progress = Mathf.Clamp01(progress + timeDelta / cycleSeconds);

        // ���� progress ��������
        UpdateScale();
    }

    void UpdateScale()
    {
        transform.localScale = Vector3.LerpUnclamped(minScale, maxScale, progress);
    }

#if UNITY_EDITOR
    // �� Inspector ������ֵʱ��ʱ����
    void OnValidate() => UpdateScale();

    [ContextMenu("��Ϊ��ǰ����ֵ �� MaxScale")]
    void SetMaxScaleToCurrent()
    {
        maxScale = transform.localScale;
        Debug.Log($"[GrowthScaleAgent] �ѽ� MaxScale ����Ϊ��ǰ����ֵ��{maxScale}", this);
    }

    [ContextMenu("��Ϊ��ǰ����ֵ �� MinScale")]
    void SetMinScaleToCurrent()
    {
        minScale = transform.localScale;
        Debug.Log($"[GrowthScaleAgent] �ѽ� MinScale ����Ϊ��ǰ����ֵ��{minScale}", this);
    }
#endif
}
