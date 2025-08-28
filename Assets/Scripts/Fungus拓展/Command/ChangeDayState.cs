using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Custom", "Change Day State", "Changes the day state of the game to a specified state.")]
public class ChangeDayState : Command
{
    [Tooltip("The target DayState to set.")]
    public DayState newDayState;

    public float fadeTime=2;
    public override void OnEnter()
    {
        TimeSystem timeSystem = FindObjectOfType<TimeSystem>();  // ��ȡ TimeSystem ʵ��

        if (timeSystem != null)
        {
            timeSystem.SetDayState(newDayState, fadeTime);  // ���� SetDayState �����л���ҹ״̬
        }
        else
        {
            Debug.LogWarning("TimeSystem not found in the scene.");
        }

        Continue();  // ����ִ�н�����������
    }

    // ��ȡ������ܽ���Ϣ
    public override string GetSummary()
    {
        return $"Set day state to {newDayState.ToString()}";  // ����ѡ������ҹ״̬��Ϊ�ܽ�
    }
}
