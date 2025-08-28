using UnityEngine;

public class BrushSelector : MonoBehaviour
{
    [Header("��ˢ����")]
    public string brushTag = "Red";
    public Color displayColor = Color.red;
    public Sprite brushIcon;

    [Header("�Ӿ�����")]
    public GameObject selectedIndicator; // ѡ��ʱ��ʾ��ָʾ��

    private SpriteRenderer sr;
    private bool isSelected = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // ���û������ͼ�꣬ʹ����ʾ��ɫ
        if (brushIcon != null)
        {
            sr.sprite = brushIcon;
        }

        // ������ʾ��ɫ
        sr.color = displayColor;

        // ��ʼ��ѡ��ָʾ��
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        // ֪ͨBrushController�л���ˢ
        BrushController brushController = FindObjectOfType<BrushController>();
        if (brushController != null)
        {
            brushController.SelectBrush(brushTag, displayColor);
            SetSelected(true);
        }

        Debug.Log($"ѡ���˱�ˢ: {brushTag}");
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(selected);
        }

        // ������������Ӿ�Ч��
        if (sr != null)
        {
            sr.color = selected ? Color.white : displayColor;
        }
    }

    // ��̬��������ȡ������ѡ��
    public static void DeselectAll()
    {
        BrushSelector[] selectors = FindObjectsOfType<BrushSelector>();
        foreach (var selector in selectors)
        {
            selector.SetSelected(false);
        }
    }
}