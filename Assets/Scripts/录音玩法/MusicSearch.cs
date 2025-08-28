using UnityEngine;
using UnityEngine.UI;

public class MusicSearch : MonoBehaviour
{
    [Header("参考对象")]
    public Transform player;
    public Transform targetObject;

    [Header("UI元素")]
    public Image rippleImage;
    public Slider frequencySlider;

    [Header("配置参数")]
    public float activationDistance = 5f;
    public float targetFrequency = 50f;
    public float tolerance = 2f;

    [Header("颜色")]
    public Color idleColor = Color.gray;       // 未激活
    public Color activeColor = Color.yellow;   // 靠近目标但未对频
    public Color successColor = Color.green;   // 对频成功

    private bool isNear = false;
    private bool isSuccess = false;

    void Update()
    {
        float dist = Vector3.Distance(player.position, targetObject.position);
        isNear = dist < activationDistance;

        if (!isNear)
        {
            rippleImage.color = idleColor;
            return;
        }

        float currentFreq = frequencySlider.value;
        float delta = Mathf.Abs(currentFreq - targetFrequency);

        if (delta <= tolerance)
        {
            isSuccess = true;
            rippleImage.color = successColor;
        }
        else
        {
            isSuccess = false;
            rippleImage.color = activeColor;
        }
    }
}
