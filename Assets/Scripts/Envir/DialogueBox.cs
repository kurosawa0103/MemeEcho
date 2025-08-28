using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public TMP_Text dialogueText; // TextMeshProUGUI 文本组件
    public RectTransform dialogueTextRect; // 对话框的 RectTransform
    public RectTransform dialogueBox; // 对话框的 RectTransform
    public float maxWidth = 500f; // 最大宽度，超过此值文本会换行
    public float padding = 10f; // 内边距


    void Start()
    {

    }

    private void Update()
    {

    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
        AdjustDialogueBox();
    }

    public void AdjustDialogueBox()
    {
        // 设置文本的最大宽度，文本如果超出会自动换行
        dialogueText.enableWordWrapping = true;

        // 强制更新文本（更新网格，确保计算正确）
        dialogueText.ForceMeshUpdate();

        // 获取文本内容的宽度和高度
        float preferredWidth = dialogueText.preferredWidth;
        float preferredHeight = dialogueText.preferredHeight;

        // 设置对话框的宽度，确保不超过最大宽度，并加上内边距
        float width = Mathf.Min(maxWidth, preferredWidth) + padding * 2;
        dialogueBox.sizeDelta = new Vector2(width, preferredHeight + padding * 2); // 设置宽高
        dialogueTextRect.sizeDelta = new Vector2(width, preferredHeight + padding * 2); // 设置宽高
        // 如果文本高度增长，RectTransform 高度自适应
        dialogueBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight + padding * 2);
        dialogueTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight + padding * 2);

    }
}
