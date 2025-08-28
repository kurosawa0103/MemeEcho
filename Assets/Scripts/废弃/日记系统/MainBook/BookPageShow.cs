using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class PageElement
{
    public GameObject element;
    public int pageIndex;     // 第几页
    public int entryIndex;    // 该页的第几条

    [Range(0.1f, 5f)]
    public float fadeDuration = 1f;

    [Range(0f, 2f)]
    public float fadeInterval = 0.5f;

    public bool waitForFade = true;
}

public class BookPageShow : MonoBehaviour
{
    public string pageName; // 页面名称，使用 PlayerPrefs 存储是否播放
    [Tooltip("页面元素配置列表")]
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
    /// 初始化所有配置元素（及其子物体）的透明度为 0
    /// </summary>
    private void InitializeTransparency()
    {
        foreach (var pageElement in pageElements)
        {
            if (pageElement.element == null)
            {
                Debug.LogWarning("PageElement 中存在 null 的 element 引用！");
                continue;
            }

            // 正确处理当前页并在进度内的元素
            if (pageElement.pageIndex == pageManager.currentPage && pageElement.entryIndex <= pageManager.currentProgress)
            {
                pageElement.element.SetActive(true);
                if (HasElementPlayed(pageElement.pageIndex, pageElement.entryIndex))
                {
                    SetAlphaAllChildren(pageElement.element, 1f); // 直接显示
                }
                else
                {
                    SetAlphaAllChildren(pageElement.element, 0f); // 准备淡入
                }
            }
            else
            {
                //隐藏其他不相关的元素
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
    /// 显示符合条件的页面元素（entryIndex <= 当前 ID）
    /// </summary>
    public void ShowBookPages()
    {
        var filteredElements = pageElements.FindAll(pe =>
            pe.pageIndex == pageManager.currentPage && pe.entryIndex <= pageManager.currentProgress);
        filteredElements.Sort((a, b) => a.entryIndex.CompareTo(b.entryIndex)); // 按顺序播放
        StartCoroutine(PlayFadeAnimations(filteredElements));
    }

    /// <summary>
    /// 播放淡入动画（处理子组件）
    /// </summary>
    private System.Collections.IEnumerator PlayFadeAnimations(List<PageElement> filteredElements)
    {
        foreach (var pageElement in filteredElements)
        {
            if (pageElement.element == null) continue;

            pageElement.element.SetActive(true); // 再次确保激活
            FadeInAllChildren(pageElement.element, pageElement.fadeDuration);
            // 在此记录为已播放
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
    /// 完整显示所有元素（立即完成）
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
    /// 显示上一页和下一页按钮
    /// </summary>
    private void ShowButtons()
    {
        if (pageManager == null) return;

        pageManager.previousButton?.gameObject.SetActive(true);
        pageManager.nextButton?.gameObject.SetActive(needShowNext);
    }

    /// <summary>
    /// 设置该对象及所有子物体上的 Image / TMP 文本透明度
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
    /// 统一淡入子组件
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
