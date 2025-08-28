using UnityEngine;
using Fungus;

[CommandInfo("自定义命令", "FlipCard", "调用卡牌上的翻转方法")]
public class FlipCardCommand : Command
{
    [Tooltip("要翻转的卡牌对象，需挂载 CardFlip 脚本")]
    public GameObject targetCard;

    public override void OnEnter()
    {
        if (targetCard != null)
        {
            CardFlip card = targetCard.GetComponent<CardFlip>();
            if (card != null)
            {
                card.Flip();
            }
            else
            {
                Debug.LogWarning("[FlipCardCommand] 未找到 CardFlip 脚本！");
            }
        }
        else
        {
            Debug.LogWarning("[FlipCardCommand] 未设置卡牌对象！");
        }

        Continue();
    }

    public override string GetSummary()
    {
        return targetCard != null ? targetCard.name : "未设置卡牌";
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255); // 粉紫色
    }
}
