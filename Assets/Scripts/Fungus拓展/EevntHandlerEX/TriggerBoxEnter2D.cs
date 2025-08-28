using UnityEngine;
using Fungus;

[EventHandlerInfo("拓展", "小框框进入触发器 (2D)", "当 Tag 为 SmallBox 的 2D 物体进入此触发区域时触发")]
public class TriggerBoxEnter2D : EventHandler
{
    [Tooltip("目标物体的 Tag")]
    [SerializeField] private string targetTag = "SmallBox";


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled || !gameObject.activeInHierarchy)
            return;

        if (other.CompareTag(targetTag))
        {
            Debug.Log($"TriggerBoxEnter2D: Tag 为 {targetTag} 的物体进入触发区域，触发事件！");
            ExecuteBlock();
        }
    }
}
