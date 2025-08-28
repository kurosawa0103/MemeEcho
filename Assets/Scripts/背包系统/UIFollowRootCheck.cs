using UnityEngine;
using UnityEngine.UI;

public class UIFollowRootCheck : MonoBehaviour
{
    [Tooltip("�� UI �뿪 Root ��������õ��ö����£�Tag=MaskRoot��")]
    public string canvasTag = "Canvas";
    public string rootTag = "Root";
    public string fallbackParentTag = "MaskRoot";

    private RectTransform selfRect;
    private RectTransform rootRect;

    private bool isUnderRoot = true;

    void Start()
    {
        selfRect = GetComponent<RectTransform>();

        // ���ó�ʼ����Ϊ Canvas
        GameObject canvasObj = GameObject.FindGameObjectWithTag(canvasTag);
        if (canvasObj != null)
        {
            transform.SetParent(canvasObj.transform, worldPositionStays: false);
        }
        else
        {
            Debug.LogWarning("�Ҳ��� tag Ϊ Canvas �Ķ���");
        }

        // ��ȡ Root ����
        GameObject rootObj = GameObject.FindGameObjectWithTag(rootTag);
        if (rootObj != null)
        {
            rootRect = rootObj.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogWarning("�Ҳ��� tag Ϊ Root �Ķ���");
        }
    }

    void Update()
    {
        if (rootRect == null || selfRect == null) return;

        // �ж��Ƿ��� Root ������
        if (!RectTransformUtility.RectangleContainsScreenPoint(rootRect, RectTransformUtility.WorldToScreenPoint(null, selfRect.position)))
        {
            if (isUnderRoot)
            {
                // �뿪 Root �����л������� MaskRoot
                GameObject fallbackParent = GameObject.FindGameObjectWithTag(fallbackParentTag);
                if (fallbackParent != null)
                {
                    transform.SetParent(fallbackParent.transform, worldPositionStays: false);
                    isUnderRoot = false;
                }
                else
                {
                    Debug.LogWarning("�Ҳ��� tag Ϊ MaskRoot �Ķ���");
                }
            }
        }
        else
        {
            isUnderRoot = true;
        }
    }
}
