using UnityEngine;
using UnityEngine.UI;

public class MusicSearch : MonoBehaviour
{
    [Header("�ο�����")]
    public Transform player;
    public Transform targetObject;

    [Header("UIԪ��")]
    public Image rippleImage;
    public Slider frequencySlider;

    [Header("���ò���")]
    public float activationDistance = 5f;
    public float targetFrequency = 50f;
    public float tolerance = 2f;

    [Header("��ɫ")]
    public Color idleColor = Color.gray;       // δ����
    public Color activeColor = Color.yellow;   // ����Ŀ�굫δ��Ƶ
    public Color successColor = Color.green;   // ��Ƶ�ɹ�

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
