using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowDebug : MonoBehaviour
{
    public TextMeshProUGUI state;
    public TextMeshProUGUI eventText;
    public TextMeshProUGUI timerText;  // 显示计时器的 TextMeshProUGUI
    public TextMeshProUGUI sailText;  // 显示计时器的 TextMeshProUGUI
    public EventManager eventManager;
    public string stateInfo;

    private float elapsedTime = 0f;  // 计时器的时间变量
    public Sailor sailor;
    void Start()
    {

    }

    void Update()
    {
        state.text = stateInfo;

        if (eventManager.currentEvent != null)
        {
            eventText.text = eventManager.currentEvent.name.ToString();
        }

        // 更新计时器并显示为分钟格式（秒数取整）
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);  // 取整的秒数
                                                            // 显示 Sailor 的航行距离
        if (sailor != null)
        {
            //sailText.text = $"距离: {sailor.sailDistance:F1} 米";  // 获取航行距离
        }

        timerText.text = $"{minutes}分钟{seconds}秒";
    }

}
