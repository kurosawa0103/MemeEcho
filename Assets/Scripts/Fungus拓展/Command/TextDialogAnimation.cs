using UnityEngine;
using Fungus;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

[CommandInfo("Custom",
             "Text Dialog Animation",
             "Control a TextMeshPro UI and Dialog Box animation.")]
public class TextDialogAnimation : Command
{
    public enum DialogType
    {
        PlayerDialog,
        CustomDialog
    }

    [Tooltip("The type of the dialog (Player or Custom)")]
    public DialogType dialogType;

    [Tooltip("The text to display in the TextMeshPro UI"), TextArea(3, 6)]
    public string dialogText;

    [Tooltip("����櫶Ի���Ԥ����")]
    public GameObject playerDialogPrefab;

    [Tooltip("�����Ի���Ԥ����")]
    public GameObject customDialogPrefab;

    [Header("��������")]
    [Tooltip("�Զ��������С��0��ʾʹ��Ĭ�ϣ�")]
    public float customFontSize = 24f;

    [Tooltip("�ȴ�ʱ��")]
    public float waitTime = 1f;

    [Tooltip("The duration of the fade-in animation for the text")]
    public float fadeInDuration = 0.5f;

    [Tooltip("The duration of the fade-out animation for the text")]
    public float fadeOutDuration = 0.5f;

    [Tooltip("The duration for dialog box scale-in")]
    public float scaleInDuration = 0.5f;

    [Tooltip("The duration for dialog box scale-out")]
    public float scaleOutDuration = 0.5f;

    [Tooltip("Custom Dialog����Ҫ����λ��")]
    public Transform bubblePosition;

    [Tooltip("�Ի���ƫ����")]
    public Vector3 dialogOffset;

    [Tooltip("��ѡ��Ż�رգ�����ѡ����Զ������Ҫ�ֶ��ر�")]
    public bool shouldCloseDialogBox = true;

    [Tooltip("�ȴ���ɲŻ����")]
    public bool waitForCompletion = true;

    [Header ("������")]
    [Tooltip("�Ƿ���Ҫ������������ܹرնԻ���")]
    public bool requireClickToClose = false;

    [Tooltip("�رնԻ����������� GameObject���簴ť�򴥷�����")]
    public GameObject clickTrigger;

    [Tooltip("���ʱ���ԵĲ㼶������UI�ڵ��㣩")]
    public LayerMask ignoreClickLayers = 1 << 5;

    [Tooltip("����ʾ��ǰ�Ի���ǰ���Ƿ��ȹر����������Ի���")]
    public bool closeAllDialogsFirst = false;

    private GameObject currentDialog;
    private bool hasClickedToClose = false;
    private bool isWaitingForClick = false;

    private static GameObject lastDialog; // ��̬����������׷����һ���Ի���

    [Tooltip("�Ƿ�ǿ�ƹر��������жԻ������Ӷ����ȴ���")]
    public bool forceCloseAllDialogs = false;

    public override void OnEnter()
    {
        if (forceCloseAllDialogs)
        {
            ForceDestroyAllDialogs();
            Continue(); // �����������̣�������ָ��������Ϊ��
            return;
        }
        // ����������һ���Ի���������ڣ�
        if (lastDialog != null)
        {
            Destroy(lastDialog);
            lastDialog = null;
        }

        //��ѡ���ٵ�����£�������������
        if (closeAllDialogsFirst)
        {
            StartCoroutine(CloseAllThenCreate());
        }
        else
        {
            CreateDialog();
        }
    }
    private void OnDisable()
    {
        if (isWaitingForClick && !hasClickedToClose)
        {
            Debug.LogWarning("[TextDialogAnimation] �Ի�����;���٣���ֹ��ǰ Block��");
            StopParentBlock(); // ���ٵ��� Continue������ֱ����ֹ���� Block
        }
    }
    private void CreateDialog()
    {
        GameObject boxBorder = GameObject.FindGameObjectWithTag("BoxBorder");

        if (dialogType == DialogType.PlayerDialog)
        {
            currentDialog = Instantiate(playerDialogPrefab, boxBorder.transform.GetChild(0));
            currentDialog.GetComponent<UIFollowWorldObject>().offset = dialogOffset;
            currentDialog.transform.SetAsFirstSibling();
        }
        else if (dialogType == DialogType.CustomDialog)
        {
            currentDialog = Instantiate(customDialogPrefab, boxBorder.transform.GetChild(0));
            currentDialog.GetComponent<UIFollowWorldObject>().worldObject = bubblePosition;
            currentDialog.GetComponent<UIFollowWorldObject>().offset = dialogOffset;
            currentDialog.transform.SetAsFirstSibling();
        }

        lastDialog = currentDialog;

        DialogShow dialogShow = currentDialog.GetComponent<DialogShow>();
        if (dialogShow != null)
        {
            dialogShow.SetupDialog(dialogText, fadeInDuration, fadeOutDuration, scaleInDuration, scaleOutDuration, customFontSize);
        }

        if (requireClickToClose && clickTrigger != null)
        {
            Collider collider = clickTrigger.GetComponent<Collider>();
            if (collider != null)
            {
                var trigger = clickTrigger.GetComponent<ClickTriggerCollider>();
                if (trigger == null)
                {
                    trigger = clickTrigger.AddComponent<ClickTriggerCollider>();
                }
                trigger.Initialize(this, clickTrigger, ignoreClickLayers);
            }
            else
            {
                Debug.LogWarning("ClickTrigger ������û�� Collider �����");
            }
            StartCoroutine(WaitForClick());//�ȴ����
        }

        if (!requireClickToClose )
        {
            StartCoroutine(WaitForClose());//�ȴ��Զ��ر�
        }

    }
    private System.Collections.IEnumerator WaitForClick()
    {
        yield return new WaitForSeconds(waitTime);
        hasClickedToClose = false; 
        isWaitingForClick = true;
    }
    private System.Collections.IEnumerator WaitForClose()
    {
        yield return new WaitForSeconds(waitTime);
        // �����ڱ�����/���غ����ִ���߼�
        if (!this.enabled || !gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[WaitForClose] Component is disabled or inactive. Aborting close.");
            yield break;
        }
        AutoCloseDialog();
    }
    private System.Collections.IEnumerator CloseAllThenCreate()
    {
        var allDialogs = GameObject.FindGameObjectsWithTag("DialogBox");
        foreach (var dialog in allDialogs)
        {
            if (dialog != null)
            {
                var ds = dialog.GetComponent<DialogShow>();
                if (ds != null)
                {
                    ds.CloseDialog(fadeOutDuration, scaleOutDuration);
                }
                else
                {
                    Destroy(dialog);
                }
            }
        }

        yield return new WaitForSeconds(fadeOutDuration + scaleOutDuration);
        CreateDialog();
    }
    public void OnTriggerClicked()
    {
        if (!requireClickToClose || hasClickedToClose || !isWaitingForClick) return;

        Debug.Log($"[TextDialogAnimation] �յ�������ԣ�{clickTrigger.name}");
        hasClickedToClose = true;
        AutoCloseDialog();
    }

    private void AutoCloseDialog()
    {
        if (!this.enabled || !gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[AutoCloseDialog] Component is disabled or inactive. Skipping.");
        }

        if (currentDialog == null || currentDialog.Equals(null))
        {
            Debug.LogWarning("[AutoCloseDialog] currentDialog �ѱ����١�");
            Continue(); // �������̿�ס
            return;
        }
        DialogShow dialogShow = currentDialog?.GetComponent<DialogShow>();

        if (dialogShow != null && shouldCloseDialogBox)
        {
            dialogShow.CloseDialog(fadeOutDuration, scaleOutDuration);
        }

        void ContinueFlow()
        {
            Continue();
        }

        if (waitForCompletion)
        {
            DOVirtual.DelayedCall(fadeOutDuration + scaleOutDuration, ContinueFlow);
        }
        else
        {
            ContinueFlow();
        }
    }  

    private void ForceDestroyAllDialogs()
    {
        var allDialogs = GameObject.FindGameObjectsWithTag("DialogBox");
        foreach (var dialog in allDialogs)
        {
            if (dialog != null)
            {
                Destroy(dialog);
            }
        }

        // �����һ�μ�¼
        lastDialog = null;
    }

    public override void OnExit()
    {
        // ��ѡ���˳�ʱ�����̬���ã������������ң�
        if (lastDialog == currentDialog)
        {
            lastDialog = null;
        }
    }

    public override string GetSummary()
    {
        if (forceCloseAllDialogs)
        {
            return "<color=red>[ǿ��������жԻ�]</color>";
        }

        string actualSpeaker = dialogType == DialogType.PlayerDialog ? "<color=#FF69B4>����櫣�</color>" : "<color=#8BC7FF>�Զ��壺</color>";
        return $"{actualSpeaker}: {dialogText}";
    }

}


public class ClickTriggerCollider : MonoBehaviour
{
    private TextDialogAnimation dialogAnimation;

    [Tooltip("���ʱҪ���ԵĲ㼶������ UI��")]
    public LayerMask ignoreClickLayers;

    [Tooltip("������� UI ʱ���Ƿ�����ض� UI ��")]
    public bool ignoreUIOnLayer = true;

    [Tooltip("ָ����Ҫ���Ե� UI ��")]
    public LayerMask uiLayerMask = 1 << 5; // Ĭ�� UI ��

    [Tooltip("ָ���Ĵ���������")]
    public GameObject clickTrigger;

    public void Initialize(TextDialogAnimation dialog,GameObject trigger, LayerMask ignoreLayers)
    {
        dialogAnimation = dialog;
        ignoreClickLayers = ignoreLayers;
        clickTrigger = trigger;
    }

    private void Update()
    {
        // ֻ����������¼�
        if (Input.GetMouseButtonDown(0))
        {
            // ���ȼ���Ƿ������� UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                // ��ȡ���е� UI ������
                var raycastResults = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, raycastResults);

                foreach (var result in raycastResults)
                {
                    // �������� UI Ԫ����ָ���� UI ��
                    if (((1 << result.gameObject.layer) & uiLayerMask) != 0)
                    {
                        //Debug.Log($"[ClickTriggerCollider] �������ָ���� UI �㣺{result.gameObject.name}�����Դ���");

                        return; // �����������Ե� UI �㣬ֱ�ӷ���
                    }
                }
            }

            // ���߼�ⳡ������
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {

                Debug.Log($"[ClickTriggerCollider] �����������壺{hit.collider.gameObject.name} (ID: {hit.collider.gameObject.GetInstanceID()})");
                CharacterObject characterBehavior = hit.collider.gameObject.GetComponent<CharacterObject>();
                if (characterBehavior != null)
                {
                    // �Ƚ����������ǲ������õ� clickTrigger
                    //���⻹�����Ƿ��ܵ��
                    if (clickTrigger != null && hit.collider.gameObject == clickTrigger && characterBehavior.canClickTalk)
                    {
                        //Debug.Log($"[ClickTriggerCollider] �������õ� Trigger��{clickTrigger.name}��ִ�д����߼���");

                        if (dialogAnimation != null)
                        {
                            dialogAnimation.OnTriggerClicked();
                        }
                    }
                    else
                    {
                        Debug.Log($"[ClickTriggerCollider] �������������壺{hit.collider.gameObject.name}���������õ� Trigger��");
                    }
                }
                
            }
            else
            {
                //Debug.Log("[ClickTriggerCollider] ����û�������κ����壬�����Ч��");
            }
        }
    }

}
