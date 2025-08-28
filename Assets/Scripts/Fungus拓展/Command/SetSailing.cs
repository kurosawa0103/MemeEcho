using Fungus;
using UnityEngine;

[CommandInfo("Custom",
             "Set Sailing",
             "�����Ƿ���")]
public class SetSailing : Command
{
    [Tooltip("����״̬���ƣ����û���ã�")]
    public bool isSailing;

    [Tooltip("Sailor �ű�����")]
    private Sailor sailor;

    [Tooltip("����Ч�����ڵ����ã�����еĻ���")]
    public Transform rippleEffectParent; // ���ڵ㣬���ڲ�����������ϵͳ

    public override void OnEnter()
    {
        // �Զ�Ѱ�� Sailor �ű�����ѡ��
        if (sailor == null)
        {
            sailor = GameObject.FindObjectOfType<Sailor>();
        }

        if (sailor != null)
        {
            sailor.isSailing = isSailing; // ���ú���״̬
        }
        else
        {
            Debug.LogWarning("δ�ҵ� Sailor �ű���");
        }

        // �������ڵ��µ����������壬���� ParticleSystem
        if (rippleEffectParent != null)
        {
            ParticleSystem[] particleSystems = rippleEffectParent.GetComponentsInChildren<ParticleSystem>();

            if (!isSailing)
            {
                // ֹͣ����ʱ�ر���������Ч��
                foreach (var particleSystem in particleSystems)
                {
                    particleSystem.Stop();
                }
            }
            else
            {
                // ��������ʱ������������Ч��
                foreach (var particleSystem in particleSystems)
                {
                    particleSystem.Play();
                }
            }
        }

        Continue();
    }

    public override string GetSummary()
    {
        return sailor != null
            ? $"���� {sailor.name} ����״̬Ϊ��{(isSailing ? "����" : "����")}"
            : "Sailor δָ��";
    }
}
