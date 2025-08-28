using UnityEngine;
using Fungus;

[CommandInfo("Custom", "Set Player Parent", "��������Ƿ��ϰ��������丸����")]
public class SetPlayerParent : Command
{
    [Tooltip("��ѡ������ϰ�����Ϊ����ʱ����������������Ϊ���������塱")]
    public bool isPlayerLanding;

    [Tooltip("������ã��򱣳��������겻��")]
    public bool worldPositionStays = true;

    public override void OnEnter()
    {
        // ����������壨Tag Ϊ "Player"��
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("SetPlayerParent Command: δ�ҵ� Player ���壡");
            Continue();
            return;
        }

        // ȷ���µĸ���
        string parentName = isPlayerLanding ? "��ʱ����" : "��������";
        Transform newParent = GameObject.Find(parentName)?.transform;

        if (newParent == null)
        {
            Debug.LogWarning($"SetPlayerParent Command: δ�ҵ� '{parentName}' ���壬�޷����ø�����");
            Continue();
            return;
        }

        // ������ҵĸ���
        player.transform.SetParent(newParent, worldPositionStays);
        Debug.Log($"SetPlayerParent Command: {player.name} ������ {newParent.name} ���Ӽ�");

        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(77, 255, 183, 255); // ��ɫ
    }

    public override string GetSummary()
    {
        return $"Player �� {(isPlayerLanding ? "��ʱ����" : "��������")}";
    }
}
