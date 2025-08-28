using UnityEngine;
using Fungus;

[CommandInfo("自定义指令", "InstanceSubRoot",
    "勾选时：在指定Tag的父对象下实例化Prefab，并绑定SmallBox的FollowUI；未勾选时：销毁该父对象下所有带指定Tag的子物体")]
public class InstanceSubRoot : Command
{
    [Tooltip("是否执行实例化，否则执行销毁")]
    public bool instantiate = true;

    [Tooltip("要实例化的 Prefab")]
    public GameObject prefab;

    [Tooltip("销毁目标 Tag")]
    public string targetTag = "SubRoot";

    [Tooltip("父物体的 Tag（在哪个父物体下操作）")]
    public string parentTag = "Canvas";

    public override void OnEnter()
    {
        if (instantiate)
        {
            GameObject parentObj = GameObject.FindWithTag(parentTag);
            if (parentObj != null && prefab != null)
            {
                //  实例化 Prefab
                GameObject newInstance = Object.Instantiate(prefab, parentObj.transform);

                // 获取第一个子物体作为 uiElement
                RectTransform firstChild = null;
                if (newInstance.transform.childCount > 0)
                {
                    firstChild = newInstance.transform.GetChild(0).GetComponent<RectTransform>();
                }

                if (firstChild == null)
                {
                    Debug.LogWarning("InstanceSubRoot：实例化对象没有有效子物体或未挂 RectTransform");
                }
                else
                {
                    //  查找所有带 SmallBox 标签的对象（包括禁用）
                    FollowUI[] allFollowUIs = Resources.FindObjectsOfTypeAll<FollowUI>();
                    foreach (var followUI in allFollowUIs)
                    {
                        if (followUI.CompareTag("SmallBox"))
                        {
                            followUI.uiElement = firstChild;
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("InstanceSubRoot：未找到指定的父物体或 Prefab 未设置");
            }
        }
        else
        {
            GameObject[] allWithTag = GameObject.FindGameObjectsWithTag(targetTag);
            foreach (GameObject go in allWithTag)
            {
                if (go.transform.parent != null && go.transform.parent.CompareTag(parentTag))
                {
                    Object.Destroy(go);
                }
            }
        }

        Continue();
    }
}
