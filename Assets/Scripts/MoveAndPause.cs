using UnityEngine;
using System.Collections;

public class MoveAndPause : MonoBehaviour
{
    [Header("目标位置")]
    public Vector3 positionA;
    public Vector3 positionB;

    [Header("移动速度")]
    public float speedToA = 2f;
    public float speedToB = 3f;

    [Header("停留时间（秒）")]
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

        // 移动到位置A
        yield return StartCoroutine(MoveToPosition(positionA, speedToA));

        // 停留
        yield return new WaitForSeconds(waitDuration);

        // 移动到位置B
        yield return StartCoroutine(MoveToPosition(positionB, speedToB));
    }

    IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition; // 精准对齐位置
    }
}