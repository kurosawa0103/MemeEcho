using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class PageElement
{
    public GameObject element;
    public int pageIndex;     // �ڼ�ҳ
    public int entryIndex;    // ��ҳ�ĵڼ���

    [Range(0.1f, 5f)]
    public float fadeDuration = 1f;

    [Range(0f, 2f)]
    public float fadeInterval = 0.5f;

    public bool waitForFade = true;
}

public class BookPageShow : MonoBehaviour
{
    public string pageName; // ҳ�����ƣ�ʹ�� PlayerPrefs �洢�Ƿ񲥷�
    [Tooltip("ҳ��Ԫ�������б�")]
    [SerializeField]
    private List<PageElement> pageElements;

    public bool needShowNext = true;

    public PageManager pageManager;

    private void Start()
    {
        
    }
    private void OnEnable()
    {
        pageManager = FindObjectOfType<PageManager>();
        RefreshPageContent();
    }

    public void RefreshPageContent()
    {
        InitializeTransparency();
        ShowBookPages();
    }
    /// <summary>
    /// ��ʼ����������Ԫ�أ����������壩��͸����Ϊ 0
    /// </summary>
    private void InitializeTransparency()
    {
        foreach (var pageElement in pageElements)
        {
            if (pageElement.element == null)
            {
                Debug.LogWarning("PageElement �д��� null �� element ���ã�");
                continue;
            }

            // ��ȷ����ǰҳ���ڽ����ڵ�Ԫ��
            if (pageElement.pageIndex == pageManager.currentPage && pageElement.entryIndex <= pageManager.currentProgress)
            {
                pageElement.element.SetActive(true);
                if (HasElementPlayed(pageElement.pageIndex, pageElement.entryIndex))
                {
                    SetAlphaAllChildren(pageElement.element, 1f); // ֱ����ʾ
                }
                else
                {
                    SetAlphaAllChildren(pageElement.element, 0f); // ׼������
                }
            }
            else
            {
                //������������ص�Ԫ��
                pageElement.element.SetActive(false);
            }
        }

        if (pageManager != null)
        {
            pageManager.previousButton?.gameObject.SetActive(false);
            pageManager.nextButton?.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// ��ʾ����������ҳ��Ԫ�أ�entryIndex <= ��ǰ ID��
    /// </summary>
    public void ShowBookPages()
    {
        var filteredElements = pageElements.FindAll(pe =>
            pe.pageIndex == pageManager.currentPage && pe.entryIndex <= pageManager.currentProgress);
        filteredElements.Sort((a, b) => a.entryIndex.CompareTo(b.entryIndex)); // ��˳�򲥷�
        StartCoroutine(PlayFadeAnimations(filteredElements));
    }

    /// <summary>
    /// ���ŵ��붯���������������
    /// </summary>
    private System.Collections.IEnumerator PlayFadeAnimations(List<PageElement> filteredElements)
    {
        foreach (var pageElement in filteredElements)
        {
            if (pageElement.element == null) continue;

            pageElement.element.SetActive(true); // �ٴ�ȷ������
            FadeInAllChildren(pageElement.element, pageElement.fadeDuration);
            // �ڴ˼�¼Ϊ�Ѳ���
            MarkElementAsPlayed(pageElement.pageIndex, pageElement.entryIndex);
            if (pageElement.waitForFade)
            {
                yield return new WaitForSeconds(pageElement.fadeDuration + pageElement.fadeInterval);
            }
            else
            {
                yield return null;
            }
        }

        PlayerPrefs.SetInt(pageName, 1);
        PlayerPrefs.Save();

        ShowButtons();
    }


    /// <summary>
    /// ������ʾ����Ԫ�أ�������ɣ�
    /// </summary>
    public void CompleteFadeIn()
    {
        foreach (var pageElement in pageElements)
        {
            if (pageElement.element == null) continue;
            SetAlphaAllChildren(pageElement.element, 1f);
        }

        StopAllCoroutines();
        ShowButtons();
    }

    /// <summary>
    /// ��ʾ��һҳ����һҳ��ť
    /// </summary>
    private void ShowButtons()
    {
        if (pageManager == null) return;

        pageManager.previousButton?.gameObject.SetActive(true);
        pageManager.nextButton?.gameObject.SetActive(needShowNext);
    }

    /// <summary>
    /// ���øö��������������ϵ� Image / TMP �ı�͸����
    /// </summary>
    private void SetAlphaAllChildren(GameObject root, float alpha)
    {
        var texts = root.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in texts)
        {
            text.alpha = alpha;
        }

        var images = root.GetComponentsInChildren<Image>(true);
        foreach (var image in images)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }

    /// <summary>
    /// ͳһ���������
    /// </summary>
    private void FadeInAllChildren(GameObject root, float duration)
    {
        var texts = root.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in texts)
        {
            text.DOFade(1f, duration);
        }

        var images = root.GetComponentsInChildren<Image>(true);
        foreach (var image in images)
        {
            image.DOFade(1f, duration);
        }
    }
    private bool HasElementPlayed(int pageIndex, int entryIndex)
    {
        string key = $"{pageName}_p{pageIndex}_e{entryIndex}";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    private void MarkElementAsPlayed(int pageIndex, int entryIndex)
    {
        string key = $"{pageName}_p{pageIndex}_e{entryIndex}";
        PlayerPrefs.SetInt(key, 1);
    }

    public void ClearAllFadeRecords()
    {
        foreach (var element in pageElements)
        {
            string key = $"{pageName}_p{element.pageIndex}_e{element.entryIndex}";
            PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
    }
}
