using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TipPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tipPanel; // ��ʾ����
    public TextMeshProUGUI tooltipText; // ��ʾ�����ı�
    [TextArea(3, 8)]
    public string tipText;
    private void Start()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ��������ڰ�ť��ʱ��ʾ������ʾ
        if (tipPanel != null)
        {
            tooltipText.text = tipText;
            tipPanel.SetActive(true);
            // ��ȡ��Ʒ��RectTransform
            RectTransform itemRectTransform = GetComponent<RectTransform>();

            // ������ʾ���ڵ�λ�ã��������½�
            Vector3 tooltipPosition = itemRectTransform.position;
            tooltipPosition.x += itemRectTransform.rect.width / 2 + tipPanel.GetComponent<RectTransform>().rect.width / 2;
            tooltipPosition.y -= itemRectTransform.rect.height / 2 + tipPanel.GetComponent<RectTransform>().rect.height / 2;

            // ����ʾ���ڵ�λ�����õ��������½�
            tipPanel.transform.position = tooltipPosition;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ����뿪��ťʱ���ع�����ʾ
        if (tipPanel != null)
        {
            tipPanel.SetActive(false);
        }
    }
}
