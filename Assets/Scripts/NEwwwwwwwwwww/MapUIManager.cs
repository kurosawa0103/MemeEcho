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

        foreach (char c in fullText)
        {
            mapDescriptionText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        // ���ֻ���������ʾ��ť�Ͱ�ť����
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
