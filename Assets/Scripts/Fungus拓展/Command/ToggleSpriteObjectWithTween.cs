using UnityEngine;
using Fungus;
using DG.Tweening;

[CommandInfo("自定义扩展", "ToggleSpriteObjectWithTween", "开关Sprite物体，带缩放+淡入动画")]
public class ToggleSpriteObjectWithTween : Command
{
    [Tooltip("要控制的物体 (需要有SpriteRenderer)")]
    public GameObject targetObject;

    [Tooltip("动效持续时间")]
    public float tweenDuration = 0.5f;

    [Tooltip("是否等待动画播放完成再继续流程")]
    public bool waitUntilFinished = true;

    [Tooltip("强制关闭，无论当前状态")]
    public bool forceClose = false;

    private SpriteRenderer spriteRenderer;

    public override void OnEnter()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("没有设置目标物体！");
            Continue();
            return;
        }

        spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("目标物体上没有SpriteRenderer！");
            Continue();
            return;
        }

        if (forceClose)
        {
            Debug.Log("[ToggleSpriteObjectWithTween] 强制关闭目标物体。");
            if (targetObject.activeSelf)
            {
                CloseObject();
            }
            else
            {
                // 如果已经是关闭状态，也要执行隐藏逻辑（防止淡入状态残留）
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
            // 正常切换逻辑
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

        // 设置初始透明度为0
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
        return targetObject != null ? targetObject.name : "未指定物体";
    }

    public override Color GetButtonColor()
    {
        return new Color(0.6f, 0.8f, 1f); // 浅蓝色
    }
}
