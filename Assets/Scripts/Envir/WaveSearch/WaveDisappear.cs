using UnityEngine;
using DG.Tweening;

public class WaveDisappear : MonoBehaviour
{
    // ȫ�ּ���
    public WaveTriggerCounter counter;

    // �Ŵ��׼ֵ�����Ը����������
    public Vector3 baseScaleIncrease = new Vector3(0.2f, 0.2f, 0);

    // ��ǰ����� SpriteRenderer
    private SpriteRenderer spriteRenderer;

    // ����Ƿ��Ѿ�������
    private bool triggered = false;

    // �Ŵ�Ŀ�꣬�ҵ�tagΪSubRoot������
    private Transform subRootTransform;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        subRootTransform = GameObject.FindGameObjectWithTag("SubRoot")?.transform;
        if (subRootTransform == null)
        {
            Debug.LogError("û���ҵ�tagΪSubRoot������");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("SmallBox"))
        {
            triggered = true;


            // �ȵ����Լ�
            spriteRenderer.DOFade(0, 1f).OnComplete(() =>
            {
                // ������ɺ�ʼ�Ŵ�SubRoot���壬�Ŵ�������ݼ�����ͬ
                Vector3 targetScale = subRootTransform.localScale + baseScaleIncrease ;
                subRootTransform.DOScale(targetScale, 0.5f);
                counter.currentCount++;
                counter.TryScale();
            });

            // ���ô���������ֹ�ظ�����
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
