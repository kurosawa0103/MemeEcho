using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Sirenix.OdinInspector;
using System.Linq; // 需要引入 LINQ
using System.Collections;

public enum CharacterType
{
    Player,    // 玩家
    Lilia      // 莉莉娅
}
public enum DialogueType
{
    Text,      // 文字
    Image      // 图片
}

[System.Serializable] // 使得 DialogueEntry 可以在编辑器中显示
public class DialogueEntry
{
    public CharacterType Speaker; // 说话角色（玩家或莉莉娅）
    public DialogueType Type;     // 对话类型（文字或图片）

    public Sprite headSprite;    // 对话头像（文字和图片对话都需要）

    [TextArea(5, 8), ShowIf("IsText")]
    public string DialogueText;  // 文字内容

    [ShowIf("IsImage")]
    public Sprite ImageSprite;    // 图片内容（仅在Type为Image时使用）

    private bool IsText => Type == DialogueType.Text;  // 判断是否为文字
    private bool IsImage => Type == DialogueType.Image; // 判断是否为图片

    public DialogueEntry(string dialogueText, CharacterType speaker, Sprite headSprite = null)
    {
        DialogueText = dialogueText;
        Speaker = speaker;
        Type = DialogueType.Text;  // 默认为文字类型
        ImageSprite = null;
        this.headSprite = headSprite;  // 对话头像
    }

    public DialogueEntry(Sprite imageSprite, CharacterType speaker, Sprite headSprite = null)
    {
        DialogueText = string.Empty;
        Speaker = speaker;
        Type = DialogueType.Image;  // 设置为图片类型
        ImageSprite = imageSprite;
        this.headSprite = headSprite;  // 对话头像
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

        // 确保资源来自 Resources 目录
        if (sprite.texture.name.Contains("Head"))
        {
            path = "Head/" + sprite.name; // 头像资源路径
        }
        else if (sprite.texture.name.Contains("ChatImage"))
        {
            path = "ChatImage/" + sprite.name; // 对话图片资源路径
        }
        else
        {
            path = sprite.name; // 默认情况
        }

        return path;
    }
}

public class DialogueManager : MonoBehaviour
{
    [Header("对话配置")]
    [SerializeField]
    private int maxDialogueCount = 5;  // 最大对话数量，暴露在编辑器中

    [Header("对话列表")]
    [SerializeField]
    private List<DialogueEntry> dialogueList = new List<DialogueEntry>();  // 存储对话的列表

    [Header("UI配置")]
    [SerializeField]
    private GameObject playerDialoguePrefab;  // 玩家对话UI预制体
    [SerializeField]
    private GameObject liliaDialoguePrefab;  // 莉莉娅对话UI预制体
    [SerializeField]
    private GameObject playerImagePrefab;  // 图片对话UI预制体
    [SerializeField]
    private GameObject liliaImagePrefab;  // 图片对话UI预制体
    [SerializeField]
    private Transform dialogueContainer;  // 存放对话的容器，通常是一个UI的Panel或者LayoutGroup

    public ScrollRect scrollRect;

    [Header("排布配置")]
    [SerializeField]
    private float verticalSpacing = 10f; // 垂直间隔
    private string saveFilePath => Path.Combine(Application.persistentDataPath, "dialogue_data.json");

    // 获取最大对话数量
    public int MaxDialogueCount
    {
        get { return maxDialogueCount; }
        set { maxDialogueCount = value; }
    }

    // 添加对话到列表
    public void AddDialogue(string dialogueText, CharacterType speaker, Sprite headSprite = null)
    {
        // 创建新的对话条目
        DialogueEntry newEntry = new DialogueEntry(dialogueText, speaker, headSprite);

        // 如果列表超出最大上限，删除最早的对话
        if (dialogueList.Count >= maxDialogueCount)
        {
            dialogueList.RemoveAt(0); // 删除最早的一条对话
        }

        // 添加新对话到列表
        dialogueList.Add(newEntry);
        RefreshUI();
    }

    // 添加图片对话到列表
    public void AddImageDialogue(Sprite imageSprite, CharacterType speaker, Sprite headSprite = null)
    {
        DialogueEntry newEntry = new DialogueEntry(imageSprite, speaker, headSprite);
        if (dialogueList.Count >= maxDialogueCount)
        {
            dialogueList.RemoveAt(0); // 删除最早的一条对话
        }
        dialogueList.Add(newEntry);
        RefreshUI();
    }

    public void RefreshUI()
    {
        // 销毁现有的对话UI
        foreach (Transform child in dialogueContainer)
        {
            Destroy(child.gameObject);
        }


        // 根据对话列表重新生成UI
        foreach (DialogueEntry entry in dialogueList)
        {
            GameObject dialogueUI = null;

            // 根据对话类型选择不同的UI预制体
            if (entry.Type == DialogueType.Text)
            {
                // 选择文字UI预制体
                if (entry.Speaker == CharacterType.Player)
                {
                    dialogueUI = Instantiate(playerDialoguePrefab, dialogueContainer);  // 玩家文字对话
                }
                else if (entry.Speaker == CharacterType.Lilia)
                {
                    dialogueUI = Instantiate(liliaDialoguePrefab, dialogueContainer);  // 莉莉娅文字对话
                }

                TextMeshProUGUI dialogueText = dialogueUI.GetComponentsInChildren<TextMeshProUGUI>(true)
                                                            .FirstOrDefault(text => text.gameObject.name == "dialogueText");
                dialogueText.text = entry.DialogueText;  // 更新文本内容

                Image headImage = dialogueUI.GetComponentsInChildren<Image>(true)
                                                            .FirstOrDefault(img => img.gameObject.name == "head");
                if (headImage != null)
                {
                    headImage.sprite = entry.headSprite;  // 设置头像
                }
            }
            else if (entry.Type == DialogueType.Image)
            {
                // 选择图片UI预制体
                if (entry.Speaker == CharacterType.Player)
                {
                    dialogueUI = Instantiate(playerImagePrefab, dialogueContainer);  // 玩家图片对话
                }
                else if (entry.Speaker == CharacterType.Lilia)
                {
                    dialogueUI = Instantiate(liliaImagePrefab, dialogueContainer);  // 莉莉娅图片对话
                }

                Image dialogueImage = dialogueUI.GetComponentsInChildren<Image>(true)
                                 .FirstOrDefault(img => img.gameObject.name == "picture");

                Image headImage = dialogueUI.GetComponentsInChildren<Image>(true)
                                            .FirstOrDefault(img => img.gameObject.name == "head");
                dialogueImage.sprite = entry.ImageSprite;  // 设置图片内容

                // 设置头像
                if (headImage != null)
                {
                    headImage.sprite = entry.headSprite;  // 设置头像
                }
            }
        }

        // 延迟滚动
        StartCoroutine(DelayScrollToBottom());
    }
    private IEnumerator DelayScrollToBottom()
    {
        // 等待一帧以确保UI完全渲染后再滚动
        yield return null;

        // 强制重建布局
        RectTransform containerRectTransform = dialogueContainer.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerRectTransform);

        // 滚动到最底部
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0;
        }

        Debug.Log("滚动到最底部");
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
        Debug.Log($"对话数据已保存至 {saveFilePath}");
    }

    public void LoadDialogueFromJson()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("没有找到存档文件！");
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
        Debug.Log("对话数据已成功加载");
    }

    private Sprite LoadSpriteFromResources(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        return Resources.Load<Sprite>(path);
    }

    // 获取所有对话
    public List<DialogueEntry> GetDialogueList()
    {
        return dialogueList;
    }

    // 获取最近一条对话
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