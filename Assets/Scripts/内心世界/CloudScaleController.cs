using UnityEngine;

public class CloudScaleController : MonoBehaviour
{
    public CloudEmitter emitter;   // ���� CloudEmitter
    public float duration = 10f;   // �ӳ�ʼ�� 0 ����ʱ��
    private float timer = 0f;
    private float initialMaxScale;

    void Start()
    {
        if (emitter == null)
        {
            emitter = GetComponent<CloudEmitter>();
        }

        if (emitter != null)
        {
            initialMaxScale = emitter.maxScale;
        }
        else
        {
            Debug.LogError("CloudEmitter not assigned to CloudScaleController.");
            enabled = false;
        }
    }

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            emitter.maxScale = Mathf.Lerp(initialMaxScale, 0f, t);
        }
    }
}