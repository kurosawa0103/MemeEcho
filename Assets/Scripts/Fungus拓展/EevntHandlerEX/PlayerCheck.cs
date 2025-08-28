using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fungus;

[EventHandlerInfo("��չ",
                  "Player Check",
                  "������ָ��Tag��������봥������ִ��")]
public class PlayerCheck : EventHandler
{
    [Header("��������Tag")]
    [Tooltip("���ü���Tag������ 'Player'")]
    [SerializeField] private string tagToCheck = "Player";

    [Header("�ӳ�ִ�е�֡��")]
    [Tooltip("�ڴ������ӳ�ָ��֡����ִ�п�")]
    [SerializeField] private int delayFrames = 0;

    [Header("ֻ������һ��")]
    [Tooltip("�����ѡ�������󲻻��ٴ���Ӧ")]
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered = false; // �Ƿ��Ѿ�������

    private void OnTriggerEnter(Collider other)
    {
        // ��������Tag�Ƿ�ƥ��
        if (other.CompareTag(tagToCheck))
        {
            if (triggerOnce && hasTriggered)
            {
                Debug.Log("�Ѿ������������Ժ�������");
                return;
            }

            Debug.Log($"Tagƥ��: {tagToCheck}�������¼�");
            hasTriggered = true;

            // �����ӳ�ִ�п��Э��
            StartCoroutine(ExecuteBlockWithDelay());
        }
    }

    private IEnumerator ExecuteBlockWithDelay()
    {
        // �ȴ�ָ��֡��
        for (int i = 0; i < delayFrames; i++)
        {
            yield return null;
        }

        // ִ�й����� Fungus ��
        ExecuteBlock();
    }

    private void OnDisable()
    {
        Debug.Log("PlayerCheck �ѽ��ã����ô���״̬");
        hasTriggered = false; // ���ô���״̬�Ա���������ʱ�����ٴδ���
    }

}
