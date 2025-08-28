using UnityEngine;
using UnityEngine.UI;

public class UIFollowRootCheck : MonoBehaviour
{
    [Tooltip("在 UI 离开 Root 区域后设置到该对象下（Tag=MaskRoot）")]
    public string canvasTag = "Canvas";
    public string rootTag = "Root";
    public string fallbackParentTag = "MaskRoot";

    private RectTransform selfRect;
    private RectTransform rootRect;

    private bool isUnderRoot = true;

    void Start()
    {
        selfRect = GetComponent<RectTransform>();

        // 设置初始父级为 Canvas
        GameObject canvasObj = GameObject.FindGameObjectWithTag(canvasTag);
        if (canvasObj != null)
        {
            transform.SetParent(canvasObj.transform, worldPositionStays: false);
        }
        else
        {
            Debug.LogWarning("找不到 tag 为 Canvas 的对象");
        }

        // 获取 Root 区域
        GameObject rootObj = GameObject.FindGameObjectWithTag(rootTag);
        if (rootObj != null)
        {
            rootRect = rootObj.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogWarning("找不到 tag 为 Root 的对象");
        }
    }

    void Update()
    {
        if (rootRect == null || selfRect == null) return;

        // 判断是否还在 Root 区域内
        if (!RectTransformUtility.RectangleContainsScreenPoint(rootRect, RectTransformUtility.WorldToScreenPoint(null, selfRect.position)))
        {
            if (isUnderRoot)
            {
                // 离开 Root 区域：切换父级到 MaskRoot
                GameObject fallbackParent = GameObject.FindGameObjectWithTag(fallbackParentTag);
                if (fallbackParent != null)
                {
                    transform.SetParent(fallbackParent.transform, worldPositionStays: false);
                    isUnderRoot = false;
                }
                else
                {
                    Debug.LogWarning("找不到 tag 为 MaskRoot 的对象");
                }
            }
        }
        else
        {
            isUnderRoot = true;
        }
    }
}
