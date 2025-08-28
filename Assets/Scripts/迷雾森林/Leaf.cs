using UnityEngine;
using System.Collections;

public class Leaf : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 3f;
    public float returnSpeed = 2f;
    public float swayIntensity = 0.1f;
    public float swaySpeed = 1f;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isBeingPushed = false;
    private bool isReturning = false;

    // 自然摆动
    private float swayOffsetX;
    private float swayOffsetY;
    private Vector3 swayDirection;

    // 移动协程
    private Coroutine moveCoroutine;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        // 记录原始位置
        originalPosition = transform.position;
        targetPosition = originalPosition;

        // 随机摆动偏移
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
    /// 推开叶子到指定位置
    /// </summary>
    /// <param name="pushTargetPosition">推开的目标位置</param>
    public void PushTo(Vector3 pushTargetPosition)
    {
        if (isBeingPushed && Vector3.Distance(targetPosition, pushTargetPosition) < 0.1f)
        {
            return; // 如果目标位置没有显著变化，不重复执行
        }

        isBeingPushed = true;
        isReturning = false;
        targetPosition = pushTargetPosition;

        // 停止之前的移动协程
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // 开始移动到目标位置
        moveCoroutine = StartCoroutine(MoveToTarget(targetPosition, moveSpeed));
    }

    /// <summary>
    /// 让叶子回到原位置
    /// </summary>
    public void ReturnToOriginal()
    {
        if (isReturning) return; // 已经在返回中

        isBeingPushed = false;
        isReturning = true;
        targetPosition = originalPosition;

        // 停止之前的移动协程
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // 开始返回原位置
        moveCoroutine = StartCoroutine(MoveToTarget(originalPosition, returnSpeed));
    }

    /// <summary>
    /// 平滑移动到目标位置
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

            // 使用缓动曲线让移动更自然
            progress = Mathf.SmoothStep(0f, 1f, progress);
            transform.position = Vector3.Lerp(startPos, target, progress);

            yield return null;
        }

        transform.position = target;

        // 移动完成，更新状态
        if (isReturning)
        {
            isReturning = false;
        }

        moveCoroutine = null;
    }

    /// <summary>
    /// 应用自然摆动效果
    /// </summary>
    void ApplyNaturalSway()
    {
        float swayX = Mathf.Sin(Time.time * swaySpeed + swayOffsetX) * swayDirection.x;
        float swayY = Mathf.Cos(Time.time * swaySpeed * 0.7f + swayOffsetY) * swayDirection.y;

        Vector3 swayOffset = new Vector3(swayX, swayY, 0);
        Vector3 swayTarget = originalPosition + swayOffset;

        // 轻微地向摆动位置移动
        transform.position = Vector3.Lerp(transform.position, swayTarget, Time.deltaTime * 2f);
    }

    /// <summary>
    /// 获取原始位置
    /// </summary>
    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }

    /// <summary>
    /// 检查是否正在被推开
    /// </summary>
    public bool IsBeingPushed()
    {
        return isBeingPushed;
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            // 显示原始位置
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(originalPosition, 0.1f);

            // 显示目标位置
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition, 0.1f);

            // 连线
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}