using UnityEngine;
using UnityEngine.UI;

public class OpenDiary : MonoBehaviour
{
    public bool diaryIsOn = false;
    public GameObject diaryPanel;
    public PageManager pageManager;
    private IDiaryProgressHandler diaryHandler = new DiaryToJson(); // 获取当前 ID 的处理器
    public GameObject redPoint;
    public void OpenDiaryPanel()
    {
        if (!diaryIsOn)
        {
            redPoint.SetActive(false);
            diaryIsOn = true;
            diaryPanel.SetActive(true);

            // 强制重建布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(diaryPanel.GetComponent<RectTransform>());
        }
        else
        {
            diaryIsOn = false;
            diaryPanel.SetActive(false);
        }
    }
}
