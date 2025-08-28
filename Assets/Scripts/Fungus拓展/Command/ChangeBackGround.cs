using UnityEngine;
using Fungus;
using DG.Tweening;
using System.Collections.Generic;

[CommandInfo("自定义指令", "背景替换（ChangeBackGround）", "使用预制体替换背景，所有 SpriteRenderer 淡入，同时旧背景淡出销毁")]
public class ChangeBackGround : Command
{
    [Tooltip("新背景预制体")]
    public GameObject newBackgroundPrefab;

    [Tooltip("父级对象的Tag")]
    public string parentTag = "Heart";

    [Tooltip("淡入淡出时间")]
    public float fadeDuration = 1f;

    [Tooltip("是否等待完成后继续执行")]
    public bool waitForCompletion = true;

    public override void OnEnter()
    {
        if (newBackgroundPrefab == null)
        {
            Debug.LogWarning("ChangeBackGround：请设置背景预制体！");
            Continue();
            return;
        }

        GameObject parentObj = GameObject.FindWithTag(parentTag);
        if (parentObj == null)
        {
            Debug.LogWarning($"ChangeBackGround：未找到 Tag 为 {parentTag} 的物体！");
            Continue();
            return;
        }

        Transform parentTransform = parentObj.transform;

        // 获取旧背景 GameObject（假设每次背景预制体都只挂一个子物体，或使用特定标记/命名）
        // 你可以通过名字/Tag等方式识别背景根
        List<GameObject> oldBackgroundRoots = new List<GameObject>();
        foreach (Transform child in parentTransform)
        {
            if (child.GetComponent<SpriteRenderer>() != null || child.GetComponentInChildren<SpriteRenderer>() != null)
            {
                oldBackgroundRoots.Add(child.gameObject);
            }
        }

        // 实例化新背景
        GameObject newBg = Object.Instantiate(newBackgroundPrefab, parentTransform);

        // 获取新背景内的所有 SpriteRenderer
        SpriteRenderer[] newRenderers = newBg.GetComponentsInChildren<SpriteRenderer>();

        List<Tween> tweens = new List<Tween>();

        foreach (var sr in newRenderers)
        {
            sr.color = new Color(1f, 1f, 1f, 0f);
            tweens.Add(sr.DOFade(1f, fadeDuration));
        }

        foreach (var oldRoot in oldBackgroundRoots)
        {
            SpriteRenderer[] oldRenderers = oldRoot.GetComponentsInChildren<SpriteRenderer>();
            foreach (var oldSr in oldRenderers)
            {
                tweens.Add(oldSr.DOFade(0f, fadeDuration));
            }

            // 延迟销毁整个背景根节点
            DOVirtual.DelayedCall(fadeDuration, () =>
            {
                Object.Destroy(oldRoot);
            });
        }

        if (waitForCompletion)
        {
            DOTween.Sequence()
                .AppendInterval(fadeDuration)
                .OnComplete(() => Continue());
        }
        else
        {
            Continue();
        }
    }


    public override string GetSummary()
    {
        return newBackgroundPrefab != null ? $"切换背景到：{newBackgroundPrefab.name}" : "未设置新背景预制体";
    }

    public override Color GetButtonColor()
    {
        return new Color32(180, 220, 255, 255);
    }
}
