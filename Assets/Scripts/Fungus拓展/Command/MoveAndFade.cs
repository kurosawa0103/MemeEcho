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

    [Header("�Ƿ��޸�״̬")]
    [LabelText("�޸�״̬")]
    public bool changeState; // �Ƿ��޸�״̬
    [ShowIf("changeState")]
    [LabelText("��״̬")]
    public CharacterState.CharState newState; // ѡ���µĽ�ɫ״̬

    // �����ĳ���ö��
    public enum FacingDirection { Left, Right }
    [Tooltip("The facing direction of the object.")]
    public FacingDirection facingDirection = FacingDirection.Right;  // Ĭ���ұ�

    // ������ö�������Ʋ�ͬ�ĵ��뵭������
    public enum FadeType { FadeOut, FadeIn, FadeOutAndIn }
    [Tooltip("The type of fade behavior.")]
    public FadeType fadeType = FadeType.FadeOutAndIn;


    public bool waitFinifshed = false;

    public override void OnEnter()
    {
        // ��� targetObject Ϊ�գ���Ĭ�ϲ��� Tag Ϊ "Player" ������
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
            // ����ִ����һ������
            Continue();
        }

        // ����ѡ��� fadeType ִ�в�ͬ�ĵ��뵭���߼�
        if (fadeType == FadeType.FadeOut)
        {
            // ��ִ�е�������
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
                Continue(); // ���û�� SpriteRenderer��ֱ�Ӽ���
            }
        }
        else if (fadeType == FadeType.FadeIn)
        {
            // Play animation after fade out
            PlayAnimation();
            //�л�״̬
            ChangeState();
            // �޸����峯��
            SetFacingDirection(facingDirection);
            // Proceed with moving the object and fading in
            MoveAndFadeObject();
        }
        else if (fadeType == FadeType.FadeOutAndIn)
        {
            // ִ�е����͵���������������е��߼�
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
                    //�л�״̬
                    ChangeState();
                    // �޸����峯��
                    SetFacingDirection(facingDirection);
                    // Proceed with moving the object and fading in
                    MoveAndFadeObject();
                });
            }
            else
            {
                // ���û�� SpriteRenderers��ֱ��ִ�ж������ƶ�
                PlayAnimation();
                MoveAndFadeObject();
            }
        }
    }


    private void ChangeState()
    {
        // ȷ��Ŀ������� Player ���޸�״̬
        if (changeState && targetObject.CompareTag("Player"))
        {
            CharacterState characterState = targetObject.GetComponent<CharacterState>();
            if (characterState != null)
            {
                characterState.ChangeState(newState);
            }
            else
            {
                Debug.LogWarning("Ŀ�����û�� CharacterState ������޷�����״̬��");
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

    // �����ĺ������������峯��
    private void SetFacingDirection(FacingDirection direction)
    {
        if (targetObject != null)
        {
            Vector3 scale = targetObject.transform.localScale;

            // ���ݳ�������localScale.x����ת����
            if (direction == FacingDirection.Left)
            {
                scale.x = -Mathf.Abs(scale.x);  // ʹ���峯��
            }
            else if (direction == FacingDirection.Right)
            {
                scale.x = Mathf.Abs(scale.x); // ʹ���峯��
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
        // ��� targetObject Ϊ�գ�Ĭ��ʹ�� "Player"
        string targetName = targetObject != null ? targetObject.name : "Player";

        // Ŀ��λ����Ϣ
        string positionInfo = targetPosition != null ? $"�� {targetPosition.name}" : "(No target position set)";

        // ������Ϣ
        string animationInfo = !string.IsNullOrEmpty(animationName) ? $" ���Ŷ���: {animationName}" : "";

        // ״̬�����Ϣ
        string stateInfo = changeState ? $" �ı�״̬ {newState}" : "";

        return $"�ƶ� {targetName} {positionInfo}{animationInfo}{stateInfo}.";
    }

}
