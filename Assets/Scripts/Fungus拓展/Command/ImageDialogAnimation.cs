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
    [Tooltip("�Ի�ͼƬ")]
    public Sprite imageToShow;

    [Tooltip("�Ի�������")]
    public ImageDialogType dialogType = ImageDialogType.Custom;

    [Tooltip("��ɫ�Ի���Prefab")]
    public GameObject characterDialogPrefab;

    [Tooltip("�Զ���ͼƬ�Ի���Prefab")]
    public GameObject customDialogPrefab;

    [Tooltip("���������λ�ö���")]
    public Transform bubblePosition;

    [Tooltip("�ڸ����������ϵ�ƫ��")]
    public Vector2 dialogOffset;

    [Tooltip("չʾʱ��")]
    public float waitTime = 2.5f;

    [Tooltip("���뵭��������ʱ��")]
    public float fadeInDuration = 0.75f;
    public float fadeOutDuration = 0.75f;
    public float scaleInDuration = 0.75f;
    public float scaleOutDuration = 0.75f;

    [Tooltip("�Ƿ�ȴ��������")]
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
            Debug.LogWarning("δ�ҵ� BoxBorder �ڵ㣡");
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

