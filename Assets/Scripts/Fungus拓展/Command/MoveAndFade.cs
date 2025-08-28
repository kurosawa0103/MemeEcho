using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using DG.Tweening;
using Sirenix.OdinInspector;

[CommandInfo("Custom", "Move and Fade", "Moves an object to a specified position with fade out, fade in, and optional animation.")]
public class MoveAndFade : Command
{
    [Tooltip("The GameObject to move and fade.")]
    public GameObject targetObject;

    [Tooltip("The target position to move the object to.")]
    public Transform targetPosition;

    [Tooltip("The duration for the fade out and fade in.")]
    public float fadeInDuration = 0.5f;
    [Tooltip("The duration for the fade out and fade in.")]
    public float fadeOutDuration = 0.5f;

    [Tooltip("The name of the animation to play.")]
    public string animationName;

    [Header("是否修改状态")]
    [LabelText("修改状态")]
    public bool changeState; // 是否修改状态
    [ShowIf("changeState")]
    [LabelText("新状态")]
    public CharacterState.CharState newState; // 选择新的角色状态

    // 新增的朝向枚举
    public enum FacingDirection { Left, Right }
    [Tooltip("The facing direction of the object.")]
    public FacingDirection facingDirection = FacingDirection.Right;  // 默认右边

    // 新增的枚举来控制不同的淡入淡出类型
    public enum FadeType { FadeOut, FadeIn, FadeOutAndIn }
    [Tooltip("The type of fade behavior.")]
    public FadeType fadeType = FadeType.FadeOutAndIn;


    public bool waitFinifshed = false;

    public override void OnEnter()
    {
        // 如果 targetObject 为空，则默认查找 Tag 为 "Player" 的物体
        if (targetObject == null)
        {
            targetObject = GameObject.FindGameObjectWithTag("Player");
        }

        // Get all SpriteRenderers in the target object and its children
        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>(targetObject.GetComponentsInChildren<SpriteRenderer>(true));

        // Check if there are any SpriteRenderers to fade in/out
        bool hasSpriteRenderers = spriteRenderers.Count > 0;

        if (waitFinifshed)
        {
            // 立即执行下一条命令
            Continue();
        }

        // 根据选择的 fadeType 执行不同的淡入淡出逻辑
        if (fadeType == FadeType.FadeOut)
        {
            // 仅执行淡出操作
            if (hasSpriteRenderers)
            {
                Sequence fadeOutSequence = DOTween.Sequence();
                foreach (var spriteRenderer in spriteRenderers)
                {
                    fadeOutSequence.Join(spriteRenderer.DOFade(0f, fadeOutDuration));
                }
                fadeOutSequence.OnComplete(() => Continue());
            }
            else
            {
                Continue(); // 如果没有 SpriteRenderer，直接继续
            }
        }
        else if (fadeType == FadeType.FadeIn)
        {
            // Play animation after fade out
            PlayAnimation();
            //切换状态
            ChangeState();
            // 修改物体朝向
            SetFacingDirection(facingDirection);
            // Proceed with moving the object and fading in
            MoveAndFadeObject();
        }
        else if (fadeType == FadeType.FadeOutAndIn)
        {
            // 执行淡出和淡入操作，保持现有的逻辑
            if (hasSpriteRenderers)
            {
                Sequence fadeOutSequence = DOTween.Sequence();
                foreach (var spriteRenderer in spriteRenderers)
                {
                    fadeOutSequence.Join(spriteRenderer.DOFade(0f, fadeOutDuration));
                }

                fadeOutSequence.OnComplete(() =>
                {
                    // Play animation after fade out
                    PlayAnimation();
                    //切换状态
                    ChangeState();
                    // 修改物体朝向
                    SetFacingDirection(facingDirection);
                    // Proceed with moving the object and fading in
                    MoveAndFadeObject();
                });
            }
            else
            {
                // 如果没有 SpriteRenderers，直接执行动画和移动
                PlayAnimation();
                MoveAndFadeObject();
            }
        }
    }


    private void ChangeState()
    {
        // 确保目标对象是 Player 才修改状态
        if (changeState && targetObject.CompareTag("Player"))
        {
            CharacterState characterState = targetObject.GetComponent<CharacterState>();
            if (characterState != null)
            {
                characterState.ChangeState(newState);
            }
            else
            {
                Debug.LogWarning("目标对象没有 CharacterState 组件，无法更改状态！");
            }
        }
    }
    private void PlayAnimation()
    {
        // Check if the target object has an Animator and play the animation if it does
        Animator animator = targetObject.GetComponent<Animator>();
        if (animator != null && !string.IsNullOrEmpty(animationName))
        {
            animator.Play(animationName);
        }
    }

    // Function to move the object and fade back in after the movement
    private void MoveAndFadeObject()
    {
        // If targetPosition is not null or Vector3.zero, move the object
        if (targetPosition != null && targetPosition.localPosition != Vector3.zero)
        {
            // Perform the move instantly (no transition)
            targetObject.transform.position = targetPosition.position;
        }

        // After the movement is complete, fade in all SpriteRenderers (if they exist)
        FadeInSpriteRenderers();
    }

    // Function to fade in all SpriteRenderers
    private void FadeInSpriteRenderers()
    {
        // Get all SpriteRenderers in the target object and its children
        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>(targetObject.GetComponentsInChildren<SpriteRenderer>(true));

        // Check if there are any SpriteRenderers to fade in
        bool hasSpriteRenderers = spriteRenderers.Count > 0;

        if (hasSpriteRenderers)
        {
            // Fade in all SpriteRenderers
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.DOFade(1f, fadeInDuration);
            }
        }

        // Continue with the flow after fading in
        Continue();
    }

    // 新增的函数来设置物体朝向
    private void SetFacingDirection(FacingDirection direction)
    {
        if (targetObject != null)
        {
            Vector3 scale = targetObject.transform.localScale;

            // 根据朝向设置localScale.x来翻转物体
            if (direction == FacingDirection.Left)
            {
                scale.x = -Mathf.Abs(scale.x);  // 使物体朝左
            }
            else if (direction == FacingDirection.Right)
            {
                scale.x = Mathf.Abs(scale.x); // 使物体朝右
            }

            targetObject.transform.localScale = scale;
        }
    }

    public override Color GetButtonColor()
    {
        return new Color32(153, 141, 190, 255);
    }

    public override string GetSummary()
    {
        // 如果 targetObject 为空，默认使用 "Player"
        string targetName = targetObject != null ? targetObject.name : "Player";

        // 目标位置信息
        string positionInfo = targetPosition != null ? $"到 {targetPosition.name}" : "(No target position set)";

        // 动画信息
        string animationInfo = !string.IsNullOrEmpty(animationName) ? $" 播放动画: {animationName}" : "";

        // 状态变更信息
        string stateInfo = changeState ? $" 改变状态 {newState}" : "";

        return $"移动 {targetName} {positionInfo}{animationInfo}{stateInfo}.";
    }

}
