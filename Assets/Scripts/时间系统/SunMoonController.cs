using UnityEngine;

public class SunMoonController : MonoBehaviour
{
    [Header("̫������������")]
    public Transform sun;
    public Transform moon;

    [Header("��ת���ĵ�")]
    public Transform centerPoint;

    [Header("��ת�뾶")]
    public float radius = 5f;

    [Header("ʱ��ϵͳ����")]
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
            // ���� + �ƻ�ʱ��Σ�̫���� 0�� -> 180��
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
            // ҹ�������� 180�� -> 360��
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
