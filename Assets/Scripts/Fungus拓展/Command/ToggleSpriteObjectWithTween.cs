using UnityEngine;
using Fungus;
using DG.Tweening;

[CommandInfo("�Զ�����չ", "ToggleSpriteObjectWithTween", "����Sprite���壬������+���붯��")]
public class ToggleSpriteObjectWithTween : Command
{
    [Tooltip("Ҫ���Ƶ����� (��Ҫ��SpriteRenderer)")]
    public GameObject targetObject;

    [Tooltip("��Ч����ʱ��")]
    public float tweenDuration = 0.5f;

    [Tooltip("�Ƿ�ȴ�������������ټ�������")]
    public bool waitUntilFinished = true;

    [Tooltip("ǿ�ƹرգ����۵�ǰ״̬")]
    public bool forceClose = false;

    private SpriteRenderer spriteRenderer;

    public override void OnEnter()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("û������Ŀ�����壡");
            Continue();
            return;
        }

        spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Ŀ��������û��SpriteRenderer��");
            Continue();
            return;
        }

        if (forceClose)
        {
            Debug.Log("[ToggleSpriteObjectWithTween] ǿ�ƹر�Ŀ�����塣");
            if (targetObject.activeSelf)
            {
                CloseObject();
            }
            else
            {
                // ����Ѿ��ǹر�״̬��ҲҪִ�������߼�����ֹ����״̬������
                Color color = spriteRenderer.color;
                color.a = 0f;
                spriteRenderer.color = color;
                targetObject.transform.localScale = Vector3.zero;
                targetObject.SetActive(false);
                Continue();
            }
        }
        else
        {
            // �����л��߼�
            if (targetObject.activeSelf)
            {
                CloseObject();
            }
            else
            {
                OpenObject();
            }
        }
    }

    void OpenObject()
    {
        targetObject.SetActive(true);
        targetObject.transform.localScale = Vector3.zero;

        // ���ó�ʼ͸����Ϊ0
        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;

        Sequence openSeq = DOTween.Sequence();
        openSeq.Append(targetObject.transform.DOScale(Vector3.one, tweenDuration).SetEase(Ease.OutBack));
        openSeq.Join(spriteRenderer.DOFade(1f, tweenDuration));

        if (waitUntilFinished)
        {
            openSeq.OnComplete(() => Continue());
        }
        else
        {
            Continue();
        }
    }

    void CloseObject()
    {
        Sequence closeSeq = DOTween.Sequence();
        closeSeq.Append(targetObject.transform.DOScale(Vector3.zero, tweenDuration).SetEase(Ease.InBack));
        closeSeq.Join(spriteRenderer.DOFade(0f, tweenDuration));

        if (waitUntilFinished)
        {
            closeSeq.OnComplete(() =>
            {
                targetObject.SetActive(false);
                Continue();
            });
        }
        else
        {
            closeSeq.OnComplete(() => targetObject.SetActive(false));
            Continue();
        }
    }

    public override string GetSummary()
    {
        return targetObject != null ? targetObject.name : "δָ������";
    }

    public override Color GetButtonColor()
    {
        return new Color(0.6f, 0.8f, 1f); // ǳ��ɫ
    }
}
