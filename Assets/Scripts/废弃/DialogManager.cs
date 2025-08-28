using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using BehaviorDesigner.Runtime; // 引入 Behavior Designer 运行时

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;
    public TimeSystem timeSystem;
    public CharacterManager characterManager;
    private CharacterState characterState;
    public Transform dialogParent;
    public Transform replyParent;
    public GameObject dialogPrefab;
    public GameObject replyPrefab; // 回复预制体
    public DialogData selectDialogData;

    [Header("对话库")]
    public List<DialogData> dialogPool = new List<DialogData>(); // 存储对话库
    [Header("当前已激活对话库")]
    public List<DialogData> activeDialogEvents = new List<DialogData>(); // 当前已激活的对话

    [Header("实例化的对话预制体")]
    public List<GameObject> dialogPrefabs = new List<GameObject>(); // 存储实例化的对话预制体

    private IDialogHandler dialogHandler = new DialogToJson();
    private IEventHandler eventHandler = new EventToJson();
    private IEventStateDataHandler eventStateHandler = new JsonGenerator();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        characterState = FindObjectOfType<CharacterState>();
        RefreshDialogList();
        
    }
    public void RefreshDialogList()
    {
        activeDialogEvents.Clear();
        activeDialogEvents = dialogPool.Where(dialogData =>
            !IsDialogCompleted(dialogData) && IsDialogUnlocked(dialogData)).ToList();
        //刷新显示
        CreatDialog();
    }


    // 判断对话是否已经完成
    private bool IsDialogCompleted(DialogData dialogData)
    {
        List<DialogState> dialogStates = dialogHandler.LoadDialogState();
        DialogState dialogState = dialogStates.Find(d => d.dialogID == dialogData.name);

        // 如果找不到该对话的状态，或者状态为 "0"，则表示对话未完成
        if (dialogState == null || string.IsNullOrEmpty(dialogState.dialogState) || dialogState.dialogState == "0")
        {
            return false; // 对话未完成
        }

        // 如果状态为 "1"，则表示对话已完成
        return dialogState.dialogState == "1";
    }

    private bool IsDialogUnlocked(DialogData dialogData)
    {
        DayState currentDayState = timeSystem.dayState;

        if (!IsTimeInRange(dialogData.availableTimes, currentDayState))
        {
            return false;
        }

        if (dialogData.unlockConditionGroups.Count == 0)
        {
            return true;
        }

        foreach (var group in dialogData.unlockConditionGroups)
        {
            if (EvaluateConditionGroup(group))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTimeInRange(List<DayState> availableTimes, DayState currentDayState)
    {
        return availableTimes.Contains(currentDayState);
    }

    private bool EvaluateConditionGroup(DialogData.UnlockConditionGroup group)
    {
        if (group.conditions == null || group.conditions.Count == 0)
        {
            return group.conditionLogic == DialogData.ConditionLogic.AND;
        }

        bool result = (group.conditionLogic == DialogData.ConditionLogic.AND);

        foreach (var conditionDetail in group.conditions)
        {
            bool conditionMet = CheckCondition(conditionDetail);

            if (group.conditionLogic == DialogData.ConditionLogic.AND)
            {
                result &= conditionMet;
            }
            else if (group.conditionLogic == DialogData.ConditionLogic.OR)
            {
                result |= conditionMet;
            }

            if (group.conditionLogic == DialogData.ConditionLogic.AND && !result)
            {
                return false;
            }
        }

        return result;
    }
    private bool CheckCondition(DialogData.UnlockConditionDetail conditionDetail)
    {
        switch (conditionDetail.unlockCondition)
        {
            case DialogData.UnlockCondition.eventState:
                return CheckEventState(conditionDetail);
            case DialogData.UnlockCondition.dialogComplete:
                return CheckDialogComplete(conditionDetail);
            case DialogData.UnlockCondition.eventComplete:
                return CheckEventComplete(conditionDetail);
            case DialogData.UnlockCondition.characterAttribute:
                return CheckCharacterAttribute(conditionDetail);
        }
        return false;
    }
    public bool CheckEventState(DialogData.UnlockConditionDetail conditionDetail)
    {
        // 通过 eventHandler 获取事件状态
        string eventStatus = eventStateHandler .GetEventStatus(conditionDetail.eventStateName);

        // 检查事件状态是否符合预期
        return eventStatus == conditionDetail.eventStateValue;
    }

    public bool CheckDialogComplete(DialogData.UnlockConditionDetail conditionDetail)
    {
        List<DialogState> dialogStates = dialogHandler.LoadDialogState();
        DialogState dialogState = dialogStates.Find(d => d.dialogID == conditionDetail.completedDialog.name);

        // 如果找到了对话的状态
        if (dialogState != null)
        {
            // 根据 conditionDetail.isCompleted 判断是否需要对话已完成
            if (conditionDetail.isDialogCompleted)
            {
                // 如果需要对话已完成，检查状态是否为 "1"
                return dialogState.dialogState == "1";
            }
            else
            {
                // 如果需要对话未完成，检查状态是否为 "0" 或为空
                return string.IsNullOrEmpty(dialogState.dialogState) || dialogState.dialogState == "0";
            }
        }

        // 如果没有找到对话的状态，判断条件是否为未完成（默认为未完成）
        return !conditionDetail.isDialogCompleted;
    }
    public bool CheckEventComplete(DialogData.UnlockConditionDetail conditionDetail)
    {
        List<EventState> eventStates = eventHandler.LoadEventState();
        EventState eventState = eventStates.Find(d => d.eventID == conditionDetail.completedEvent.name);

        // 如果找到了对话的状态
        if (eventState != null)
        {
            // 根据 conditionDetail.isCompleted 判断是否需要对话已完成
            if (conditionDetail.isEventCompleted)
            {
                // 如果需要对话已完成，检查状态是否为 "1"
                return eventState.eventState == "1";
            }
            else
            {
                // 如果需要对话未完成，检查状态是否为 "0" 或为空
                return string.IsNullOrEmpty(eventState.eventState) || eventState.eventState == "0";
            }
        }

        // 如果没有找到对话的状态，判断条件是否为未完成（默认为未完成）
        return !conditionDetail.isEventCompleted;
    }
    public bool CheckCharacterAttribute(DialogData.UnlockConditionDetail conditionDetail)
    {
        CharacterAttributes attributes = characterManager.GetCharacterAttributes();
        float attributeValue = 0;

        switch (conditionDetail.characterAttribute)
        {
            case CharacterManager.CharacterAttribute.Cute:
                attributeValue = attributes.cuteHeart;
                break;
            case CharacterManager.CharacterAttribute.Emotional:
                attributeValue = attributes.emotionalHeart;
                break;
            case CharacterManager.CharacterAttribute.Rational:
                attributeValue = attributes.rationalHeart;
                break;
            case CharacterManager.CharacterAttribute.Vitality:
                attributeValue = attributes.vitalityHeart;
                break;
        }

        return attributeValue >= conditionDetail.attributeRange.x && attributeValue <= conditionDetail.attributeRange.y;
    }

    private void ExecuteDialogActions(DialogData dialogData)
    {
        foreach (var action in dialogData.dialogActions)
        {
            switch (action.dialogActionType)
            {
                case DialogData.DialogActionType.ModifyCharacterStats:
                    characterManager.ModifyCharacterAttribute(action.characterAttribute, action.attributeValue);
                    break;
                case DialogData.DialogActionType.UpdateEventState:
                    eventStateHandler.ModifyEventStatus(action.eventName, action.eventKey);

                    break;
                case DialogData.DialogActionType.ChangeCharacterState:
                    characterState.ChangeAction(action.newAction);
                    break;
            }
        }
    }

    public void CreatDialog()
    {
        // 先清除之前的对话框
        foreach (var prefab in dialogPrefabs)
        {
            Destroy(prefab);
        }
        dialogPrefabs.Clear();

        // 遍历当前激活的对话列表，逐个实例化
        foreach (var dialogData in activeDialogEvents)
        {
            GameObject dialogOb = Instantiate(dialogPrefab, dialogParent);
            if (dialogOb != null)
            {
                dialogPrefabs.Add(dialogOb);

                Transform textTransform = dialogOb.transform
                                .GetChild(0)
                                .GetChild(0)
                                .GetChild(0);

                TextMeshProUGUI textMeshPro = textTransform.GetComponent<TextMeshProUGUI>();
                if (textMeshPro != null)
                {
                    textMeshPro.text = dialogData.dialogText; //文本赋值
                }
                dialogOb.transform.GetComponent<ThisDialog>().thisDialogData = dialogData;//dialogdata赋值
            }
        }
    }
    public void GenerateReply(DialogData dialogData)
    {
        characterState.ChangeAction(CharacterState.CharAction.ChatAction);

        // 从 dialogData 中随机选择一个回复
        if (dialogData.dialogReplies != null && dialogData.dialogReplies.Count > 0)
        {
            int randomIndex = Random.Range(0, dialogData.dialogReplies.Count);
            DialogData.DialogReply randomReply = dialogData.dialogReplies[randomIndex];

            // 生成回复对话框
            GameObject replyDialogOb = Instantiate(replyPrefab, replyParent);
            if (replyDialogOb != null)
            {
                replyDialogOb.GetComponent<UIFollowWorldObject>().offset = randomReply.replydata.dialogOffset;

                // 获取 DialogShow 组件并设置动画
                DialogShow dialogShow = replyDialogOb.GetComponent<DialogShow>();
                if (dialogShow != null)
                {
                    dialogShow.SetupDialog(randomReply.replydata.dialogText,
                                           dialogShow.fadeInDuration,
                                           dialogShow.fadeOutDuration,
                                           dialogShow.scaleInDuration,
                                           dialogShow.scaleOutDuration,24);

                }
            }

            // 处理回复等待时间
            StartCoroutine(HandleReply(randomReply));
        }
        else
        {
            characterState.ChangeAction(CharacterState.CharAction.WaitAction);
        }
    }
    private IEnumerator HandleReply(DialogData.DialogReply dialogReply)
    {
        
        // 检查回复文本是否为空
        if (string.IsNullOrEmpty(dialogReply.replydata.dialogText))
        {
            Debug.LogWarning("回复文本为空，跳过此回复");
            characterState.ChangeAction(CharacterState.CharAction.WaitAction);
            yield break;
        }

        // 等待回复的等待时间
        yield return new WaitForSeconds(dialogReply.waitTime);
        // 执行对话结束后的动作
        ExecuteDialogActions(dialogReply.replydata);
        // 如果有下一条对话，继续
        if (dialogReply.nextReply != null)
        {
            // 获取当前对话框并调用 CloseDialog 进行淡出
            DialogShow dialogShow = replyParent.GetComponentInChildren<DialogShow>();
            if (dialogShow != null)
            {
                dialogShow.CloseDialog(dialogShow.fadeOutDuration, dialogShow.scaleOutDuration);
            }

            yield return new WaitForSeconds(dialogShow.fadeOutDuration);
            GenerateReply(dialogReply.nextReply);
        }
        else
        {

            // 获取当前对话框并调用 CloseDialog 进行淡出
            DialogShow dialogShow = replyParent.GetComponentInChildren<DialogShow>();
            if (dialogShow != null)
            {
                dialogShow.CloseDialog(dialogShow.fadeOutDuration, dialogShow.scaleOutDuration);
            }

            // 等待对话框的销毁
            yield return new WaitForSeconds(dialogShow.fadeOutDuration);
        }
    }

}
