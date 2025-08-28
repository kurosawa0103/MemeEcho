using UnityEngine;
using UnityEngine.UI;

public class ToggleMenu : MonoBehaviour
{
    [Header("��ť�б�")]
    public Button[] buttons;

    [Header("��Ӧ������б���Ϊ�գ�")]
    public GameObject[] panels;

    private void Start()
    {
        // ��ÿ����ť�󶨵���¼�
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // ����հ�����
            buttons[i].onClick.AddListener(() => ShowPanel(index));
        }

        // Ĭ�Ϲر����� Panel
        CloseAllPanels();
    }

    public void ShowPanel(int index)
    {
        CloseAllPanels();

        // ��� panels ����û���û��߶�Ӧ����Ϊ�գ�ֱ�ӷ���
        if (panels == null || index >= panels.Length || panels[index] == null)
            return;

        panels[index].SetActive(true);
    }

    private void CloseAllPanels()
    {
        if (panels == null) return;

        foreach (var panel in panels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }
}
