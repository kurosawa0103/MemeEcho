using UnityEngine;
using Fungus;

[CommandInfo("Camera", "SetCameraDragController", "通过 Command 修改 CameraDragController 参数")]
public class SetCameraDragController : Command
{

    [Tooltip("拖拽速度")]
    public float dragSpeed = 0.002f;

    [Tooltip("X 轴最小限制")]
    public float xMin = -2f;

    [Tooltip("X 轴最大限制")]
    public float xMax = 2f;

    [Tooltip("Y 轴最小限制")]
    public float yMin = 0.5f;

    [Tooltip("Y 轴最大限制")]
    public float yMax = 1f;

    public override void OnEnter()
    {
        CameraDragController cameraDragController = FindObjectOfType<CameraDragController>();
        if (cameraDragController == null)
        {
            Debug.LogWarning("未找到 cameraDragController 脚本，请确保场景中存在该组件");
            Continue();
            return;
        }

        // 修改参数
        cameraDragController.dragSpeed = dragSpeed;
        cameraDragController.xLimit = new Vector2(xMin, xMax);
        cameraDragController.yLimit = new Vector2(yMin, yMax);

        Continue();
    }
    public override string GetSummary()
    {
        return $"设置拖拽速度:{dragSpeed}, X范围:[{xMin},{xMax}], Y范围:[{yMin},{yMax}]";
    }

}
