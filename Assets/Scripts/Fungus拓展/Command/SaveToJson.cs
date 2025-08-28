using UnityEngine;
using Fungus;

[CommandInfo("Save",
             "SaveToJson",
             "���浽json�������Ի��������ռǽ���.")]
public class SaveToJson : Command
{
    private IGameDataHandler gameDataHandler = new GameDataToJson();
    private IDiaryProgressHandler diaryHandler = new DiaryToJson();
    private IEventStateDataHandler eventStateDataHandler = new JsonGenerator();
    private PageManager pageManager;

    [Tooltip("��ǰҳ�루��0��ʼ��")]
    public int pageIndex;

    [Tooltip("��ǰҳ�ڵ���Ŀ��������0��ʼ��")]
    public int entryIndex;

    public bool clearSave;
    public override void OnEnter()
    {
        pageManager = FindObjectOfType<PageManager>();
        pageManager.currentPage = pageIndex;
        pageManager.currentProgress = entryIndex;
        // ���浱ǰ�ռ��Ķ�����
        diaryHandler.SaveProgress(pageIndex, entryIndex);

        if (clearSave)
        {
            diaryHandler.ClearProgress();
            eventStateDataHandler.ClearEventStateData();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save(); // ȷ�����ı�����
        }
        Continue();
    }

    public override string GetSummary()
    {
        if (clearSave)
        {
            return "������д浵�������������¼�״̬";
        }
        else
        {
            return $"������� | ��ǰҳ: {pageIndex} ��Ŀ: {entryIndex}";
        }
    }

}
