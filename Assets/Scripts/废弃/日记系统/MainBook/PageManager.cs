using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour
{
    [Header("ҳ������")]
    public Page pageDataConfig; // ҳ������ ScriptableObject

    [Header("��ҳ��ť")]
    public Button nextButton;              // ��һҳ��ť
    public Button previousButton;          // ��һҳ��ť

    private int currentPageIndex = 0;      // ��ǰҳ��
    private GameObject currentPageInstance;
    public GameObject bookPanel;

    public int currentPage;
    public int currentProgress;

    private void Start()
    {
        // ��ʼ��ҳ��Ͱ�ť״̬
        LoadPage(currentPageIndex);
        UpdateButtonVisibility();
    }

    private void LoadPage(int pageIndex)
    {
        // ж�ص�ǰҳ��
        if (currentPageInstance != null)
        {
            Destroy(currentPageInstance);
        }

        // ʵ�����µ�ҳ��
        if (IsValidPageIndex(pageIndex))
        {
            PageData pageData = pageDataConfig.pages[pageIndex];

            // ��鵱ǰҳ���Ƿ�����
            if (!pageData.isLocked) // ���ҳ��δ������
            {
                currentPageInstance = Instantiate(pageData.prefab, bookPanel.transform);
                currentPageInstance.transform.SetAsLastSibling(); // ����Ϊ���ϲ�
            }
            else
            {
                Debug.LogWarning($"ҳ�� {pageIndex} �ѱ��������޷����ء�");
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
