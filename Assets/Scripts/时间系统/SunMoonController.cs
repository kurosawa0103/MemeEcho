using UnityEngine;

public class SunMoonController : MonoBehaviour
{
    [Header("太阳和月亮对象")]
    public Transform sun;
    public Transform moon;

    [Header("旋转中心点")]
    public Transform centerPoint;

    [Header("旋转半径")]
    public float radius = 5f;

    [Header("时间系统引用")]
    public TimeSystem timeSystem;

    void Update()
    {
        if (timeSystem == null) return;

        float dayLength = timeSystem.dayLength;
        float duskLength = timeSystem.duskLength;
        float nightLength = timeSystem.nightLength;
        float totalDuration = dayLength + duskLength + nightLength;

        float time = timeSystem.timeCount;
        Vector3 center = centerPoint.position;

        float angle = 0f;

        if (time < (dayLength + duskLength))
        {
            // 白天 + 黄昏时间段：太阳从 0° -> 180°
            float progress = time / (dayLength + duskLength);
            angle = Mathf.Lerp(0f, 180f, progress);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 sunPos = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
            sun.position = sunPos;
            sun.gameObject.SetActive(true);
            moon.gameObject.SetActive(false);
        }
        else
        {
            // 夜晚：月亮从 180° -> 360°
            float nightTime = time - (dayLength + duskLength);
            float progress = nightTime / nightLength;
            angle = Mathf.Lerp(0f, 180f, progress);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 moonPos = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
            moon.position = moonPos;
            sun.gameObject.SetActive(false);
            moon.gameObject.SetActive(true);
        }
    }
}
