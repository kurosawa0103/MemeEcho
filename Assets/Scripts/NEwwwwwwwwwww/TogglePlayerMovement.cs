using UnityEngine;
using Fungus;

[CommandInfo("Player", "Toggle Player Movement", "���û��������ƶ����Զ����� MovementSystem��")]
public class TogglePlayerMovement : Command
{
    [Tooltip("��ѡ = �����ƶ���ȡ����ѡ = ��ֹ�ƶ�")]
    public bool canMove = true;

    private MovementSystem player;

    public override void OnEnter()
    {
        // �Զ����ҳ����е� MovementSystem
        if (player == null)
        {
            player = GameObject.FindObjectOfType<MovementSystem>();
        }

        if (player != null)
        {
            player.SetMovementEnabled(canMove);
        }
        else
        {
            Debug.LogWarning("TogglePlayerMovement: �������Ҳ��� MovementSystem��");
        }

        Continue();
    }

    public override string GetSummary()
    {
        return (canMove ? "�����ƶ�" : "��ֹ�ƶ�");
    }
}
