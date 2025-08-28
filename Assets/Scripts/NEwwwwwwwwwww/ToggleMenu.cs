using UnityEngine;
using UnityEngine.UI;

public class ToggleMenu : MonoBehaviour
{
    [Header("按钮列表")]
    public Button[] buttons;

    [Header("对应的面板列表（可为空）")]
    public GameObject[] panels;

    private void Start()
    {
        // 给每个按钮绑定点击事件
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // 避免闭包问题
            buttons[i].onClick.AddListener(() => ShowPanel(index));
        }

        // 默认关闭所有 Panel
        CloseAllPanels();
    }

    public void ShowPanel(int index)
    {
        CloseAllPanels();

        // 如果 panels 数组没设置或者对应索引为空，直接返回
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
