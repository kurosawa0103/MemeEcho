using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject prefabToInstantiate; // 要实例化的预设对象
    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform validArea; // 合法的拖拽范围
    private Vector2 originalPosition;
    private Vector2 offset; // 鼠标点击位置与UI元素中心的偏移量
    private bool isDragging;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // 查找名字为“柜子”的物体，并获取其RectTransform
        GameObject validAreaObject = GameObject.Find("柜子");
        if (validAreaObject != null)
        {
            validArea = validAreaObject.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("没有找到名字为‘柜子’的物体！");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 保存初始位置
        originalPosition = rectTransform.anchoredPosition;
        isDragging = true;

        // 计算鼠标点击位置与UI元素中心的偏移量
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localMousePosition);
        offset = rectTransform.anchoredPosition - localMousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            // 使UI元素跟随鼠标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localMousePosition);
            rectTransform.anchoredPosition = localMousePosition + offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // 检查拖拽结束时是否离开指定范围
        if (prefabToInstantiate != null && !IsWithinValidArea(rectTransform.anchoredPosition))
        {
            GameObject parentCanvas = GameObject.FindWithTag("Canvas");
            if (parentCanvas != null)
            {
                Transform parentTransform = parentCanvas.transform;
                GameObject instantiatedObject = Instantiate(prefabToInstantiate, parentTransform);
                RectTransform instantiatedRectTransform = instantiatedObject.GetComponent<RectTransform>();

                if (instantiatedRectTransform != null)
                {
                    // 将实例化对象的位置设置为拖拽结束的位置
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localMousePosition);
                    instantiatedRectTransform.anchoredPosition = localMousePosition;
                }
            }
            else
            {
                Debug.LogError("没有找到标签为'Canvas'的物体！");
            }
        }

        // 将UI元素复位到原始位置
        rectTransform.anchoredPosition = originalPosition;
    }

    private bool IsWithinValidArea(Vector2 position)
    {
        if (validArea == null) return true;

        Vector3[] worldCorners = new Vector3[4];
        validArea.GetWorldCorners(worldCorners);

        Vector2 min = worldCorners[0]; // Bottom-left corner
        Vector2 max = worldCorners[2]; // Top-right corner

        return position.x >= min.x && position.x <= max.x && position.y >= min.y && position.y <= max.y;
    }
}
