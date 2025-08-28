using UnityEngine;

public class TriggerToUIExpander : MonoBehaviour
{
    [Tooltip("拖入目标 UI 对象上的 Expand 脚本")]
    public ExpandUIOnTrigger uiExpander;

    [Tooltip("触发器对象")]
    public GameObject triggerObject;

    [Tooltip("需要启用的脚本")]
    public MonoBehaviour scriptToEnable;

    [Tooltip("需要禁用的脚本")]
    public MonoBehaviour scriptToDisable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == triggerObject)
        {
            Debug.Log("[TriggerToUIExpander] 触发器触发，执行操作");

            // 启用脚本
            if (scriptToEnable != null)
            {
                scriptToEnable.enabled = true;
                Debug.Log("[TriggerToUIExpander] 已启用脚本：" + scriptToEnable.GetType().Name);
            }

            // 禁用脚本
            if (scriptToDisable != null)
            {
                scriptToDisable.enabled = false;
                Debug.Log("[TriggerToUIExpander] 已禁用脚本：" + scriptToDisable.GetType().Name);
            }

            // 通知 UI 执行扩张
            if (uiExpander != null)
            {
                uiExpander.StartExpand();
                Debug.Log("[TriggerToUIExpander] 调用了 UI 扩展方法");
            }
        }
    }
}