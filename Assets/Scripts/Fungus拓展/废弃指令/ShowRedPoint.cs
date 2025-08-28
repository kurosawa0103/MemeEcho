using UnityEngine;
using Fungus;
using UnityEngine.UI;

[CommandInfo("Custom",
             "ShowRedPoint",
             "��ʾ��ͼ���ռǱ��ĺ����ʾ������ѡ�����䰴ť����")]
public class ShowRedPoint : Command
{
    public enum TargetType { Map, Diary }

    [Tooltip("ѡ��Ҫ��ʾ����Ŀ��")]
    public TargetType target;

    [Tooltip("�Ƿ����øð�ť�Ľ������ɵ����")]
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

        Continue(); // ����ִ��Fungus Flow
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
            Debug.LogWarning("OpenMap ���� redPoint δ�ҵ���");
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
            Debug.LogWarning("OpenDiary ���� redPoint δ�ҵ���");
        }
    }

    public override string GetSummary()
    {
        string interactiveText = setButtonInteractive ? "�������ð�ť��" : "";
        return $"��ʾ {target} �ĺ��{interactiveText}";
    }
}
