using UnityEngine;
using Fungus;

[CommandInfo("Save",
             "SaveToJson",
             "储存到json，包括对话描述和日记进度.")]
public class SaveToJson : Command
{
    private IGameDataHandler gameDataHandler = new GameDataToJson();
    private IDiaryProgressHandler diaryHandler = new DiaryToJson();
    private IEventStateDataHandler eventStateDataHandler = new JsonGenerator();
    private PageManager pageManager;

    [Tooltip("当前页码（从0开始）")]
    public int pageIndex;

    [Tooltip("当前页内的条目索引（从0开始）")]
    public int entryIndex;

    public bool clearSave;
    public override void OnEnter()
    {
        pageManager = FindObjectOfType<PageManager>();
        pageManager.currentPage = pageIndex;
        pageManager.currentProgress = entryIndex;
        // 储存当前日记阅读进度
        diaryHandler.SaveProgress(pageIndex, entryIndex);

        if (clearSave)
        {
            diaryHandler.ClearProgress();
            eventStateDataHandler.ClearEventStateData();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save(); // 确保更改被保存
        }
        Continue();
    }

    public override string GetSummary()
    {
        if (clearSave)
        {
            return "清除所有存档，包括进度与事件状态";
        }
        else
        {
            return $"保存进度 | 当前页: {pageIndex} 条目: {entryIndex}";
        }
    }

}
