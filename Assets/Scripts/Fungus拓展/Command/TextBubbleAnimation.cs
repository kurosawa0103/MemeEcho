using UnityEngine;
using Fungus;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

[CommandInfo("Custom",
             "Text Bubble Animation",
             "Control a TextMeshPro UI and Dialog Box animation.")]
public class TextBubbleAnimation : Command
{
    [Header("预览功能")]
    [Tooltip("是否在编辑器中预览对话框")]
    public bool previewInEditor = false;

    [Tooltip("勾选时销毁所有现有气泡对话框")]
    public bool destroyAllDialogsOnToggle = false;

    private GameObject previewDialog;

    [Tooltip("The text to display in the TextMeshPro UI"), TextArea(3, 6)]
    public string dialogText;

    [Tooltip("场景对话框预制体")]
    public GameObject customDialogPrefab;

    [Header("字体设置")]
    [Tooltip("自定义字体大小（0表示使用默认）")]
    public float customFontSize = 24f;


    [Tooltip("等待时间")]
    public float waitTime = 1f;

    public Vector2 boxSize=new Vector2 (500,0);

    public bool dialogBoxUseScale = true;

    [Tooltip("The duration of the fade-in animation for the text")]
    public float fadeInDuration = 0.5f;

    [Tooltip("The duration of the fade-out animation for the text")]
    public float fadeOutDuration = 0.5f;

    [Tooltip("The duration for dialog box scale-in")]
    public float scaleInDuration = 0.5f;

    [Tooltip("The duration for dialog box scale-out")]
    public float scaleOutDuration = 0.5f;

    [Tooltip("Custom Dialog才需要配置位置")]
    public Vector3 bubblePosition;

    [Tooltip("对话框偏移量")]
    public Vector3 dialogOffset;

    [Tooltip("勾选后才会关闭，不勾选则永远存在需要手动关闭")]
    public bool shouldCloseDialogBox = true;

    [Tooltip("等待完成才会继续")]
    public bool waitForCompletion = true;

    [Tooltip("在显示当前对话框前，是否先关闭所有其他对话框")]
    public bool closeAllDialogsFirst = false;

    private GameObject currentDialog;

    private static GameObject lastDialog; // 静态变量，用于追踪上一个对话框

    [Tooltip("是否强制关闭所有现有对话框（无视动画等待）")]
    public bool forceCloseAllDialogs = false;

    public string sfxName;
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
#if UNITY_EDITOR
    private bool needDestroyAllDialogs = false;

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (destroyAllDialogsOnToggle && !needDestroyAllDialogs)
            {
                needDestroyAllDialogs = true;
                // 自动取消勾选，防止重复调用
                destroyAllDialogsOnToggle = false;
            }

            if (previewInEditor)
            {
                if (previewDialog == null)
                {
                    CreatePreviewDialog();
                }
            }
            if (previewDialog != null)
            {
                previewDialog.transform.localPosition = bubblePosition;
            }
        }
    }

    [UnityEditor.InitializeOnLoadMethod]
    private static void OnEditorUpdate()
    {
        UnityEditor.EditorApplication.update += EditorUpdate;
    }

    private static void EditorUpdate()
    {
        var commands = UnityEngine.Object.FindObjectsOfType<TextBubbleAnimation>();
        foreach (var cmd in commands)
        {
            if (cmd.needDestroyAllDialogs)
            {
                cmd.needDestroyAllDialogs = false;
                cmd.DestroyAllBubbleDialogs();
            }
        }
    }


    private void DestroyAllBubbleDialogs()
    {
        var allDialogs = GameObject.FindGameObjectsWithTag("BubbleBox");
        foreach (var dialog in allDialogs)
        {
            DestroyImmediate(dialog); // 这里没问题，因为是在EditorUpdate中调用
        }
    }


    private void CreatePreviewDialog()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        if (canvas == null) return;

        if (customDialogPrefab != null)
        {
            previewDialog = UnityEditor.PrefabUtility.InstantiatePrefab(customDialogPrefab, canvas.transform) as GameObject;
            previewDialog.transform.localPosition = bubblePosition;
        }

        if (previewDialog != null)
        {
            previewDialog.name = "PreviewDialog" ;
        }
    }
#endif
    private void CreateDialog()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");

        currentDialog = Instantiate(customDialogPrefab, canvas.transform);
        currentDialog.transform.localPosition = bubblePosition;

        lastDialog = currentDialog;

        BubbleTextShow dialogShow = currentDialog.GetComponent<BubbleTextShow>();
        if (dialogShow != null)
        {
            dialogShow.SetupDialog(dialogText, fadeInDuration, fadeOutDuration, customFontSize, dialogBoxUseScale, boxSize, sfxName);
        }

        StartCoroutine(WaitForClose());//等待自动关闭

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
        var allDialogs = GameObject.FindGameObjectsWithTag("BubbleBox");
        foreach (var dialog in allDialogs)
        {
            if (dialog != null)
            {
                var ds = dialog.GetComponent<BubbleTextShow>();
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
        BubbleTextShow dialogShow = currentDialog?.GetComponent<BubbleTextShow>();

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
        var allDialogs = GameObject.FindGameObjectsWithTag("BubbleBox");
        foreach (var dialog in allDialogs)
        {
            if (dialog != null)
            {
                dialog.GetComponent<BubbleTextShow>().CloseDialog(fadeOutDuration, scaleOutDuration);
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
        string prefix = forceCloseAllDialogs ? "[强制清除所有对话] " : "";
        return $"{prefix}: {dialogText}";
    }
    public override Color GetButtonColor()
    {
        return new Color32(100, 255, 200, 255);
    }
}
