using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisDialog : MonoBehaviour
{
    public DialogData thisDialogData;
    private DialogManager dialogManager;
    public CharacterState characterState;

    private void Start()
    {
        dialogManager = FindObjectOfType<DialogManager>();

    }
    public void SelectDialog()
    {
        // �����ǰѡ�е� ���Լ�����ȡ��ѡ����Ϊ null��
        if (dialogManager.selectDialogData == thisDialogData)
        {
            dialogManager.selectDialogData = null;
        }
        else
        {
            // ѡ���µ�
            dialogManager.selectDialogData = thisDialogData;
        }

        FindPlayerCharacterState();
    }
    private void FindPlayerCharacterState()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            characterState = player.GetComponent<CharacterState>();
            if (characterState == null)
            {
                Debug.LogWarning("Player ������û���ҵ� CharacterState �����");
            }
            else
            {
                //���������ʱ��Ż�ȴ�
                if(characterState.currentAction !=CharacterState.CharAction.ChatAction)
                {
                    characterState.ChangeAction(CharacterState.CharAction.ChatWaitAction);
                    //characterState.ChangeState(CharacterState.CharState.Idle);
                }


            }
        }
        else
        {
            Debug.LogWarning("δ�ҵ� Tag Ϊ 'Player' �Ķ���");
        }
    }
}
