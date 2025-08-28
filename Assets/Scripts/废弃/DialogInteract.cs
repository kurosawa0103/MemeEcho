using UnityEngine;
using UnityEngine.UI; // ���� UI �����ռ�

public class DialogInteract : MonoBehaviour
{
    public Button interactDialogButton; // ���� UI ��ť
    private DialogManager dialogManager;
    private CharacterState characterState;
    void Start()
    {
        dialogManager = FindObjectOfType<DialogManager>();
        characterState = FindObjectOfType<CharacterState>();
        UpdateButtonState(); // ��ʼ����ť״̬
    }

    void Update()
    {
        UpdateButtonState(); // ÿ֡��鲢���°�ť״̬
    }

    void UpdateButtonState()
    {
        if (interactDialogButton != null && dialogManager != null)
        {
            bool isChatWait = characterState.currentAction == CharacterState.CharAction.ChatWaitAction;
            bool isNotChatting = characterState.currentAction != CharacterState.CharAction.ChatAction;

            // ֻ�е���ɫ���� ChatWaitAction �Ҳ��� ChatAction ״̬ʱ����ť�ſɽ���
            interactDialogButton.interactable = (dialogManager.selectDialogData != null) && isChatWait && isNotChatting;
        }
    }


    public void InteractDialog()
    {
        if (dialogManager.selectDialogData != null)
        {
            Debug.Log("���� " + dialogManager.selectDialogData.name + " �ɹ���");
            dialogManager.GenerateReply(dialogManager.selectDialogData);//�ظ�

            dialogManager.dialogPool.Remove(dialogManager.selectDialogData);//�Ƴ�ѡ��dialog
            dialogManager.selectDialogData = null;
            dialogManager.RefreshDialogList();//ˢ��

        }
    }

    // �رճ���ǰ�Ի���֮������жԻ���
    private void CloseOtherDialogs()
    {
        DialogShow[] allDialogs = FindObjectsOfType<DialogShow>(); // �������жԻ������

        foreach (DialogShow dialog in allDialogs)
        {
            if (dialog.gameObject != this.gameObject) // ȷ�����رյ�ǰ����
            {
                dialog.CloseDialog(dialog.fadeOutDuration, dialog.scaleOutDuration);
            }
        }
    }
}
