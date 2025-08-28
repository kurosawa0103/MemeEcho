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

    [Tooltip("莉莉娅对话框预制体")]
    public GameObject playerDialogPrefab;

    [Tooltip("场景对话框预制体")]
    public GameObject customDialogPrefab;

    [Header("字体设置")]
    [Tooltip("自定义字体大小（0表示使用默认）")]
    public float customFontSize = 24f;

    [Tooltip("等待时间")]
    public float waitTime = 1f;

    [Tooltip("The duration of the fade-in animation for the text")]
    public float fadeInDuration = 0.5f;

    [Tooltip("The duration of the fade-out animation for the text")]
    public float fadeOutDuration = 0.5f;

    [Tooltip("The duration for dialog box scale-in")]
    public float scaleInDuration = 0.5f;

    [Tooltip("The duration for dialog box scale-out")]
    public float scaleOutDuration = 0.5f;

    [Tooltip("Custom Dialog才需要配置位置")]
    public Transform bubblePosition;

    [Tooltip("对话框偏移量")]
    public Vector3 dialogOffset;

    [Tooltip("勾选后才会关闭，不勾选则永远存在需要手动关闭")]
    public bool shouldCloseDialogBox = true;

    [Tooltip("等待完成才会继续")]
    public bool waitForCompletion = true;

    [Header ("点击相关")]
    [Tooltip("是否需要点击触发器才能关闭对话框")]
    public bool requireClickToClose = false;

    [Tooltip("关闭对话框所需点击的 GameObject（如按钮或触发器）")]
    public GameObject clickTrigger;

    [Tooltip("点击时忽略的层级（比如UI遮挡层）")]
    public LayerMask ignoreClickLayers = 1 << 5;

    [Tooltip("在显示当前对话框前，是否先关闭所有其他对话框")]
    public bool closeAllDialogsFirst = false;

    private GameObject currentDialog;
    private bool hasClickedToClose = false;
    private bool isWaitingForClick = false;

    private static GameObject lastDialog; // 静态变量，用于追踪上一个对话框

    [Tooltip("是否强制关闭所有现有对话框（无视动画等待）")]
    public bool forceCloseAllDialogs = false;

    public override void OnEnter()
    {
        if (forceCloseAllDialogs)
        {
            ForceDestroyAllDialogs();
            Continue(); // 继续后续流程（跳过本指令其他行为）
            return;
        }
        // 立即销毁上一个对话框（如果存在）
        if (lastDialog != null)
        {
            Destroy(lastDialog);
            lastDialog = null;
        }

        //勾选销毁的情况下，先销毁再生成
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
            Debug.LogWarning("[TextDialogAnimation] 对话框被中途销毁，终止当前 Block。");
            StopParentBlock(); // 不再调用 Continue，而是直接终止整个 Block
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
                Debug.LogWarning("ClickTrigger 对象上没有 Collider 组件！");
            }
            StartCoroutine(WaitForClick());//等待点击
        }

        if (!requireClickToClose )
        {
            StartCoroutine(WaitForClose());//等待自动关闭
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
        // 避免在被禁用/隐藏后继续执行逻辑
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

        Debug.Log($"[TextDialogAnimation] 收到点击来自：{clickTrigger.name}");
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
            Debug.LogWarning("[AutoCloseDialog] currentDialog 已被销毁。");
            Continue(); // 避免流程卡住
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

        // 清除上一次记录
        lastDialog = null;
    }

    public override void OnExit()
    {
        // 可选：退出时清除静态引用（避免引用悬挂）
        if (lastDialog == currentDialog)
        {
            lastDialog = null;
        }
    }

    public override string GetSummary()
    {
        if (forceCloseAllDialogs)
        {
            return "<color=red>[强制清除所有对话]</color>";
        }

        string actualSpeaker = dialogType == DialogType.PlayerDialog ? "<color=#FF69B4>莉莉娅：</color>" : "<color=#8BC7FF>自定义：</color>";
        return $"{actualSpeaker}: {dialogText}";
    }

}


public class ClickTriggerCollider : MonoBehaviour
{
    private TextDialogAnimation dialogAnimation;

    [Tooltip("点击时要忽略的层级（包括 UI）")]
    public LayerMask ignoreClickLayers;

    [Tooltip("当鼠标点击 UI 时，是否忽略特定 UI 层")]
    public bool ignoreUIOnLayer = true;

    [Tooltip("指定需要忽略的 UI 层")]
    public LayerMask uiLayerMask = 1 << 5; // 默认 UI 层

    [Tooltip("指定的触发器物体")]
    public GameObject clickTrigger;

    public void Initialize(TextDialogAnimation dialog,GameObject trigger, LayerMask ignoreLayers)
    {
        dialogAnimation = dialog;
        ignoreClickLayers = ignoreLayers;
        clickTrigger = trigger;
    }

    private void Update()
    {
        // 只处理鼠标点击事件
        if (Input.GetMouseButtonDown(0))
        {
            // 首先检查是否点击到了 UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                // 获取所有的 UI 点击结果
                var raycastResults = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, raycastResults);

                foreach (var result in raycastResults)
                {
                    // 如果点击的 UI 元素在指定的 UI 层
                    if (((1 << result.gameObject.layer) & uiLayerMask) != 0)
                    {
                        //Debug.Log($"[ClickTriggerCollider] 点击到了指定的 UI 层：{result.gameObject.name}，忽略处理。");

                        return; // 如果点击到忽略的 UI 层，直接返回
                    }
                }
            }

            // 射线检测场景物体
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {

                Debug.Log($"[ClickTriggerCollider] 射线命中物体：{hit.collider.gameObject.name} (ID: {hit.collider.gameObject.GetInstanceID()})");
                CharacterObject characterBehavior = hit.collider.gameObject.GetComponent<CharacterObject>();
                if (characterBehavior != null)
                {
                    // 比较命中物体是不是配置的 clickTrigger
                    //此外还需检查是否能点击
                    if (clickTrigger != null && hit.collider.gameObject == clickTrigger && characterBehavior.canClickTalk)
                    {
                        //Debug.Log($"[ClickTriggerCollider] 命中配置的 Trigger：{clickTrigger.name}，执行触发逻辑。");

                        if (dialogAnimation != null)
                        {
                            dialogAnimation.OnTriggerClicked();
                        }
                    }
                    else
                    {
                        Debug.Log($"[ClickTriggerCollider] 命中了其他物体：{hit.collider.gameObject.name}，不是配置的 Trigger。");
                    }
                }
                
            }
            else
            {
                //Debug.Log("[ClickTriggerCollider] 射线没有命中任何物体，点击无效。");
            }
        }
    }

}
