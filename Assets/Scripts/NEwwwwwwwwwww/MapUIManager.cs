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
    public Button button; // 按钮组件

    public MapInfo defaultMapInfo;

    public MapInfo currentMapInfo;



    public float typewriterSpeed = 0.05f; // 打字机速度，每字间隔秒数
    private bool skipTypewriter = false; // 是否跳过打字机
    private bool isTyping = false;
    private float currentSpeed;
    private void Awake()
    {
        ShowMapInfo(defaultMapInfo);

        // 按钮绑定点击事件
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
            // 点击加速，速度翻倍或自定义倍数
            currentSpeed = typewriterSpeed * 0.1f; // 例如原速度10倍
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

        // 按钮先隐藏，描述文本清空，等待打字机完成
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

        // 显示按钮和文字
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
        // 禁止玩家移动
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
