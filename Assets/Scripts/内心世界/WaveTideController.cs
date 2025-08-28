using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class WaveTideController : MonoBehaviour
{
    [Header("涨潮参数")]
    [Tooltip("涨潮最小Y轴点")]
    public float minTideY = 0f;

    [Tooltip("涨潮最大Y轴点")]
    public float maxTideY = 200f;

    [Tooltip("湿沙滩精灵")]
    public SpriteRenderer wetSandSprite;

    [Tooltip("湿沙滩淡出时间")]
    public float wetSandFadeOutDuration = 1f;

    [Header("退潮参数")]
    [Tooltip("退潮目标Y轴点")]
    public float lowTideY = -100f;

    [Header("涨退潮控制")]
    [Tooltip("涨潮持续时间")]
    public float tideDuration = 5f;

    [Tooltip("退潮持续时间")]
    public float retreatDuration = 5f;

    [Tooltip("是否自动循环涨退潮")]
    public bool autoCycle = true;

    [Tooltip("拖拽物体（可无）")]
    public DraggableItem draggableItem;

    private Transform tideTransform;

    private enum TideState { Rising, Falling }
    private TideState currentState = TideState.Rising;

    private Tween moveTween;
    private Tween wetSandFadeTween;

    private void Awake()
    {
        draggableItem = GameObject.FindGameObjectWithTag("SpItem")?.GetComponent<DraggableItem>();
        tideTransform = transform;

        if (wetSandSprite != null)
        {
            Color c = wetSandSprite.color;
            c.a = 0f;
            wetSandSprite.color = c;
            wetSandSprite.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (autoCycle)
        {
            StartRising();
        }
    }

    private void OnDisable()
    {
        moveTween?.Kill();
        wetSandFadeTween?.Kill();
    }

    private void StartRising()
    {
        currentState = TideState.Rising;
        float targetY = GetRisingTargetY();

        moveTween?.Kill();
        moveTween = tideTransform.DOLocalMoveY(targetY, tideDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                ShowWetSandEffect();
                if (autoCycle)
                {
                    StartFalling(); // 立即退潮
                }
            });
    }

    private void StartFalling()
    {
        currentState = TideState.Falling;

        moveTween?.Kill();
        moveTween = tideTransform.DOLocalMoveY(lowTideY, retreatDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                if (autoCycle)
                {
                    StartRising();
                }
            });
    }

    private float GetRisingTargetY()
    {
        float percent = 0.5f;
        if (draggableItem != null)
        {
            percent = Mathf.Clamp01(draggableItem.anglePercent);
        }
        return Mathf.Lerp(minTideY, maxTideY, percent);
    }

    private void ShowWetSandEffect()
    {
        if (wetSandSprite == null) return;

        wetSandFadeTween?.Kill();

        // 设置湿沙滩精灵的位置到当前涨潮目标点
        Vector3 pos = wetSandSprite.transform.localPosition;
        pos.y = GetRisingTargetY();
        wetSandSprite.transform.localPosition = pos;

        wetSandSprite.gameObject.SetActive(true);
        Color c = wetSandSprite.color;
        c.a = 1f;
        wetSandSprite.color = c;

        wetSandFadeTween = wetSandSprite.DOFade(0f, wetSandFadeOutDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            wetSandSprite.gameObject.SetActive(false);
        });
    }

    // 外部调用，刷新涨潮位置（无动画）
    public void RefreshTidePosition()
    {
        if (currentState == TideState.Rising)
        {
            float newY = GetRisingTargetY();
            tideTransform.localPosition = new Vector3(tideTransform.localPosition.x, newY, tideTransform.localPosition.z);
        }
    }
}
