using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using BehaviorDesigner.Runtime; // ���� Behavior Designer ����ʱ

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;
    public TimeSystem timeSystem;
    public CharacterManager characterManager;
    private CharacterState characterState;
    public Transform dialogParent;
    public Transform replyParent;
    public GameObject dialogPrefab;
    public GameObject replyPrefab; // �ظ�Ԥ����
    public DialogData selectDialogData;

    [Header("�Ի���")]
    public List<DialogData> dialogPool = new List<DialogData>(); // �洢�Ի���
    [Header("��ǰ�Ѽ���Ի���")]
    public List<DialogData> activeDialogEvents = new List<DialogData>(); // ��ǰ�Ѽ���ĶԻ�

    [Header("ʵ�����ĶԻ�Ԥ����")]
    public List<GameObject> dialogPrefabs = new List<GameObject>(); // �洢ʵ�����ĶԻ�Ԥ����

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
        //ˢ����ʾ
        CreatDialog();
    }


    // �ж϶Ի��Ƿ��Ѿ����
    private bool IsDialogCompleted(DialogData dialogData)
    {
        List<DialogState> dialogStates = dialogHandler.LoadDialogState();
        DialogState dialogState = dialogStates.Find(d => d.dialogID == dialogData.name);

        // ����Ҳ����öԻ���״̬������״̬Ϊ "0"�����ʾ�Ի�δ���
        if (dialogState == null || string.IsNullOrEmpty(dialogState.dialogState) || dialogState.dialogState == "0")
        {
            return false; // �Ի�δ���
        }

        // ���״̬Ϊ "1"�����ʾ�Ի������
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
        // ͨ�� eventHandler ��ȡ�¼�״̬
        string eventStatus = eventStateHandler .GetEventStatus(conditionDetail.eventStateName);

        // ����¼�״̬�Ƿ����Ԥ��
        return eventStatus == conditionDetail.eventStateValue;
    }

    public bool CheckDialogComplete(DialogData.UnlockConditionDetail conditionDetail)
    {
        List<DialogState> dialogStates = dialogHandler.LoadDialogState();
        DialogState dialogState = dialogStates.Find(d => d.dialogID == conditionDetail.completedDialog.name);

        // ����ҵ��˶Ի���״̬
        if (dialogState != null)
        {
            // ���� conditionDetail.isCompleted �ж��Ƿ���Ҫ�Ի������
            if (conditionDetail.isDialogCompleted)
            {
                // �����Ҫ�Ի�����ɣ����״̬�Ƿ�Ϊ "1"
                return dialogState.dialogState == "1";
            }
            else
            {
                // �����Ҫ�Ի�δ��ɣ����״̬�Ƿ�Ϊ "0" ��Ϊ��
                return string.IsNullOrEmpty(dialogState.dialogState) || dialogState.dialogState == "0";
            }
        }

        // ���û���ҵ��Ի���״̬���ж������Ƿ�Ϊδ��ɣ�Ĭ��Ϊδ��ɣ�
        return !conditionDetail.isDialogCompleted;
    }
    public bool CheckEventComplete(DialogData.UnlockConditionDetail conditionDetail)
    {
        List<EventState> eventStates = eventHandler.LoadEventState();
        EventState eventState = eventStates.Find(d => d.eventID == conditionDetail.completedEvent.name);

        // ����ҵ��˶Ի���״̬
        if (eventState != null)
        {
            // ���� conditionDetail.isCompleted �ж��Ƿ���Ҫ�Ի������
            if (conditionDetail.isEventCompleted)
            {
                // �����Ҫ�Ի�����ɣ����״̬�Ƿ�Ϊ "1"
                return eventState.eventState == "1";
            }
            else
            {
                // �����Ҫ�Ի�δ��ɣ����״̬�Ƿ�Ϊ "0" ��Ϊ��
                return string.IsNullOrEmpty(eventState.eventState) || eventState.eventState == "0";
            }
        }

        // ���û���ҵ��Ի���״̬���ж������Ƿ�Ϊδ��ɣ�Ĭ��Ϊδ��ɣ�
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
        // �����֮ǰ�ĶԻ���
        foreach (var prefab in dialogPrefabs)
        {
            Destroy(prefab);
        }
        dialogPrefabs.Clear();

        // ������ǰ����ĶԻ��б����ʵ����
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
                    textMeshPro.text = dialogData.dialogText; //�ı���ֵ
                }
                dialogOb.transform.GetComponent<ThisDialog>().thisDialogData = dialogData;//dialogdata��ֵ
            }
        }
    }
    public void GenerateReply(DialogData dialogData)
    {
        characterState.ChangeAction(CharacterState.CharAction.ChatAction);

        // �� dialogData �����ѡ��һ���ظ�
        if (dialogData.dialogReplies != null && dialogData.dialogReplies.Count > 0)
        {
            int randomIndex = Random.Range(0, dialogData.dialogReplies.Count);
            DialogData.DialogReply randomReply = dialogData.dialogReplies[randomIndex];

            // ���ɻظ��Ի���
            GameObject replyDialogOb = Instantiate(replyPrefab, replyParent);
            if (replyDialogOb != null)
            {
                replyDialogOb.GetComponent<UIFollowWorldObject>().offset = randomReply.replydata.dialogOffset;

                // ��ȡ DialogShow ��������ö���
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

            // ����ظ��ȴ�ʱ��
            StartCoroutine(HandleReply(randomReply));
        }
        else
        {
            characterState.ChangeAction(CharacterState.CharAction.WaitAction);
        }
    }
    private IEnumerator HandleReply(DialogData.DialogReply dialogReply)
    {
        
        // ���ظ��ı��Ƿ�Ϊ��
        if (string.IsNullOrEmpty(dialogReply.replydata.dialogText))
        {
            Debug.LogWarning("�ظ��ı�Ϊ�գ������˻ظ�");
            characterState.ChangeAction(CharacterState.CharAction.WaitAction);
            yield break;
        }

        // �ȴ��ظ��ĵȴ�ʱ��
        yield return new WaitForSeconds(dialogReply.waitTime);
        // ִ�жԻ�������Ķ���
        ExecuteDialogActions(dialogReply.replydata);
        // �������һ���Ի�������
        if (dialogReply.nextReply != null)
        {
            // ��ȡ��ǰ�Ի��򲢵��� CloseDialog ���е���
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

            // ��ȡ��ǰ�Ի��򲢵��� CloseDialog ���е���
            DialogShow dialogShow = replyParent.GetComponentInChildren<DialogShow>();
            if (dialogShow != null)
            {
                dialogShow.CloseDialog(dialogShow.fadeOutDuration, dialogShow.scaleOutDuration);
            }

            // �ȴ��Ի��������
            yield return new WaitForSeconds(dialogShow.fadeOutDuration);
        }
    }

}
