using UnityEngine;
using Fungus;

[CommandInfo("Custom", "播放动画", "播放一个Animator动画，可以选择是否等待动画播放完毕再继续，并支持莉莉娅或自定义模式")]

public class PlayAnimationCommand : Command
{
    public enum AnimatorSource
    {
        Liliya,    // 自动找tag为Player的物体
        Custom     // 手动指定Animator
    }

    [Tooltip("选择动画控制器来源")]
    public AnimatorSource animatorSource = AnimatorSource.Liliya;

    [Tooltip("手动指定Animator（仅Custom模式有效）")]
    public Animator customAnimator;

    [Tooltip("动画状态名")]
    public string stateName;

    [Tooltip("等待动画完成后继续执行后续命令")]
    public bool waitUntilFinished = false;

    private Animator targetAnimator;

    public override void OnEnter()
    {
        if (animatorSource == AnimatorSource.Liliya)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                targetAnimator = player.GetComponent<Animator>();
        }
        else // Custom
        {
            targetAnimator = customAnimator;
        }

        if (targetAnimator == null)
        {
            Debug.LogWarning("播放动画失败：未找到目标 Animator");
            Continue();
            return;
        }

        if (string.IsNullOrEmpty(stateName))
        {
            Debug.LogWarning("播放动画失败：未指定动画状态名");
            Continue();
            return;
        }

        targetAnimator.Play(stateName);

        if (waitUntilFinished)
        {
            StartCoroutine(WaitAnimationEnd());
        }
        else
        {
            Continue();
        }
    }

    private System.Collections.IEnumerator WaitAnimationEnd()
    {
        // 等待当前动画状态播放完毕
        var stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0);
        float length = stateInfo.length;

        yield return new WaitForSeconds(length);

        Continue();
    }

    public override string GetSummary()
    {
        string animatorDesc = animatorSource == AnimatorSource.Liliya ? "莉莉娅(Player tag)" : (customAnimator != null ? customAnimator.name : "未指定");
        if (string.IsNullOrEmpty(stateName))
            return "未指定动画状态名";
        return $"播放动画 [{stateName}]，来源: {animatorDesc}，等待完成: {waitUntilFinished}";
    }
}
