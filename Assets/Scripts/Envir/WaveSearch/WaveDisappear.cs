using UnityEngine;
using DG.Tweening;

public class WaveDisappear : MonoBehaviour
{
    // 全局计数
    public WaveTriggerCounter counter;

    // 放大基准值，可以根据需求调整
    public Vector3 baseScaleIncrease = new Vector3(0.2f, 0.2f, 0);

    // 当前物体的 SpriteRenderer
    private SpriteRenderer spriteRenderer;

    // 标记是否已经触发过
    private bool triggered = false;

    // 放大目标，找到tag为SubRoot的物体
    private Transform subRootTransform;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        subRootTransform = GameObject.FindGameObjectWithTag("SubRoot")?.transform;
        if (subRootTransform == null)
        {
            Debug.LogError("没有找到tag为SubRoot的物体");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("SmallBox"))
        {
            triggered = true;


            // 先淡出自己
            spriteRenderer.DOFade(0, 1f).OnComplete(() =>
            {
                // 淡出完成后开始放大SubRoot物体，放大比例根据计数不同
                Vector3 targetScale = subRootTransform.localScale + baseScaleIncrease ;
                subRootTransform.DOScale(targetScale, 0.5f);
                counter.currentCount++;
                counter.TryScale();
            });

            // 禁用触发器，防止重复触发
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
