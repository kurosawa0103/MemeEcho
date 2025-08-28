using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject prefabToInstantiate; // Ҫʵ������Ԥ�����
    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform validArea; // �Ϸ�����ק��Χ
    private Vector2 originalPosition;
    private Vector2 offset; // �����λ����UIԪ�����ĵ�ƫ����
    private bool isDragging;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // ��������Ϊ�����ӡ������壬����ȡ��RectTransform
        GameObject validAreaObject = GameObject.Find("����");
        if (validAreaObject != null)
        {
            validArea = validAreaObject.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("û���ҵ�����Ϊ�����ӡ������壡");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // �����ʼλ��
        originalPosition = rectTransform.anchoredPosition;
        isDragging = true;

        // ���������λ����UIԪ�����ĵ�ƫ����
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localMousePosition);
        offset = rectTransform.anchoredPosition - localMousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            // ʹUIԪ�ظ������
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localMousePosition);
            rectTransform.anchoredPosition = localMousePosition + offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // �����ק����ʱ�Ƿ��뿪ָ����Χ
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
                    // ��ʵ���������λ������Ϊ��ק������λ��
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localMousePosition);
                    instantiatedRectTransform.anchoredPosition = localMousePosition;
                }
            }
            else
            {
                Debug.LogError("û���ҵ���ǩΪ'Canvas'�����壡");
            }
        }

        // ��UIԪ�ظ�λ��ԭʼλ��
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
