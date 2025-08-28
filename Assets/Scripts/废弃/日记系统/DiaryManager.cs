using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryManager : MonoBehaviour
{
    public List<Page> pages;        // 存储所有 Page 配置文件
    public Transform pageContainer; // 页面放置的父级容器
    public Button prevButton;       // 上一页按钮
    public Button nextButton;       // 下一页按钮

    private int currentPageIndex = 0;  // 当前页索引
    private GameObject currentPage;    // 当前加载的页面实例

    void Start()
    {
        RefreshDiary(); // 启动时刷新日记并加载第一页
    }

    // 刷新日记列表
    public void RefreshDiary()
    {

        if (pages.Count > 0)
        {
            currentPageIndex = pages.Count - 1; // 重置为最后一页
            LoadPage(currentPageIndex);
        }
        else
        {
            currentPageIndex = 0;
            ClearCurrentPage();
        }

        UpdateButtons();
    }


    public void NextPage()
    {
        if (currentPageIndex < pages.Count - 1)
        {
            currentPageIndex++;
            LoadPage(currentPageIndex);
        }
    }

    public void PrevPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            LoadPage(currentPageIndex);
        }
    }

    private void LoadPage(int index)
    {
        ClearCurrentPage();

        //if (pages[index].prefab != null)
        //{
        //    currentPage = Instantiate(pages[index].prefab, pageContainer);
        //}
        //else
        //{
        //    Debug.LogWarning($"Page {index} 没有绑定预制体！");
        //}

        UpdateButtons();
    }

    private void ClearCurrentPage()
    {
        if (currentPage != null)
        {
            Destroy(currentPage);
            currentPage = null;
        }
    }

    private void UpdateButtons()
    {
        prevButton.interactable = (currentPageIndex > 0);
        nextButton.interactable = (currentPageIndex < pages.Count - 1);
    }

    // 模拟加载新的 Page 数据，可以替换为读取存档或其他方式获取数据
}
