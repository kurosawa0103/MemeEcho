using UnityEngine;
using UnityEngine.UI;

public class OpenDiary : MonoBehaviour
{
    public bool diaryIsOn = false;
    public GameObject diaryPanel;
    public PageManager pageManager;
    private IDiaryProgressHandler diaryHandler = new DiaryToJson(); // ��ȡ��ǰ ID �Ĵ�����
    public GameObject redPoint;
    public void OpenDiaryPanel()
    {
        if (!diaryIsOn)
        {
            redPoint.SetActive(false);
            diaryIsOn = true;
            diaryPanel.SetActive(true);

            // ǿ���ؽ�����
            LayoutRebuilder.ForceRebuildLayoutImmediate(diaryPanel.GetComponent<RectTransform>());
        }
        else
        {
            diaryIsOn = false;
            diaryPanel.SetActive(false);
        }
    }
}
