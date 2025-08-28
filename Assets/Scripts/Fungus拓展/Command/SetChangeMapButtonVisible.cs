using UnityEngine;
using Fungus;

[CommandInfo("Custom", "Set ChangeMapButton Visible", "设置 ChangeMapButton 脚本的 canShow 属性")]
public class SetChangeMapButtonVisible : Command
{
    [Tooltip("要设置的目标物体")]
    public GameObject targetObject;

    [Tooltip("是否允许显示 targetObject")]
    public BooleanData canShow = new BooleanData(true);

    public override void OnEnter()
    {
        if (targetObject != null)
        {
            ChangeMapButton cmb = targetObject.GetComponent<ChangeMapButton>();
            if (cmb != null)
            {
                cmb.canShow = canShow.Value;
                cmb.RefreshShowStatus();
            }
            else
            {
                Debug.LogWarning("目标物体上没有 ChangeMapButton 组件！");
            }
        }

        Continue();
    }

    public override string GetSummary()
    {
        if (targetObject == null)
            return "未设置目标物体";
        return targetObject.name + ".canShow = " + canShow.Value;
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255); // 淡粉色
    }
}
