using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public TMP_Text dialogueText; // TextMeshProUGUI �ı����
    public RectTransform dialogueTextRect; // �Ի���� RectTransform
    public RectTransform dialogueBox; // �Ի���� RectTransform
    public float maxWidth = 500f; // ����ȣ�������ֵ�ı��ỻ��
    public float padding = 10f; // �ڱ߾�


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
        // �����ı�������ȣ��ı�����������Զ�����
        dialogueText.enableWordWrapping = true;

        // ǿ�Ƹ����ı�����������ȷ��������ȷ��
        dialogueText.ForceMeshUpdate();

        // ��ȡ�ı����ݵĿ�Ⱥ͸߶�
        float preferredWidth = dialogueText.preferredWidth;
        float preferredHeight = dialogueText.preferredHeight;

        // ���öԻ���Ŀ�ȣ�ȷ������������ȣ��������ڱ߾�
        float width = Mathf.Min(maxWidth, preferredWidth) + padding * 2;
        dialogueBox.sizeDelta = new Vector2(width, preferredHeight + padding * 2); // ���ÿ��
        dialogueTextRect.sizeDelta = new Vector2(width, preferredHeight + padding * 2); // ���ÿ��
        // ����ı��߶�������RectTransform �߶�����Ӧ
        dialogueBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight + padding * 2);
        dialogueTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight + padding * 2);

    }
}
