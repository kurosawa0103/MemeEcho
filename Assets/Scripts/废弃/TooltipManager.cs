using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipPanel; // ��ʾ����
    public TextMeshProUGUI tooltipText; // ��ʾ�����ı�
    public TextMeshProUGUI toolNameText; // ��ʾ�����ı�
    public Image toolItemImage;
    private void Start()
    {
        // ȷ��������ʾһ��ʼ�����ص�
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ��������ڰ�ť��ʱ��ʾ������ʾ
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ����뿪��ťʱ���ع�����ʾ
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}
