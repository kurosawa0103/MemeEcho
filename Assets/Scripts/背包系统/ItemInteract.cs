using UnityEngine;
using UnityEngine.UI; // ���� UI �����ռ�

public class ItemInteract : MonoBehaviour
{
    public Button interactButton; // ���� UI ��ť
    private InventorySystem inventorySystem;
    private CharacterState characterState;
    public Transform eventParent;
    void Start()
    {
        inventorySystem = FindObjectOfType<InventorySystem>();
        characterState = FindObjectOfType<CharacterState>();
        UpdateButtonState(); // ��ʼ����ť״̬
    }

    void Update()
    {
        UpdateButtonState(); // ÿ֡��鲢���°�ť״̬
    }

    void UpdateButtonState()
    {
        if (interactButton != null && inventorySystem != null)
        {
            interactButton.interactable = (inventorySystem.selectItem != null);
        }
    }

    public void InteractItem()
    {
        if (inventorySystem.selectItem != null)
        {
            Debug.Log("�� " + inventorySystem.selectItem.name + " ������");
            //characterState.currentAction = inventorySystem.selectItem.charAction;
            //GameObject prefabIsland = Instantiate(inventorySystem.selectItem.itemEvent.eventPrefab, eventParent);
        }
    }
}
