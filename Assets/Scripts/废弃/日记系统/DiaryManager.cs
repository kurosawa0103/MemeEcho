using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryManager : MonoBehaviour
{
    public List<Page> pages;        // �洢���� Page �����ļ�
    public Transform pageContainer; // ҳ����õĸ�������
    public Button prevButton;       // ��һҳ��ť
    public Button nextButton;       // ��һҳ��ť

    private int currentPageIndex = 0;  // ��ǰҳ����
    private GameObject currentPage;    // ��ǰ���ص�ҳ��ʵ��

    void Start()
    {
        RefreshDiary(); // ����ʱˢ���ռǲ����ص�һҳ
    }

    // ˢ���ռ��б�
    public void RefreshDiary()
    {

        if (pages.Count > 0)
        {
            currentPageIndex = pages.Count - 1; // ����Ϊ���һҳ
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
        //    Debug.LogWarning($"Page {index} û�а�Ԥ���壡");
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

    // ģ������µ� Page ���ݣ������滻Ϊ��ȡ�浵��������ʽ��ȡ����
}
