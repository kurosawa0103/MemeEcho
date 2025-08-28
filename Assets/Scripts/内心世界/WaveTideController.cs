using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class WaveTideController : MonoBehaviour
{
    [Header("�ǳ�����")]
    [Tooltip("�ǳ���СY���")]
    public float minTideY = 0f;

    [Tooltip("�ǳ����Y���")]
    public float maxTideY = 200f;

    [Tooltip("ʪɳ̲����")]
    public SpriteRenderer wetSandSprite;

    [Tooltip("ʪɳ̲����ʱ��")]
    public float wetSandFadeOutDuration = 1f;

    [Header("�˳�����")]
    [Tooltip("�˳�Ŀ��Y���")]
    public float lowTideY = -100f;

    [Header("���˳�����")]
    [Tooltip("�ǳ�����ʱ��")]
    public float tideDuration = 5f;

    [Tooltip("�˳�����ʱ��")]
    public float retreatDuration = 5f;

    [Tooltip("�Ƿ��Զ�ѭ�����˳�")]
    public bool autoCycle = true;

    [Tooltip("��ק���壨���ޣ�")]
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
                    StartFalling(); // �����˳�
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

        // ����ʪɳ̲�����λ�õ���ǰ�ǳ�Ŀ���
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

    // �ⲿ���ã�ˢ���ǳ�λ�ã��޶�����
    public void RefreshTidePosition()
    {
        if (currentState == TideState.Rising)
        {
            float newY = GetRisingTargetY();
            tideTransform.localPosition = new Vector3(tideTransform.localPosition.x, newY, tideTransform.localPosition.z);
        }
    }
}
