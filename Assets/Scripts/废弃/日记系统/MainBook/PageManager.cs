using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour
{
    [Header("页面配置")]
    public Page pageDataConfig; // 页面配置 ScriptableObject

    [Header("翻页按钮")]
    public Button nextButton;              // 下一页按钮
    public Button previousButton;          // 上一页按钮

    private int currentPageIndex = 0;      // 当前页码
    private GameObject currentPageInstance;
    public GameObject bookPanel;

    public int currentPage;
    public int currentProgress;

    private void Start()
    {
        // 初始化页面和按钮状态
        LoadPage(currentPageIndex);
        UpdateButtonVisibility();
    }

    private void LoadPage(int pageIndex)
    {
        // 卸载当前页面
        if (currentPageInstance != null)
        {
            Destroy(currentPageInstance);
        }

        // 实例化新的页面
        if (IsValidPageIndex(pageIndex))
        {
            PageData pageData = pageDataConfig.pages[pageIndex];

            // 检查当前页面是否被锁定
            if (!pageData.isLocked) // 如果页面未被锁定
            {
                currentPageInstance = Instantiate(pageData.prefab, bookPanel.transform);
                currentPageInstance.transform.SetAsLastSibling(); // 设置为最上层
            }
            else
            {
                Debug.LogWarning($"页面 {pageIndex} 已被锁定，无法加载。");
            }
        }

        UpdateButtonVisibility();
    }

    public void NextPage()
    {
        if (currentPageIndex < pageDataConfig.pages.Length - 1)
        {
            currentPageIndex++;
            LoadPage(currentPageIndex);
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            LoadPage(currentPageIndex);
        }
    }

    private void UpdateButtonVisibility()
    {
        previousButton.interactable = currentPageIndex > 0;
        nextButton.interactable = IsValidPageIndex(currentPageIndex + 1) && !pageDataConfig.pages[currentPageIndex + 1].isLocked;
    }

    private bool IsValidPageIndex(int pageIndex)
    {
        return pageIndex >= 0 && pageIndex < pageDataConfig.pages.Length;
    }

    public void OpenBook()
    {
        bookPanel.SetActive(true);
        LoadPage(currentPageIndex);
        UpdateButtonVisibility();

    }

    public void CloseBook()
    {
        bookPanel.SetActive(false);
    }
}
