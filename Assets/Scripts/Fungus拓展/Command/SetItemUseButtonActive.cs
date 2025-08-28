using UnityEngine;
using UnityEngine.UI;
using Fungus;

[CommandInfo("Custom",
             "Set Item UseButton State (By Item)",
             "ͨ�� Item ʵ������ DraggableItem �� useButton ��ʾ�뽻��")]
public class SetItemUseButtonStateByItem : Command
{
    [Tooltip("Ŀ�� Item ����")]
    public Item targetItem;


    [Tooltip("�Ƿ�������")]
    public BooleanData interactable = new BooleanData(true);

    public override void OnEnter()
    {
        // �ҵ����������� DraggableItem
        DraggableItem[] allItems = GameObject.FindObjectsOfType<DraggableItem>();
        foreach (var di in allItems)
        {
            if (di.thisItem == targetItem)
            {
                if (di.useButton != null)
                {
                    Button btn = di.useButton.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.interactable = interactable.Value;
                    }
                }
                else
                {
                    Debug.LogWarning("useButton δ������Ŀ�� DraggableItem �ϣ�");
                }

                break; // ���ҵ�Ŀ�꣬�˳�ѭ��
            }
        }

        Continue();
    }

    public override string GetSummary()
    {
        return $"Set UseButton for '{targetItem?.name}' Interactable={interactable.Value}";
    }
}
