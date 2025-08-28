using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class CharacterState : MonoBehaviour
{
    //表示行动总览，包含多个CharState
    public enum CharAction
    {
        WaitAction,
        ReadAction,
        SleepAction,
        PatrolAction,
        ChatWaitAction,
        ChatAction,
        InteractAction,
        ControlAction
    }

    //表示状态，决定动画
    public enum CharState
    {
        Idle,
        Walk,
        Read,
        Eat,
        Sleep,
        Sit1,
        Sit2
    }

    private HashSet<CharState> normalStates = new HashSet<CharState>
    {
        CharState.Idle,
        CharState.Walk,
    };

    [LabelText("当前状态")]
    public CharState currentState = CharState.Idle;

    [LabelText("先前状态")]
    public CharState previousState;

    [LabelText("当前行动")]
    public CharAction currentAction;

    public Animator anim;

    // 修改后的 ChangeState 方法，支持淡入和淡出时间
    public void ChangeState(CharState newState, float fadeInTime = 0f, float fadeOutTime = 0f)
    {
        previousState = currentState;
        currentState = newState;

        // 获取对应的动画名称
        string animationName = GetAnimationNameForState(newState);

        // 如果指定了淡出时间，则先执行淡出操作
        if (fadeOutTime > 0f)
        {
            FadeOutAndInSpriteRenderers(animationName, fadeInTime, fadeOutTime);
        }
        else
        {
            // 如果没有指定淡出时间，直接切换状态
            SetStateWithoutFade(animationName);
        }
    }

    private void SetStateWithoutFade(string animationName)
    {
        ResetAnimation(); // 重置动画
        anim.SetBool(animationName, true); // 直接根据动画名称设置
    }

    private void ResetAnimation()
    {
        anim.SetBool("Idle1", false);
        anim.SetBool("Walk", false);
        anim.SetBool("Sit2", false);
        anim.SetBool("Read", false);
        anim.SetBool("Sleep", false);
    }

    // 获取状态对应的动画名称
    private string GetAnimationNameForState(CharState state)
    {
        switch (state)
        {
            case CharState.Idle:
                return "Idle1";
            case CharState.Walk:
                return "Walk";
            case CharState.Read:
                return "Read";
            case CharState.Sit2:
                return "Sit2";
            case CharState.Sleep:
                return "Sleep";
            default:
                return "Idle1"; // 默认值
        }
    }

    public void ChangeAction(CharAction newAction)
    {
        currentAction = newAction;
    }

    private void PlayRandomIdleAnimation()
    {
        int idleAnimationIndex = Random.Range(0, 100); // 生成0或1的随机数

        if (idleAnimationIndex > 0 && idleAnimationIndex <= 50)
        {
            anim.SetBool("Idle1", true);
        }
        else if (idleAnimationIndex > 50 && idleAnimationIndex <= 70)
        {
            anim.SetBool("Idle2", true);
        }
        else if (idleAnimationIndex > 70)
        {
            anim.SetBool("Idle3", true);
        }
    }

    // 判断是否在正常状态中
    public bool IsInNormalState()
    {
        return normalStates.Contains(currentState);
    }

    public void FadeOutAndInSpriteRenderers(string animationName, float fadeInTime, float fadeOutTime)
    {
        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(true));

        bool hasSpriteRenderers = spriteRenderers.Count > 0;

        // 执行淡出和淡入操作，保持现有的逻辑
        if (hasSpriteRenderers)
        {
            Sequence fadeOutSequence = DOTween.Sequence();

            // 执行淡出
            foreach (var spriteRenderer in spriteRenderers)
            {
                fadeOutSequence.Join(spriteRenderer.DOFade(0f, fadeOutTime));
            }

            fadeOutSequence.OnComplete(() =>
            {
                ResetAnimation(); // 重置动画
                anim.SetBool(animationName, true);

                // 执行淡入
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.DOFade(1f, fadeInTime);
                }
            });
        }
        else
        {
            // 如果没有 SpriteRenderer，直接切换状态
            ResetAnimation(); // 重置动画
            anim.SetBool(animationName, true);
        }
    }
}
