using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class CharacterObject : MonoBehaviour
{
    public string trackName;
    public bool needRecord;
    [ShowIf("needRecord", true)]
    public string eventName;
    [ShowIf("needRecord", true)]
    public string eventValue;
    [ShowIf("needRecord", true), ReadOnly]
    public float accumulatedLockTime = 0f;  // �ۻ�������ʱ��
    [ShowIf("needRecord", true)]
    public float targetTime;
    [ShowIf("needRecord", true)]
    public bool hasRecorded = false;        // ��־����¼�Ƿ��Ѿ�ִ�й�

    public bool needPhoto;
    [ShowIf("needPhoto", true)]
    public string photoTargetName;  // ֻ���� needPhoto Ϊ true ʱ��ʾ

    public bool canClickTalk=true;//���ڵ���Ի����

    [Header("������ݿ���")]
    public bool canShow = true;

    [LabelText("���ݶ���UI��3D���壩")]
    public GameObject speechBubble;

    private bool isUIDragging = false;
    private Tween currentTween;
    private SpriteRenderer spriteRenderer;

    private Tween hoverTween;
    private Vector3 initialLocalPosition;
    private Tween idleTween;
    [Header("������ʾ�߼�����")]
    public bool onlyShowOnHover = false;  // ��ѡ������������ʱ����ʾ����
    public float targetHintScale = 1;

    private void Awake()
    {
        if (speechBubble != null)
        {
            spriteRenderer = speechBubble.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogWarning("speechBubble ��Ҫ���� SpriteRenderer ���");
            }

            initialLocalPosition = speechBubble.transform.localPosition;

            // ��������ˡ�������ʱ��ʾ�������ʼ�������ݲ�����Ϊ���ɼ�״̬
            if (onlyShowOnHover)
            {
                speechBubble.transform.localScale = Vector3.zero;
                if (spriteRenderer != null)
                {
                    Color c = spriteRenderer.color;
                    spriteRenderer.color = new Color(c.r, c.g, c.b, 0f);
                }
                speechBubble.SetActive(false);
            }
        }
    }


    private void OnEnable()
    {
        DraggableUIWindow.OnDraggingChanged += OnDraggingChanged;
        StartIdleAnimation(); //  ����Ĭ������
    }

    private void OnDisable()
    {
        DraggableUIWindow.OnDraggingChanged -= OnDraggingChanged;
        StopIdleAnimation(); //  �ر�Ĭ������
    }

    private void OnDraggingChanged(bool isDragging)
    {
        isUIDragging = isDragging;
    }


    private void OnMouseOver()
    {
        if (canShow && !isUIDragging)
        {
            if (speechBubble != null )
            {
                StartHoverAnimation();
            }
        }
    }
    private void OnMouseEnter()
    {
        if (canShow && onlyShowOnHover)
        {
            ShowBubble(); //  ��ѡ�����ʱ����ʾ
        }
    }
    private void OnMouseExit()
    {
        StopHoverAnimation();
        if (onlyShowOnHover)
        {
            HideBubble(); // ��ѡ���뿪ʱ������
        }
    }

    public void ShowBubble()
{
    if (speechBubble == null || spriteRenderer == null) return;

    currentTween?.Kill();

    speechBubble.SetActive(true);
    speechBubble.transform.localPosition = initialLocalPosition;
    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
    speechBubble.transform.localScale = Vector3.zero;

        currentTween = DOTween.Sequence()
            .Append(spriteRenderer.DOFade(1f, 0.4f))
            .Join(speechBubble.transform.DOScale(targetHintScale, 0.4f).SetEase(Ease.OutBack))
            .Join(speechBubble.transform.DOShakePosition(
                    0.4f,
                    0.1f,
                    10,
                    90
                    ).SetRelative(true));
    }


    public void HideBubble()
    {
        if (speechBubble == null || spriteRenderer == null) return;

        currentTween?.Kill();

        currentTween = DOTween.Sequence()
            .Append(spriteRenderer.DOFade(0f, 0.5f))
            .Join(speechBubble.transform.DOScale(0f, 0.5f))
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                speechBubble.SetActive(false);
            });
    }

    private void StartHoverAnimation()
    {
        if (hoverTween != null && hoverTween.IsActive())
            return; // �����ظ�����

        hoverTween = speechBubble.transform.DOPunchPosition(
            new Vector3(0, 0.08f, 0), // ���ϵ�0.05��λ
            0.8f,                     // ����ʱ��0.5��
            8,                       // �𶯴���
            1f                        // ���ǿ��
        ).OnComplete(() =>
        {
            // ������ɺ��ٹ�λһ��
            speechBubble.transform.localPosition = initialLocalPosition;
            hoverTween = null;
        });
    }

    private void StopHoverAnimation()
    {
        // Punch���������Զ���λ������ǿ��ֹͣ
        // �������������ã��������ж���״̬
        if (hoverTween != null)
        {
            hoverTween.Kill();
            hoverTween = null;
        }
        if (speechBubble != null)
        {
            // �뿪ʱҲǿ�ƹ�λ
            speechBubble.transform.localPosition = initialLocalPosition;
        }
    }
    private void StartIdleAnimation()
    {
        if (speechBubble == null) return;

        // �����ظ�
        if (idleTween != null && idleTween.IsActive()) return;

        idleTween = DOTween.Sequence()
            .AppendCallback(() =>
            {
                speechBubble.transform.DOPunchPosition(
                    new Vector3(0, 0.05f, 0f), //  ��΢����
                    0.5f,
                    4,
                    0.5f
                ).SetRelative(true);
            })
            .AppendInterval(0.5f) // ÿ1.5����һ��
            .SetLoops(-1, LoopType.Restart);
    }
    private void StopIdleAnimation()
    {
        if (idleTween != null)
        {
            idleTween.Kill();
            idleTween = null;
        }
    }

    public bool HasReachedRecordTime()
    {
        return accumulatedLockTime >= targetTime;
    }

}
