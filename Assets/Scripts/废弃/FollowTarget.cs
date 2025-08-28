using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target; // 要跟随的目标
    public Vector3 offset;   // 偏移量

    void Update()
    {
        if (target != null)
        {
            // 将UI图片的位置设置为目标物体的位置加上偏移量
            transform.position = target.position + offset;
        }
    }
}
