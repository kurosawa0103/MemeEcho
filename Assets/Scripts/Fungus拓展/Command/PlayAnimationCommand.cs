using UnityEngine;
using Fungus;

[CommandInfo("Custom", "���Ŷ���", "����һ��Animator����������ѡ���Ƿ�ȴ�������������ټ�������֧������櫻��Զ���ģʽ")]

public class PlayAnimationCommand : Command
{
    public enum AnimatorSource
    {
        Liliya,    // �Զ���tagΪPlayer������
        Custom     // �ֶ�ָ��Animator
    }

    [Tooltip("ѡ�񶯻���������Դ")]
    public AnimatorSource animatorSource = AnimatorSource.Liliya;

    [Tooltip("�ֶ�ָ��Animator����Customģʽ��Ч��")]
    public Animator customAnimator;

    [Tooltip("����״̬��")]
    public string stateName;

    [Tooltip("�ȴ�������ɺ����ִ�к�������")]
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
            Debug.LogWarning("���Ŷ���ʧ�ܣ�δ�ҵ�Ŀ�� Animator");
            Continue();
            return;
        }

        if (string.IsNullOrEmpty(stateName))
        {
            Debug.LogWarning("���Ŷ���ʧ�ܣ�δָ������״̬��");
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
        // �ȴ���ǰ����״̬�������
        var stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0);
        float length = stateInfo.length;

        yield return new WaitForSeconds(length);

        Continue();
    }

    public override string GetSummary()
    {
        string animatorDesc = animatorSource == AnimatorSource.Liliya ? "�����(Player tag)" : (customAnimator != null ? customAnimator.name : "δָ��");
        if (string.IsNullOrEmpty(stateName))
            return "δָ������״̬��";
        return $"���Ŷ��� [{stateName}]����Դ: {animatorDesc}���ȴ����: {waitUntilFinished}";
    }
}
