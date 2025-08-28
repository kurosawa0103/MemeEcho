using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public string playerTag = "Player"; // 目标物体的Tag
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;

        if (player == null)
        {
            Debug.LogWarning("未找到玩家目标，请确保场景中有 Tag 为 'Player' 的物体！");
            enabled = false;
        }
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position; // 直接同步位置
        }
    }
}
