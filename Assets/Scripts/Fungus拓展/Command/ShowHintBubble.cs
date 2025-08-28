using UnityEngine;
using Fungus;
using System.Collections.Generic;
using System.Collections;

[CommandInfo("�Զ�������", "ShowHintBubble", "����Ŀ���ɫ�Ƿ�������ʾ����")]
public class ShowHintBubble : Command
{
    [Tooltip("Ŀ���ɫ��������� CharacterBehavior �ű�")]
    public GameObject target;

    [Tooltip("�Ƿ�������ʾ����")]
    public bool enableHint = true;

    public float durationTime = 0.4f;
    public override void OnEnter()
    {
        if (target != null)
        {
            CharacterObject cb = target.GetComponent<CharacterObject>();
            if (cb != null)
            {
                cb.canShow = enableHint;

                if (enableHint)
                {
                    // ��Ҫ��ʾ����
                    if (cb.speechBubble != null && !cb.speechBubble.activeSelf)
                    {
                        cb.ShowBubble();
                    }
                    Continue(); // ��ʾ���ݶ���һ�����첽�����������̣�����ֱ�Ӽ���
                }
                else
                {
                    // ��Ҫ��������
                    if (cb.speechBubble != null && cb.speechBubble.activeSelf)
                    {
                        cb.HideBubble();
                        // �ȶ���ʱ�����ټ���
                        StartCoroutine(WaitAndContinue(durationTime));
                        return;  // ���ﷵ�أ��ȴ��������
                    }
                    else
                    {
                        Continue(); // ���ݱ��������أ�ֱ�Ӽ���
                    }
                }
            }
            else
            {
                Debug.LogWarning("Ŀ�����δ���� CharacterBehavior �ű���" + target.name);
                Continue();
            }
        }
        else
        {
            Debug.LogWarning("ShowHintBubble ָ���Ŀ�����Ϊ�գ�");
            Continue();
        }
    }


    private IEnumerator WaitAndContinue(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Continue();
    }



    public override string GetSummary()
    {
        return target != null
            ? $"���� {target.name} ��������ʾ = {enableHint}"
            : "δ����Ŀ�����";
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255); // ����ɫ
    }
}
