using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class CharacterState : MonoBehaviour
{
    //��ʾ�ж��������������CharState
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

    //��ʾ״̬����������
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

    [LabelText("��ǰ״̬")]
    public CharState currentState = CharState.Idle;

    [LabelText("��ǰ״̬")]
    public CharState previousState;

    [LabelText("��ǰ�ж�")]
    public CharAction currentAction;

    public Animator anim;

    // �޸ĺ�� ChangeState ������֧�ֵ���͵���ʱ��
    public void ChangeState(CharState newState, float fadeInTime = 0f, float fadeOutTime = 0f)
    {
        previousState = currentState;
        currentState = newState;

        // ��ȡ��Ӧ�Ķ�������
        string animationName = GetAnimationNameForState(newState);

        // ���ָ���˵���ʱ�䣬����ִ�е�������
        if (fadeOutTime > 0f)
        {
            FadeOutAndInSpriteRenderers(animationName, fadeInTime, fadeOutTime);
        }
        else
        {
            // ���û��ָ������ʱ�䣬ֱ���л�״̬
            SetStateWithoutFade(animationName);
        }
    }

    private void SetStateWithoutFade(string animationName)
    {
        ResetAnimation(); // ���ö���
        anim.SetBool(animationName, true); // ֱ�Ӹ��ݶ�����������
    }

    private void ResetAnimation()
    {
        anim.SetBool("Idle1", false);
        anim.SetBool("Walk", false);
        anim.SetBool("Sit2", false);
        anim.SetBool("Read", false);
        anim.SetBool("Sleep", false);
    }

    // ��ȡ״̬��Ӧ�Ķ�������
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
                return "Idle1"; // Ĭ��ֵ
        }
    }

    public void ChangeAction(CharAction newAction)
    {
        currentAction = newAction;
    }

    private void PlayRandomIdleAnimation()
    {
        int idleAnimationIndex = Random.Range(0, 100); // ����0��1�������

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

    // �ж��Ƿ�������״̬��
    public bool IsInNormalState()
    {
        return normalStates.Contains(currentState);
    }

    public void FadeOutAndInSpriteRenderers(string animationName, float fadeInTime, float fadeOutTime)
    {
        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(true));

        bool hasSpriteRenderers = spriteRenderers.Count > 0;

        // ִ�е����͵���������������е��߼�
        if (hasSpriteRenderers)
        {
            Sequence fadeOutSequence = DOTween.Sequence();

            // ִ�е���
            foreach (var spriteRenderer in spriteRenderers)
            {
                fadeOutSequence.Join(spriteRenderer.DOFade(0f, fadeOutTime));
            }

            fadeOutSequence.OnComplete(() =>
            {
                ResetAnimation(); // ���ö���
                anim.SetBool(animationName, true);

                // ִ�е���
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.DOFade(1f, fadeInTime);
                }
            });
        }
        else
        {
            // ���û�� SpriteRenderer��ֱ���л�״̬
            ResetAnimation(); // ���ö���
            anim.SetBool(animationName, true);
        }
    }
}
