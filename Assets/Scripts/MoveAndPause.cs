using UnityEngine;
using System.Collections;

public class MoveAndPause : MonoBehaviour
{
    [Header("Ŀ��λ��")]
    public Vector3 positionA;
    public Vector3 positionB;

    [Header("�ƶ��ٶ�")]
    public float speedToA = 2f;
    public float speedToB = 3f;

    [Header("ͣ��ʱ�䣨�룩")]
    public float waitDuration = 2f;

    private bool hasStarted = false;

    void Start()
    {
        StartCoroutine(MoveSequence());
    }

    IEnumerator MoveSequence()
    {
        if (hasStarted) yield break;
        hasStarted = true;

        // �ƶ���λ��A
        yield return StartCoroutine(MoveToPosition(positionA, speedToA));

        // ͣ��
        yield return new WaitForSeconds(waitDuration);

        // �ƶ���λ��B
        yield return StartCoroutine(MoveToPosition(positionB, speedToB));
    }

    IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition; // ��׼����λ��
    }
}