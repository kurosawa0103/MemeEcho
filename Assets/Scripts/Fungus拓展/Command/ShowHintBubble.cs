using UnityEngine;
using Fungus;
using System.Collections.Generic;
using System.Collections;

[CommandInfo("自定义命令", "ShowHintBubble", "控制目标角色是否启用提示气泡")]
public class ShowHintBubble : Command
{
    [Tooltip("目标角色对象，需挂载 CharacterBehavior 脚本")]
    public GameObject target;

    [Tooltip("是否允许显示气泡")]
    public bool enableHint = true;

    public float durationTime = 0.4f;
    public override void OnEnter()
    {
        if (target != null)
        {
            CharacterObject cb = target.GetComponent<CharacterObject>();
            if (cb != null)
            {
                cb.canShow = enableHint;

                if (enableHint)
                {
                    // 需要显示气泡
                    if (cb.speechBubble != null && !cb.speechBubble.activeSelf)
                    {
                        cb.ShowBubble();
                    }
                    Continue(); // 显示气泡动画一般是异步，不阻塞流程，这里直接继续
                }
                else
                {
                    // 需要隐藏气泡
                    if (cb.speechBubble != null && cb.speechBubble.activeSelf)
                    {
                        cb.HideBubble();
                        // 等动画时长后再继续
                        StartCoroutine(WaitAndContinue(durationTime));
                        return;  // 这里返回，等待动画完毕
                    }
                    else
                    {
                        Continue(); // 气泡本身已隐藏，直接继续
                    }
                }
            }
            else
            {
                Debug.LogWarning("目标对象未挂载 CharacterBehavior 脚本：" + target.name);
                Continue();
            }
        }
        else
        {
            Debug.LogWarning("ShowHintBubble 指令的目标对象为空！");
            Continue();
        }
    }


    private IEnumerator WaitAndContinue(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Continue();
    }



    public override string GetSummary()
    {
        return target != null
            ? $"设置 {target.name} 的气泡显示 = {enableHint}"
            : "未设置目标对象";
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255); // 粉紫色
    }
}
