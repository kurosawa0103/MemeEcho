using UnityEngine;
using UnityEngine.EventSystems;
using Fungus;

[EventHandlerInfo("UI", "On UI Hover", "�������ͣ��ָ�� UI Ԫ����ʱ�����¼�")]
public class OnUIHover : EventHandler, IPointerEnterHandler
{
    [Tooltip("Ҫ������ UI ��������� UI �ϣ�")]
    public GameObject targetUI;

    [Tooltip("�Ƿ�ֻ����һ��")]
    public bool triggerOnce = false;

    private bool triggered = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (triggered && triggerOnce) return;

        if (eventData.pointerEnter == targetUI)
        {
            triggered = true;
            ExecuteBlock();
        }
    }

    public override string GetSummary()
    {
        return targetUI != null ? targetUI.name : "δ���� UI Ԫ��";
    }

}
