using UnityEngine;
using Fungus;

[CommandInfo("Camera", "Set Camera Zoom Z Range", "设置相机缩放 Z 轴范围并更新目标位置")]
public class SetCameraZoomZRange : Command
{
    [Tooltip("新的最小 Z 值")]
    public float newMinZ =-16;

    [Tooltip("新的最大 Z 值")]
    public float newMaxZ=-20f;

    [Tooltip("设置目标 Z 位置（可选）")]
    public float setZ=-20;

    public override void OnEnter()
    {
        CameraZoomWithObjectScale zoomScript = FindObjectOfType<CameraZoomWithObjectScale>();
        if (zoomScript == null)
        {
            Debug.LogWarning("未找到 CameraZoomWithObjectScale 脚本，请确保场景中存在该组件");
            Continue();
            return;
        }

        // 设置 minZ 和 maxZ
        zoomScript.minZ = newMinZ;
        zoomScript.maxZ = newMaxZ;

        // 设置 targetZ，并立即应用
        zoomScript.targetZ = setZ;

        Continue();
    }

    public override string GetSummary()
    {

        return $"设置Z范围：{newMinZ} 至 {newMaxZ}, 当前Z: {setZ}";
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 180, 75, 255); // 自定义颜色
    }
}
