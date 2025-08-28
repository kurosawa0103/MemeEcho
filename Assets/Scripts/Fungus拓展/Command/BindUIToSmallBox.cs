using UnityEngine;
using Fungus;

[CommandInfo("自定义指令", "Bind UI To SmallBox", "将指定 UI 绑定到带有 SmallBox 标签的 FollowUI 上")]
public class BindUIToSmallBox : Command
{
    [Tooltip("要绑定的 UI 元素")]
    public RectTransform uiToBind;

    public override void OnEnter()
    {
        if (uiToBind == null)
        {
            Debug.LogWarning("BindUIToSmallBox：未设置要绑定的 UI");
            Continue();
            return;
        }

        // 查找所有场景中激活的 FollowUI 组件
        FollowUI[] allFollowUIs = GameObject.FindObjectsOfType<FollowUI>(true);

        bool success = false;

        foreach (var followUI in allFollowUIs)
        {
            if (followUI.CompareTag("SmallBox"))
            {
                followUI.uiElement = uiToBind;
                Debug.Log($"BindUIToSmallBox：成功绑定 {uiToBind.name} 到 {followUI.gameObject.name}");
                success = true;
                break; // 只绑定第一个SmallBox
            }
        }

        if (!success)
        {
            Debug.LogWarning("BindUIToSmallBox：未找到任何带 SmallBox 标签的 FollowUI");
        }

        Continue();
    }

    public override string GetSummary()
    {
        return uiToBind != null ? $"绑定 {uiToBind.name} 到 SmallBox" : "未设置 UI";
    }

    public override Color GetButtonColor()
    {
        return new Color32(255, 240, 200, 255);
    }
}
