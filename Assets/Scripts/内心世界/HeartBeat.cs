using UnityEngine;

public class HeartBeat : MonoBehaviour
{
    [Header("��������")]
    public float beatDuration = 1.0f;           // һ�����������ڵ�ʱ�䣨�룩
    public float scaleAmount = 0.2f;            // ���ŷ���
    public AnimationCurve beatCurve;            // ���������������״

    private Vector3 originalScale;
    private float timer;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (beatCurve == null || beatCurve.length == 0) return;

        // ʱ���� [0, 1] ֮��ѭ��
        timer += Time.deltaTime / beatDuration;
        if (timer > 1f) timer -= 1f;

        float curveValue = beatCurve.Evaluate(timer); // ��ȡ��ǰ����ǿ��
        transform.localScale = originalScale + Vector3.one * curveValue * scaleAmount;
    }
}