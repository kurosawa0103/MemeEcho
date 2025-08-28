using UnityEngine;
using Fungus;

[CommandInfo("UI",
             "Set UI Drag Enabled",
             "���û����һ�� DraggableUIWindow ����ק����")]
public class SetUIDragEnabled : Command
{
    [Tooltip("Ŀ�� UI ���� GameObject")]
    public GameObject targetWindow;

    [Tooltip("�Ƿ�������ק")]
    public BooleanData allowDrag;
    public string targetTag = "Root";

    public override void OnEnter()
    {
        if (targetWindow == null)
        {
            // �����Զ���ȡ DraggableUIWindow�����糡���еĵ�һ����
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
                Debug.LogWarning("Ŀ��������û�� DraggableUIWindow ���");
            }
        }
        else
        {
            Debug.LogWarning("δ����Ŀ�괰�ڣ���δ���Զ��ҵ� DraggableUIWindow");
        }

        Continue();
    }


    public override string GetSummary()
    {
        string windowName = targetWindow != null ? targetWindow.name : $"<�Զ���ȡ: Tag = \"{targetTag}\">";
        string dragState = allowDrag == null ? "<δ����>"
                          : allowDrag.Value ? "������ק"
                          : "������ק";

        return $"{windowName} �� {dragState}";
    }


    public override Color GetButtonColor()
    {
        return new Color32(35, 191, 217, 255);
    }
}
