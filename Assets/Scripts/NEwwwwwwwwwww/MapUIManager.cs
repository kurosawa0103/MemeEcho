using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class MapUIManager : MonoBehaviour
{

    public MovementSystem playerMovement;
    public TMP_Text mapNameText;
    public TMP_Text mapDescriptionText;

    public GameObject buttonParent;
    public TMP_Text buttonText;
    public Button button; // ��ť���

    public MapInfo defaultMapInfo;

    public MapInfo currentMapInfo;



    public float typewriterSpeed = 0.05f; // ���ֻ��ٶȣ�ÿ�ּ������
    private bool skipTypewriter = false; // �Ƿ��������ֻ�
    private bool isTyping = false;
    private float currentSpeed;
    private void Awake()
    {
        ShowMapInfo(defaultMapInfo);

        // ��ť�󶨵���¼�
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
        }
    
    }
    private void Update()
    {
        if (isTyping && Input.GetMouseButtonDown(0))
        {
            // ������٣��ٶȷ������Զ��屶��
            currentSpeed = typewriterSpeed * 0.1f; // ����ԭ�ٶ�10��
        }
    }
    public void ShowMapInfo(MapInfo mapInfo)
    {
        if (mapInfo == null)
        {
            mapInfo = defaultMapInfo;
        }

        currentMapInfo = mapInfo;

        if (mapNameText) mapNameText.text = mapInfo.mapName;

        // ��ť�����أ������ı���գ��ȴ����ֻ����
        if (buttonParent)
            buttonParent.SetActive(false);
        if (mapDescriptionText)
            mapDescriptionText.text = "";

        StopAllCoroutines();
        StartCoroutine(TypewriterEffect(mapInfo.mapDescription));
    }

    private IEnumerator TypewriterEffect(string fullText)
    {
        if (mapDescriptionText == null)
            yield break;

        mapDescriptionText.text = "";
        isTyping = true;
        currentSpeed = typewriterSpeed;

        foreach (char c in fullText)
        {
            mapDescriptionText.text += c;
            yield return new WaitForSeconds(currentSpeed);
        }

        isTyping = false;

        // ��ʾ��ť������
        if (currentMapInfo != null && buttonParent != null)
        {
            if (string.IsNullOrWhiteSpace(currentMapInfo.buttonText))
            {
                buttonParent.SetActive(false);
            }
            else
            {
                buttonParent.SetActive(true);
                if (buttonText) buttonText.text = currentMapInfo.buttonText;
            }
        }
    }

    private void OnButtonClick()
    {
        // ��ֹ����ƶ�
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

    }

    public void ResetToDefault()
    {
        ShowMapInfo(defaultMapInfo);
    }
}
