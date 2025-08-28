using UnityEngine;
using Fungus;
using UnityEngine.UI;

[CommandInfo("Custom",
             "ShowRedPoint",
             "显示地图或日记本的红点提示，并可选启用其按钮交互")]
public class ShowRedPoint : Command
{
    public enum TargetType { Map, Diary }

    [Tooltip("选择要显示红点的目标")]
    public TargetType target;

    [Tooltip("是否启用该按钮的交互（可点击）")]
    public bool setButtonInteractive = false;

    public override void OnEnter()
    {
        switch (target)
        {
            case TargetType.Map:
                ShowMapRedPoint();
                break;
            case TargetType.Diary:
                ShowDiaryRedPoint();
                break;
        }

        Continue(); // 继续执行Fungus Flow
    }

    void ShowMapRedPoint()
    {
        OpenMap openMap = GameObject.FindObjectOfType<OpenMap>();
        if (openMap != null)
        {
            if (openMap.redPoint != null)
            {
                openMap.redPoint.SetActive(true);
            }

            if (setButtonInteractive && openMap.GetComponent<Button>() != null)
            {
                openMap.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            Debug.LogWarning("OpenMap 或其 redPoint 未找到！");
        }
    }

    void ShowDiaryRedPoint()
    {
        OpenDiary openDiary = GameObject.FindObjectOfType<OpenDiary>();
        if (openDiary != null)
        {
            if (openDiary.redPoint != null)
            {
                openDiary.redPoint.SetActive(true);
            }

            if (setButtonInteractive && openDiary.GetComponent<Button>() != null)
            {
                openDiary.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            Debug.LogWarning("OpenDiary 或其 redPoint 未找到！");
        }
    }

    public override string GetSummary()
    {
        string interactiveText = setButtonInteractive ? "（并启用按钮）" : "";
        return $"显示 {target} 的红点{interactiveText}";
    }
}
