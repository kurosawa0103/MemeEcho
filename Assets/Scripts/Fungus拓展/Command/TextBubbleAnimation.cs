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
    [Header("Ԥ������")]
    [Tooltip("�Ƿ��ڱ༭����Ԥ���Ի���")]
    public bool previewInEditor = false;

    [Tooltip("��ѡʱ���������������ݶԻ���")]
    public bool destroyAllDialogsOnToggle = false;

    private GameObject previewDialog;

    [Tooltip("The text to display in the TextMeshPro UI"), TextArea(3, 6)]
    public string dialogText;

    [Tooltip("�����Ի���Ԥ����")]
    public GameObject customDialogPrefab;

    [Header("��������")]
    [Tooltip("�Զ��������С��0��ʾʹ��Ĭ�ϣ�")]
    public float customFontSize = 24f;


    [Tooltip("�ȴ�ʱ��")]
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

    [Tooltip("Custom Dialog����Ҫ����λ��")]
    public Vector3 bubblePosition;

    [Tooltip("�Ի���ƫ����")]
    public Vector3 dialogOffset;

    [Tooltip("��ѡ��Ż�رգ�����ѡ����Զ������Ҫ�ֶ��ر�")]
    public bool shouldCloseDialogBox = true;

    [Tooltip("�ȴ���ɲŻ����")]
    public bool waitForCompletion = true;

    [Tooltip("����ʾ��ǰ�Ի���ǰ���Ƿ��ȹر����������Ի���")]
    public bool closeAllDialogsFirst = false;

    private GameObject currentDialog;

    private static GameObject lastDialog; // ��̬����������׷����һ���Ի���

    [Tooltip("�Ƿ�ǿ�ƹر��������жԻ������Ӷ����ȴ���")]
    public bool forceCloseAllDialogs = false;

    public string sfxName;
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
#if UNITY_EDITOR
    private bool needDestroyAllDialogs = false;

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (destroyAllDialogsOnToggle && !needDestroyAllDialogs)
            {
                needDestroyAllDialogs = true;
                // �Զ�ȡ����ѡ����ֹ�ظ�����
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
            DestroyImmediate(dialog); // ����û���⣬��Ϊ����EditorUpdate�е���
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

        StartCoroutine(WaitForClose());//�ȴ��Զ��ر�

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
            Debug.LogWarning("[AutoCloseDialog] currentDialog �ѱ����١�");
            Continue(); // �������̿�ס
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
        string prefix = forceCloseAllDialogs ? "[ǿ��������жԻ�] " : "";
        return $"{prefix}: {dialogText}";
    }
    public override Color GetButtonColor()
    {
        return new Color32(100, 255, 200, 255);
    }
}
