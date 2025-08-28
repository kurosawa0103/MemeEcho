using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public string playerTag = "Player"; // Ŀ�������Tag
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;

        if (player == null)
        {
            Debug.LogWarning("δ�ҵ����Ŀ�꣬��ȷ���������� Tag Ϊ 'Player' �����壡");
            enabled = false;
        }
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position; // ֱ��ͬ��λ��
        }
    }
}
