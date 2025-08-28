using UnityEngine;
using SoftMasking; // 记得引入 SoftMask 所在命名空间
using System.Collections;
public class SoftMaskActivator : MonoBehaviour
{
    public GameObject maskObject;
    public string targetTag = "TargetCollider";

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("触发碰撞：" + other.name);

        if (other.CompareTag(targetTag))
        {
            if (maskObject == null)
            {
                Debug.LogWarning("maskObject 没有设置！");
                return;
            }

            SoftMask softMask = maskObject.GetComponent<SoftMask>();
            if (softMask != null)
            {
                softMask.enabled = true;
                Debug.Log("SoftMask 已启用！");
            }
            else
            {
                Debug.LogWarning("目标物体没有 SoftMask 组件！");
            }
        }
    }
}