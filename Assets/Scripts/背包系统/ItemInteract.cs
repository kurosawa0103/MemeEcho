using UnityEngine;
using UnityEngine.UI; // 引入 UI 命名空间

public class ItemInteract : MonoBehaviour
{
    public Button interactButton; // 关联 UI 按钮
    private InventorySystem inventorySystem;
    private CharacterState characterState;
    public Transform eventParent;
    void Start()
    {
        inventorySystem = FindObjectOfType<InventorySystem>();
        characterState = FindObjectOfType<CharacterState>();
        UpdateButtonState(); // 初始化按钮状态
    }

    void Update()
    {
        UpdateButtonState(); // 每帧检查并更新按钮状态
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
            Debug.Log("与 " + inventorySystem.selectItem.name + " 互动！");
            //characterState.currentAction = inventorySystem.selectItem.charAction;
            //GameObject prefabIsland = Instantiate(inventorySystem.selectItem.itemEvent.eventPrefab, eventParent);
        }
    }
}
