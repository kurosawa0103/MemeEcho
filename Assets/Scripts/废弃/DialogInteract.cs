using UnityEngine;
using UnityEngine.UI; // 引入 UI 命名空间

public class DialogInteract : MonoBehaviour
{
    public Button interactDialogButton; // 关联 UI 按钮
    private DialogManager dialogManager;
    private CharacterState characterState;
    void Start()
    {
        dialogManager = FindObjectOfType<DialogManager>();
        characterState = FindObjectOfType<CharacterState>();
        UpdateButtonState(); // 初始化按钮状态
    }

    void Update()
    {
        UpdateButtonState(); // 每帧检查并更新按钮状态
    }

    void UpdateButtonState()
    {
        if (interactDialogButton != null && dialogManager != null)
        {
            bool isChatWait = characterState.currentAction == CharacterState.CharAction.ChatWaitAction;
            bool isNotChatting = characterState.currentAction != CharacterState.CharAction.ChatAction;

            // 只有当角色处于 ChatWaitAction 且不在 ChatAction 状态时，按钮才可交互
            interactDialogButton.interactable = (dialogManager.selectDialogData != null) && isChatWait && isNotChatting;
        }
    }


    public void InteractDialog()
    {
        if (dialogManager.selectDialogData != null)
        {
            Debug.Log("发送 " + dialogManager.selectDialogData.name + " 成功！");
            dialogManager.GenerateReply(dialogManager.selectDialogData);//回复

            dialogManager.dialogPool.Remove(dialogManager.selectDialogData);//移除选中dialog
            dialogManager.selectDialogData = null;
            dialogManager.RefreshDialogList();//刷新

        }
    }

    // 关闭除当前对话框之外的所有对话框
    private void CloseOtherDialogs()
    {
        DialogShow[] allDialogs = FindObjectsOfType<DialogShow>(); // 查找所有对话框组件

        foreach (DialogShow dialog in allDialogs)
        {
            if (dialog.gameObject != this.gameObject) // 确保不关闭当前对象
            {
                dialog.CloseDialog(dialog.fadeOutDuration, dialog.scaleOutDuration);
            }
        }
    }
}
