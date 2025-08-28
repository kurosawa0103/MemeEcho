using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SpeedChanger : MonoBehaviour
{
    public float timeScale=1;//时间倍率

    public Button speedButton;
    private int currentSpeedIndex = 0;
    private float[] speeds = { 1f, 2f, 5f};
    private string[] speedTexts = { "速度 x 1 ", "速度 x 2\n(不推荐!)", "速度 x 5\n(不推荐!!!)"};

    void Start()
    {
        speedButton.onClick.AddListener(OnSpeedButtonClick);
        UpdateButton();
    }

    private void OnSpeedButtonClick()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % speeds.Length;
        ApplySpeedChange();
        UpdateButton();
    }

    private void ApplySpeedChange()
    {
        switch (currentSpeedIndex)
        {
            case 0:
                Method1();
                break;
            case 1:
                Method2();
                break;
            case 2:
                Method3();
                break;
        }
    }

    private void UpdateButton()
    {
        speedButton.GetComponentInChildren<TextMeshProUGUI>().text = speedTexts[currentSpeedIndex];
    }

    private void Method1()
    {
        Debug.Log("1倍速启用");
        Time.timeScale = 1;
    }

    private void Method2()
    {
        Debug.Log("2倍速启用");
        Time.timeScale = 2;
    }

    private void Method3()
    {
        Debug.Log("5倍速启用");
        Time.timeScale = 5;
    }
    private void Method4()
    {
        Debug.Log("20倍速启用");
        Time.timeScale = 20;
    }
}
