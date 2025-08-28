using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class BehaviorTreeManager : MonoBehaviour
{
    public BehaviorTree currentBehavior;           // ��ǰ�������е���Ϊ��
    public ExternalBehaviorTree normalBehaviorTree;   // Ҫ�л���������Ϊ��
    public ExternalBehaviorTree hungryBehaviorTree;   // Ҫ�л���������Ϊ��

    private void Start()
    {
        currentBehavior = GetComponent<BehaviorTree>();

    }

    private void Update()
    {
        // �� Update �п���ͨ��ĳЩ�������л���Ϊ��
        if (Input.GetKeyDown(KeyCode.T))   // ʾ�������� T ��ʱ�л���Ϊ��
        {
            ReplaceBehaviorTree(hungryBehaviorTree);
        }
    }

    // �滻��Ϊ���ķ���
    public void ReplaceBehaviorTree(ExternalBehaviorTree newTree)
    {
        if (currentBehavior != null)
        {
            currentBehavior.DisableBehavior();  // ֹͣ��ǰ��Ϊ��
            currentBehavior.ExternalBehavior = newTree; // �����µ���Ϊ��
            currentBehavior.EnableBehavior();   // ��������Ϊ��
            Debug.Log("��Ϊ���滻Ϊ"+ newTree.name);
        }
    }
}
