using UnityEngine;
using System.Collections;

public class Leaf : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float moveSpeed = 3f;
    public float returnSpeed = 2f;
    public float swayIntensity = 0.1f;
    public float swaySpeed = 1f;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isBeingPushed = false;
    private bool isReturning = false;

    // ��Ȼ�ڶ�
    private float swayOffsetX;
    private float swayOffsetY;
    private Vector3 swayDirection;

    // �ƶ�Э��
    private Coroutine moveCoroutine;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        // ��¼ԭʼλ��
        originalPosition = transform.position;
        targetPosition = originalPosition;

        // ����ڶ�ƫ��
        swayOffsetX = Random.Range(0f, Mathf.PI * 2f);
        swayOffsetY = Random.Range(0f, Mathf.PI * 2f);
        swayDirection = Random.insideUnitCircle.normalized * swayIntensity;
    }

    void Update()
    {
        if (!isBeingPushed && !isReturning)
        {
            ApplyNaturalSway();
        }
    }

    /// <summary>
    /// �ƿ�Ҷ�ӵ�ָ��λ��
    /// </summary>
    /// <param name="pushTargetPosition">�ƿ���Ŀ��λ��</param>
    public void PushTo(Vector3 pushTargetPosition)
    {
        if (isBeingPushed && Vector3.Distance(targetPosition, pushTargetPosition) < 0.1f)
        {
            return; // ���Ŀ��λ��û�������仯�����ظ�ִ��
        }

        isBeingPushed = true;
        isReturning = false;
        targetPosition = pushTargetPosition;

        // ֹ֮ͣǰ���ƶ�Э��
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // ��ʼ�ƶ���Ŀ��λ��
        moveCoroutine = StartCoroutine(MoveToTarget(targetPosition, moveSpeed));
    }

    /// <summary>
    /// ��Ҷ�ӻص�ԭλ��
    /// </summary>
    public void ReturnToOriginal()
    {
        if (isReturning) return; // �Ѿ��ڷ�����

        isBeingPushed = false;
        isReturning = true;
        targetPosition = originalPosition;

        // ֹ֮ͣǰ���ƶ�Э��
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // ��ʼ����ԭλ��
        moveCoroutine = StartCoroutine(MoveToTarget(originalPosition, returnSpeed));
    }

    /// <summary>
    /// ƽ���ƶ���Ŀ��λ��
    /// </summary>
    IEnumerator MoveToTarget(Vector3 target, float speed)
    {
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, target);
        float duration = distance / speed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // ʹ�û����������ƶ�����Ȼ
            progress = Mathf.SmoothStep(0f, 1f, progress);
            transform.position = Vector3.Lerp(startPos, target, progress);

            yield return null;
        }

        transform.position = target;

        // �ƶ���ɣ�����״̬
        if (isReturning)
        {
            isReturning = false;
        }

        moveCoroutine = null;
    }

    /// <summary>
    /// Ӧ����Ȼ�ڶ�Ч��
    /// </summary>
    void ApplyNaturalSway()
    {
        float swayX = Mathf.Sin(Time.time * swaySpeed + swayOffsetX) * swayDirection.x;
        float swayY = Mathf.Cos(Time.time * swaySpeed * 0.7f + swayOffsetY) * swayDirection.y;

        Vector3 swayOffset = new Vector3(swayX, swayY, 0);
        Vector3 swayTarget = originalPosition + swayOffset;

        // ��΢����ڶ�λ���ƶ�
        transform.position = Vector3.Lerp(transform.position, swayTarget, Time.deltaTime * 2f);
    }

    /// <summary>
    /// ��ȡԭʼλ��
    /// </summary>
    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }

    /// <summary>
    /// ����Ƿ����ڱ��ƿ�
    /// </summary>
    public bool IsBeingPushed()
    {
        return isBeingPushed;
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            // ��ʾԭʼλ��
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(originalPosition, 0.1f);

            // ��ʾĿ��λ��
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition, 0.1f);

            // ����
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}