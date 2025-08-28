using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Sirenix.OdinInspector;
using System.Linq; // ��Ҫ���� LINQ
using System.Collections;

public enum CharacterType
{
    Player,    // ���
    Lilia      // �����
}
public enum DialogueType
{
    Text,      // ����
    Image      // ͼƬ
}

[System.Serializable] // ʹ�� DialogueEntry �����ڱ༭������ʾ
public class DialogueEntry
{
    public CharacterType Speaker; // ˵����ɫ����һ�����櫣�
    public DialogueType Type;     // �Ի����ͣ����ֻ�ͼƬ��

    public Sprite headSprite;    // �Ի�ͷ�����ֺ�ͼƬ�Ի�����Ҫ��

    [TextArea(5, 8), ShowIf("IsText")]
    public string DialogueText;  // ��������

    [ShowIf("IsImage")]
    public Sprite ImageSprite;    // ͼƬ���ݣ�����TypeΪImageʱʹ�ã�

    private bool IsText => Type == DialogueType.Text;  // �ж��Ƿ�Ϊ����
    private bool IsImage => Type == DialogueType.Image; // �ж��Ƿ�ΪͼƬ

    public DialogueEntry(string dialogueText, CharacterType speaker, Sprite headSprite = null)
    {
        DialogueText = dialogueText;
        Speaker = speaker;
        Type = DialogueType.Text;  // Ĭ��Ϊ��������
        ImageSprite = null;
        this.headSprite = headSprite;  // �Ի�ͷ��
    }

    public DialogueEntry(Sprite imageSprite, CharacterType speaker, Sprite headSprite = null)
    {
        DialogueText = string.Empty;
        Speaker = speaker;
        Type = DialogueType.Image;  // ����ΪͼƬ����
        ImageSprite = imageSprite;
        this.headSprite = headSprite;  // �Ի�ͷ��
    }
}

[System.Serializable]
public class DialogueEntryData
{
    public CharacterType Speaker;
    public DialogueType Type;
    public string DialogueText;
    public string headSpritePath;
    public string ImageSpritePath;

    public DialogueEntryData(DialogueEntry entry)
    {
        Speaker = entry.Speaker;
        Type = entry.Type;
        DialogueText = entry.DialogueText;
        headSpritePath = GetSpritePath(entry.headSprite);
        ImageSpritePath = GetSpritePath(entry.ImageSprite);
    }

    private string GetSpritePath(Sprite sprite)
    {
        if (sprite == null) return "";

        string path = "";

        // ȷ����Դ���� Resources Ŀ¼
        if (sprite.texture.name.Contains("Head"))
        {
            path = "Head/" + sprite.name; // ͷ����Դ·��
        }
        else if (sprite.texture.name.Contains("ChatImage"))
        {
            path = "ChatImage/" + sprite.name; // �Ի�ͼƬ��Դ·��
        }
        else
        {
            path = sprite.name; // Ĭ�����
        }

        return path;
    }
}

public class DialogueManager : MonoBehaviour
{
    [Header("�Ի�����")]
    [SerializeField]
    private int maxDialogueCount = 5;  // ���Ի���������¶�ڱ༭����

    [Header("�Ի��б�")]
    [SerializeField]
    private List<DialogueEntry> dialogueList = new List<DialogueEntry>();  // �洢�Ի����б�

    [Header("UI����")]
    [SerializeField]
    private GameObject playerDialoguePrefab;  // ��ҶԻ�UIԤ����
    [SerializeField]
    private GameObject liliaDialoguePrefab;  // ����櫶Ի�UIԤ����
    [SerializeField]
    private GameObject playerImagePrefab;  // ͼƬ�Ի�UIԤ����
    [SerializeField]
    private GameObject liliaImagePrefab;  // ͼƬ�Ի�UIԤ����
    [SerializeField]
    private Transform dialogueContainer;  // ��ŶԻ���������ͨ����һ��UI��Panel����LayoutGroup

    public ScrollRect scrollRect;

    [Header("�Ų�����")]
    [SerializeField]
    private float verticalSpacing = 10f; // ��ֱ���
    private string saveFilePath => Path.Combine(Application.persistentDataPath, "dialogue_data.json");

    // ��ȡ���Ի�����
    public int MaxDialogueCount
    {
        get { return maxDialogueCount; }
        set { maxDialogueCount = value; }
    }

    // ��ӶԻ����б�
    public void AddDialogue(string dialogueText, CharacterType speaker, Sprite headSprite = null)
    {
        // �����µĶԻ���Ŀ
        DialogueEntry newEntry = new DialogueEntry(dialogueText, speaker, headSprite);

        // ����б���������ޣ�ɾ������ĶԻ�
        if (dialogueList.Count >= maxDialogueCount)
        {
            dialogueList.RemoveAt(0); // ɾ�������һ���Ի�
        }

        // ����¶Ի����б�
        dialogueList.Add(newEntry);
        RefreshUI();
    }

    // ���ͼƬ�Ի����б�
    public void AddImageDialogue(Sprite imageSprite, CharacterType speaker, Sprite headSprite = null)
    {
        DialogueEntry newEntry = new DialogueEntry(imageSprite, speaker, headSprite);
        if (dialogueList.Count >= maxDialogueCount)
        {
            dialogueList.RemoveAt(0); // ɾ�������һ���Ի�
        }
        dialogueList.Add(newEntry);
        RefreshUI();
    }

    public void RefreshUI()
    {
        // �������еĶԻ�UI
        foreach (Transform child in dialogueContainer)
        {
            Destroy(child.gameObject);
        }


        // ���ݶԻ��б���������UI
        foreach (DialogueEntry entry in dialogueList)
        {
            GameObject dialogueUI = null;

            // ���ݶԻ�����ѡ��ͬ��UIԤ����
            if (entry.Type == DialogueType.Text)
            {
                // ѡ������UIԤ����
                if (entry.Speaker == CharacterType.Player)
                {
                    dialogueUI = Instantiate(playerDialoguePrefab, dialogueContainer);  // ������ֶԻ�
                }
                else if (entry.Speaker == CharacterType.Lilia)
                {
                    dialogueUI = Instantiate(liliaDialoguePrefab, dialogueContainer);  // ��������ֶԻ�
                }

                TextMeshProUGUI dialogueText = dialogueUI.GetComponentsInChildren<TextMeshProUGUI>(true)
                                                            .FirstOrDefault(text => text.gameObject.name == "dialogueText");
                dialogueText.text = entry.DialogueText;  // �����ı�����

                Image headImage = dialogueUI.GetComponentsInChildren<Image>(true)
                                                            .FirstOrDefault(img => img.gameObject.name == "head");
                if (headImage != null)
                {
                    headImage.sprite = entry.headSprite;  // ����ͷ��
                }
            }
            else if (entry.Type == DialogueType.Image)
            {
                // ѡ��ͼƬUIԤ����
                if (entry.Speaker == CharacterType.Player)
                {
                    dialogueUI = Instantiate(playerImagePrefab, dialogueContainer);  // ���ͼƬ�Ի�
                }
                else if (entry.Speaker == CharacterType.Lilia)
                {
                    dialogueUI = Instantiate(liliaImagePrefab, dialogueContainer);  // �����ͼƬ�Ի�
                }

                Image dialogueImage = dialogueUI.GetComponentsInChildren<Image>(true)
                                 .FirstOrDefault(img => img.gameObject.name == "picture");

                Image headImage = dialogueUI.GetComponentsInChildren<Image>(true)
                                            .FirstOrDefault(img => img.gameObject.name == "head");
                dialogueImage.sprite = entry.ImageSprite;  // ����ͼƬ����

                // ����ͷ��
                if (headImage != null)
                {
                    headImage.sprite = entry.headSprite;  // ����ͷ��
                }
            }
        }

        // �ӳٹ���
        StartCoroutine(DelayScrollToBottom());
    }
    private IEnumerator DelayScrollToBottom()
    {
        // �ȴ�һ֡��ȷ��UI��ȫ��Ⱦ���ٹ���
        yield return null;

        // ǿ���ؽ�����
        RectTransform containerRectTransform = dialogueContainer.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerRectTransform);

        // ��������ײ�
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0;
        }

        Debug.Log("��������ײ�");
    }

    public void SaveDialogueToJson()
    {
        List<DialogueEntryData> dialogueDataList = new List<DialogueEntryData>();
        foreach (var entry in dialogueList)
        {
            dialogueDataList.Add(new DialogueEntryData(entry));
        }

        string json = JsonUtility.ToJson(new DialogueSaveData(dialogueDataList), true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"�Ի������ѱ����� {saveFilePath}");
    }

    public void LoadDialogueFromJson()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("û���ҵ��浵�ļ���");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        DialogueSaveData loadedData = JsonUtility.FromJson<DialogueSaveData>(json);

        dialogueList.Clear();
        foreach (var entryData in loadedData.dialogues)
        {
            Sprite headSprite = LoadSpriteFromResources(entryData.headSpritePath);
            Sprite imageSprite = LoadSpriteFromResources(entryData.ImageSpritePath);

            if (entryData.Type == DialogueType.Text)
            {
                dialogueList.Add(new DialogueEntry(entryData.DialogueText, entryData.Speaker, headSprite));
            }
            else if (entryData.Type == DialogueType.Image)
            {
                dialogueList.Add(new DialogueEntry(imageSprite, entryData.Speaker, headSprite));
            }
        }

        RefreshUI();
        Debug.Log("�Ի������ѳɹ�����");
    }

    private Sprite LoadSpriteFromResources(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        return Resources.Load<Sprite>(path);
    }

    // ��ȡ���жԻ�
    public List<DialogueEntry> GetDialogueList()
    {
        return dialogueList;
    }

    // ��ȡ���һ���Ի�
    public DialogueEntry GetLastDialogue()
    {
        if (dialogueList.Count > 0)
        {
            return dialogueList[dialogueList.Count - 1];
        }
        return null;
    }
}

[System.Serializable]
public class DialogueSaveData
{
    public List<DialogueEntryData> dialogues;

    public DialogueSaveData(List<DialogueEntryData> dialogues)
    {
        this.dialogues = dialogues;
    }
}