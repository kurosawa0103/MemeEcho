using UnityEngine;
using Fungus;

[CommandInfo("�Զ�������", "FlipCard", "���ÿ����ϵķ�ת����")]
public class FlipCardCommand : Command
{
    [Tooltip("Ҫ��ת�Ŀ��ƶ�������� CardFlip �ű�")]
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
                Debug.LogWarning("[FlipCardCommand] δ�ҵ� CardFlip �ű���");
            }
        }
        else
        {
            Debug.LogWarning("[FlipCardCommand] δ���ÿ��ƶ���");
        }

        Continue();
    }

    public override string GetSummary()
    {
        return targetCard != null ? targetCard.name : "δ���ÿ���";
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255); // ����ɫ
    }
}
