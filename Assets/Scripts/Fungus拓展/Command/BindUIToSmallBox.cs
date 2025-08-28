using UnityEngine;
using Fungus;

[CommandInfo("�Զ���ָ��", "Bind UI To SmallBox", "��ָ�� UI �󶨵����� SmallBox ��ǩ�� FollowUI ��")]
public class BindUIToSmallBox : Command
{
    [Tooltip("Ҫ�󶨵� UI Ԫ��")]
    public RectTransform uiToBind;

    public override void OnEnter()
    {
        if (uiToBind == null)
        {
            Debug.LogWarning("BindUIToSmallBox��δ����Ҫ�󶨵� UI");
            Continue();
            return;
        }

        // �������г����м���� FollowUI ���
        FollowUI[] allFollowUIs = GameObject.FindObjectsOfType<FollowUI>(true);

        bool success = false;

        foreach (var followUI in allFollowUIs)
        {
            if (followUI.CompareTag("SmallBox"))
            {
                followUI.uiElement = uiToBind;
                Debug.Log($"BindUIToSmallBox���ɹ��� {uiToBind.name} �� {followUI.gameObject.name}");
                success = true;
                break; // ֻ�󶨵�һ��SmallBox
            }
        }

        if (!success)
        {
            Debug.LogWarning("BindUIToSmallBox��δ�ҵ��κδ� SmallBox ��ǩ�� FollowUI");
        }

        Continue();
    }

    public override string GetSummary()
    {
        return uiToBind != null ? $"�� {uiToBind.name} �� SmallBox" : "δ���� UI";
    }

    public override Color GetButtonColor()
    {
        return new Color32(255, 240, 200, 255);
    }
}
