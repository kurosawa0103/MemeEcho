using UnityEngine;
using Fungus;

[CommandInfo("自定义",
             "晃动检测",
             "等待 UI 被指定方向晃动指定次数后继续")]
public class ShakeDetectCommand : Command
{
    private DraggableUIWindow draggableWindow;

    [Tooltip("检测垂直还是水平晃动")]
    public bool vertical = true;

    [Tooltip("需要的晃动次数")]
    public int requiredShakeCount = 3;

    [Tooltip("判定为晃动的最小距离（像素）")]
    public float shakeDistance = 30f;

    [Tooltip("是否在成功后清除晃动记录")]
    public bool resetAfterSuccess = true;


    public override void OnEnter()
    {
        draggableWindow = GameObject.FindGameObjectWithTag("Root") .transform .GetComponent< DraggableUIWindow>();
        if (draggableWindow == null)
        {
            Debug.LogWarning("未设置 draggableWindow");
            Continue();
            return;
        }

        // 启用晃动检测
        draggableWindow.enableShakeDetection = true;
        draggableWindow.shakeRequiredCount = requiredShakeCount;
        draggableWindow.shakeThreshold = shakeDistance;

        // 不再重置晃动次数

        FungusManager.Instance.StartCoroutine(WaitForShake());
    }

    private System.Collections.IEnumerator WaitForShake()
    {
        while (true)
        {
            if (draggableWindow.GetCurrentShakeCount(vertical) >= requiredShakeCount)
            {
                Debug.Log("晃动检测成功！");
                if (resetAfterSuccess)
                    draggableWindow.ResetShakeCount();

                break;
            }
            yield return null;
        }

        Continue();
    }

    public override string GetSummary()
    {

        return $"{(vertical ? "垂直" : "水平")}晃动 {requiredShakeCount} 次（阈值 {shakeDistance}）";
    }

    public override Color GetButtonColor()
    {
        return new Color32(200, 100, 150, 255);
    }
}
