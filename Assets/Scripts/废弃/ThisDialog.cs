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
        // 如果当前选中的 是自己，则取消选择（设为 null）
        if (dialogManager.selectDialogData == thisDialogData)
        {
            dialogManager.selectDialogData = null;
        }
        else
        {
            // 选择新的
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
                Debug.LogWarning("Player 物体上没有找到 CharacterState 组件！");
            }
            else
            {
                //不在聊天的时候才会等待
                if(characterState.currentAction !=CharacterState.CharAction.ChatAction)
                {
                    characterState.ChangeAction(CharacterState.CharAction.ChatWaitAction);
                    //characterState.ChangeState(CharacterState.CharState.Idle);
                }


            }
        }
        else
        {
            Debug.LogWarning("未找到 Tag 为 'Player' 的对象！");
        }
    }
}
