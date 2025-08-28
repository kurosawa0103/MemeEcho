using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using BehaviorDesigner.Runtime; // Behavior Tree ����������ռ�

[CommandInfo("AI", "Active AI", "���û����Ŀ������ϵ���Ϊ��������ѡ���л� AI ��Ϊ��")]
public class ActiveAi : Command
{
    public enum TargetType
    {
        Player,  // Ŀ���ǳ����е� Player
        Custom   // Ŀ�����ֶ�ָ���Ķ���
    }

    [SerializeField] private bool enableAI = true; // ��ѡ�����ã�ȡ�������
    [SerializeField] private TargetType targetType = TargetType.Player; // ѡ��Ŀ������
    [SerializeField] private GameObject customTarget; // �Զ���Ŀ�꣨���� TargetType = Custom ʱʹ�ã�

    [Header("AI �л�����")]
    [SerializeField] private bool switchAI = false; // �Ƿ��л� AI
    [SerializeField] private ExternalBehaviorTree newExternalBehaviorTree; // �µ��ⲿ AI ��Ϊ��

    public override void OnEnter()
    {
        GameObject targetObject = null;

        if (targetType == TargetType.Player)
        {
            targetObject = GameObject.FindGameObjectWithTag("Player"); // ��ȡ Player
        }
        else if (targetType == TargetType.Custom && customTarget != null)
        {
            targetObject = customTarget;
        }

        if (targetObject != null)
        {
            // **��ȡ BehaviorTree ���**
            BehaviorTree behaviorTree = targetObject.GetComponent<BehaviorTree>();

            if (behaviorTree != null)
            {
                if (switchAI && newExternalBehaviorTree != null)
                {
                    // �Ƚ��� BehaviorTree �������
                    behaviorTree.enabled = false;

                    // �л����µ� External Behavior
                    Debug.Log($"�л� AI: {behaviorTree.ExternalBehavior} -> {newExternalBehaviorTree}");
                    behaviorTree.ExternalBehavior = newExternalBehaviorTree;

                    // �������� BehaviorTree��������Ϊ����Ч
                    behaviorTree.enabled = true;
                }


                // **���û���� AI**
                behaviorTree.enabled = enableAI;
            }
            else
            {
                Debug.LogWarning("Ŀ�������û���ҵ� BehaviorTree �����");
            }

            // **��ȡ CharacterState ���**
            CharacterState characterState = targetObject.GetComponent<CharacterState>();

            if (characterState != null)
            {
                if (!enableAI)
                {
                    // **AI �ر�ʱ��ǿ�ƽ��� Idle ״̬**
                    characterState.ChangeState(CharacterState.CharState.Idle);
                }
            }
            else
            {
                Debug.LogWarning("Ŀ�������û���ҵ� CharacterState �����");
            }
        }
        else
        {
            Debug.LogWarning("δ�ҵ�Ŀ������������ã�");
        }

        Continue(); // ����ִ����һ������
    }

    public override string GetSummary()
    {
        string targetName = targetType == TargetType.Player ? "Player" : (customTarget != null ? customTarget.name : "δָ��");
        string aiStatus = enableAI ? "����" : "����";
        string aiSwitchInfo = switchAI ? $"�л��� {newExternalBehaviorTree?.name}" : "���ֵ�ǰ AI";

        return $"Ŀ��: {targetName} | AI״̬: {aiStatus} | {aiSwitchInfo}";
    }
}
