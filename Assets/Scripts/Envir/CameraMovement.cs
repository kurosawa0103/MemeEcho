using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target; // 旋转和跟随的目标
    public float xSpeed = 200; // 控制y轴上的旋转速度
    public float ySpeed = 200; // 控制x轴上的旋转速度
    public float mSpeed = 10; // 鼠标滚轮缩放速度
    public float yMinLimit = -50; // y轴最小限制
    public float yMaxLimit = 50; // y轴最大限制
    public float xMinLimit = -50; // x轴最小限制
    public float xMaxLimit = 50; // x轴最大限制
    public float distance = 2; // 当前距离
    public float minDistance = 2; // 最小距离
    public float maxDistance = 30; // 最大距离

    public bool needDamping = true; // 是否需要阻尼
    public float damping = 5.0f; // 阻尼速度

    public float x = 0.0f; // x轴旋转角度
    public float y = 0.0f; // y轴旋转角度

    private Vector3 currentVelocity; // 用于 SmoothDamp 平滑移动的速度变量

    // 定义相机移动范围的中心点和大小
    public Vector3 center = Vector3.zero; // 边界框的中心点
    public Vector3 size = new Vector3(20, 10, 20); // 边界框的尺寸，XYZ方向的大小

    public bool isCameraLocked = false; // 相机锁定开关，启用时禁止相机任何移动、旋转和缩放操作
    private Coroutine moveCoroutine; // 用来保存当前的协程
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    // 设置相机初始位置和旋转
    public void SetCameraAwake()
    {
        Quaternion rotation = Quaternion.Euler(yMinLimit, 0, 0.0f);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 initialPosition = rotation * negDistance + target.position;

        initialPosition = ClampPositionWithinBounds(initialPosition);

        transform.position = initialPosition;
        transform.rotation = rotation;
    }

    void FixedUpdate()
    {
        SetCameraMove();
    }

    public void SetCameraMove()
    {
        if (isCameraLocked) return;
        if (target)
        {
            if (Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);
                x = ClampAngle(x, xMinLimit, xMaxLimit);
            }

            distance -= Input.GetAxis("Mouse ScrollWheel") * mSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 targetPosition = rotation * negDistance + target.position;

            targetPosition = ClampPositionWithinBounds(targetPosition);

            if (needDamping)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * damping);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, Time.deltaTime * damping);
            }
            else
            {
                transform.rotation = rotation;
                transform.position = targetPosition;
            }
        }
    }

    private Vector3 ClampPositionWithinBounds(Vector3 targetPosition)
    {
        Vector3 minBounds = center - size / 2;
        Vector3 maxBounds = center + size / 2;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.z, maxBounds.z);

        return targetPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    // 新的 SetNewTarget 方法，用于在指定时间内移动到目标
    public void SetNewTarget(Transform newTarget, float duration, System.Action onComplete = null)
    {
        if (isCameraLocked) return;

        if (newTarget != null)
        {
            // 如果有正在进行的协程，先停止它
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            target = newTarget;
            moveCoroutine = StartCoroutine(MoveCamera(duration, onComplete)); // 启动新的平滑过渡
        }
    }

    private IEnumerator MoveCamera(float duration, System.Action onComplete)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 targetPosition = rotation * negDistance + target.position;

        targetPosition = ClampPositionWithinBounds(targetPosition);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Lerp(startRotation, rotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = rotation;

        onComplete?.Invoke(); // 移动完成时调用回调
    }

}
