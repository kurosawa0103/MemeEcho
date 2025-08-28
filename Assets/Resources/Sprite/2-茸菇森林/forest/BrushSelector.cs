using UnityEngine;

public class BrushSelector : MonoBehaviour
{
    [Header("笔刷设置")]
    public string brushTag = "Red";
    public Color displayColor = Color.red;
    public Sprite brushIcon;

    [Header("视觉反馈")]
    public GameObject selectedIndicator; // 选中时显示的指示器

    private SpriteRenderer sr;
    private bool isSelected = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // 如果没有设置图标，使用显示颜色
        if (brushIcon != null)
        {
            sr.sprite = brushIcon;
        }

        // 设置显示颜色
        sr.color = displayColor;

        // 初始化选中指示器
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        // 通知BrushController切换笔刷
        BrushController brushController = FindObjectOfType<BrushController>();
        if (brushController != null)
        {
            brushController.SelectBrush(brushTag, displayColor);
            SetSelected(true);
        }

        Debug.Log($"选择了笔刷: {brushTag}");
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(selected);
        }

        // 可以添加其他视觉效果
        if (sr != null)
        {
            sr.color = selected ? Color.white : displayColor;
        }
    }

    // 静态方法用于取消所有选择
    public static void DeselectAll()
    {
        BrushSelector[] selectors = FindObjectsOfType<BrushSelector>();
        foreach (var selector in selectors)
        {
            selector.SetSelected(false);
        }
    }
}