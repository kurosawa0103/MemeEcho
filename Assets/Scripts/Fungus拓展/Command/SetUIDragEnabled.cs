using UnityEngine;
using Fungus;

[CommandInfo("UI",
             "Set UI Drag Enabled",
             "启用或禁用一个 DraggableUIWindow 的拖拽功能")]
public class SetUIDragEnabled : Command
{
    [Tooltip("目标 UI 窗口 GameObject")]
    public GameObject targetWindow;

    [Tooltip("是否启用拖拽")]
    public BooleanData allowDrag;
    public string targetTag = "Root";

    public override void OnEnter()
    {
        if (targetWindow == null)
        {
            // 尝试自动获取 DraggableUIWindow（例如场景中的第一个）
            var found = GameObject.FindGameObjectWithTag(targetTag).transform.GetComponent<DraggableUIWindow>();
            if (found != null)
            {
                targetWindow = found.gameObject;
            }
        }

        if (targetWindow != null)
        {
            var draggable = targetWindow.GetComponent<DraggableUIWindow>();
            if (draggable != null)
            {
                draggable.canDrag = allowDrag != null && allowDrag.Value;
            }
            else
            {
                Debug.LogWarning("目标物体上没有 DraggableUIWindow 组件");
            }
        }
        else
        {
            Debug.LogWarning("未设置目标窗口，且未能自动找到 DraggableUIWindow");
        }

        Continue();
    }


    public override string GetSummary()
    {
        string windowName = targetWindow != null ? targetWindow.name : $"<自动获取: Tag = \"{targetTag}\">";
        string dragState = allowDrag == null ? "<未设置>"
                          : allowDrag.Value ? "启用拖拽"
                          : "禁用拖拽";

        return $"{windowName} → {dragState}";
    }


    public override Color GetButtonColor()
    {
        return new Color32(35, 191, 217, 255);
    }
}
