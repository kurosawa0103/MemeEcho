using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SpeedChanger : MonoBehaviour
{
    public float timeScale=1;//ʱ�䱶��

    public Button speedButton;
    private int currentSpeedIndex = 0;
    private float[] speeds = { 1f, 2f, 5f};
    private string[] speedTexts = { "�ٶ� x 1 ", "�ٶ� x 2\n(���Ƽ�!)", "�ٶ� x 5\n(���Ƽ�!!!)"};

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
        Debug.Log("1��������");
        Time.timeScale = 1;
    }

    private void Method2()
    {
        Debug.Log("2��������");
        Time.timeScale = 2;
    }

    private void Method3()
    {
        Debug.Log("5��������");
        Time.timeScale = 5;
    }
    private void Method4()
    {
        Debug.Log("20��������");
        Time.timeScale = 20;
    }
}
