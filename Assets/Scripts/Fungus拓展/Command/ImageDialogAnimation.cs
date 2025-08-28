using UnityEngine;
using Fungus;
using DG.Tweening;
using UnityEngine.UI;

public enum ImageDialogType
{
    Player,
    Custom
}
[CommandInfo("Custom", "Image Dialog Animation", "Display an image dialog animation (Character or Custom).")]
public class ImageDialogAnimation : Command
{
    [Tooltip("对话图片")]
    public Sprite imageToShow;

    [Tooltip("对话框类型")]
    public ImageDialogType dialogType = ImageDialogType.Custom;

    [Tooltip("角色对话框Prefab")]
    public GameObject characterDialogPrefab;

    [Tooltip("自定义图片对话框Prefab")]
    public GameObject customDialogPrefab;

    [Tooltip("跟随的世界位置对象")]
    public Transform bubblePosition;

    [Tooltip("在跟随对象基础上的偏移")]
    public Vector2 dialogOffset;

    [Tooltip("展示时间")]
    public float waitTime = 2.5f;

    [Tooltip("淡入淡出与缩放时长")]
    public float fadeInDuration = 0.75f;
    public float fadeOutDuration = 0.75f;
    public float scaleInDuration = 0.75f;
    public float scaleOutDuration = 0.75f;

    [Tooltip("是否等待动画完成")]
    public bool waitForCompletion = true;

    private GameObject currentDialog;
    private static GameObject lastDialog;

    public override void OnEnter()
    {
        if (lastDialog != null)
        {
            Destroy(lastDialog);
            lastDialog = null;
        }

        GameObject boxBorder = GameObject.FindGameObjectWithTag("BoxBorder");
        if (boxBorder == null)
        {
            Debug.LogWarning("未找到 BoxBorder 节点！");
            Continue();
            return;
        }

        GameObject prefabToUse = dialogType == ImageDialogType.Player ? characterDialogPrefab : customDialogPrefab;
        currentDialog = GameObject.Instantiate(prefabToUse, boxBorder.transform.GetChild(0));
        UIFollowWorldObject follow = currentDialog.GetComponent<UIFollowWorldObject>();
        if (follow != null && bubblePosition != null)
        {
            follow.worldObject = bubblePosition;
            follow.offset = dialogOffset;
        }
        currentDialog.transform.SetAsFirstSibling();
        lastDialog = currentDialog;

        ImageShow imageShow = currentDialog.GetComponent<ImageShow>();
        if (imageShow != null)
        {
            if (dialogType == ImageDialogType.Player)
            {
                imageShow.SetupCharacterImage(imageToShow, fadeInDuration, fadeOutDuration, scaleInDuration, scaleOutDuration);
            }
            else
            {
                imageShow.SetupImage(imageToShow, fadeInDuration, fadeOutDuration, scaleInDuration, scaleOutDuration);
            }
        }

        DOVirtual.DelayedCall(waitTime, CloseDialog);
    }

    private void CloseDialog()
    {
        ImageShow imageShow = currentDialog?.GetComponent<ImageShow>();

        if (imageShow != null)
        {
            imageShow.CloseImage(fadeOutDuration, scaleOutDuration);
        }

        void ContinueFlow() => Continue();

        if (waitForCompletion)
        {
            DOVirtual.DelayedCall(fadeOutDuration + scaleOutDuration, ContinueFlow);
        }
        else
        {
            ContinueFlow();
        }
    }

    public override string GetSummary()
    {
        return $"{dialogType} - {(imageToShow != null ? imageToShow.name : "No Image")}";
    }
    public override Color GetButtonColor()
    {
        return new Color32(33, 147, 199, 255);
    }
    public override void OnExit()
    {
        if (lastDialog == currentDialog)
        {
            lastDialog = null;
        }
    }
}

