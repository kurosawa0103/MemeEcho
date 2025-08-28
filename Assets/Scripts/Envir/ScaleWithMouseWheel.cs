using UnityEngine;

public class ScaleWithMouseWheel : MonoBehaviour
{
    public Transform targetObject;   // 需要缩放的物体
    public float zoomSpeed = 0.1f;   // 每次缩放的速度（控制滚轮的缩放速率）
    public float lerpSpeed = 10f;    // 平滑过渡的速度（控制缩放平滑的插值速度）
    public float minScale = 0.5f;    // 最小缩放值
    public float maxScale = 3f;      // 最大缩放值

    public Vector3 targetScale;     // 目标缩放值

    void Start()
    {
        targetScale = targetObject.localScale; // 初始化为当前的缩放值
    }

    void Update()
    {
        // 1️⃣ 获取滚轮输入
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            // 2️⃣ 计算新的目标缩放值
            targetScale += Vector3.one * scrollInput * zoomSpeed;

            // 3️⃣ 限制缩放值在最大值和最小值之间
            targetScale = new Vector3(
                Mathf.Clamp(targetScale.x, minScale, maxScale),
                Mathf.Clamp(targetScale.y, minScale, maxScale),
                Mathf.Clamp(targetScale.z, minScale, maxScale)
            );
        }

        // 4️⃣ 平滑插值，平滑过渡到目标缩放值
        targetObject.localScale = Vector3.Lerp(targetObject.localScale, targetScale, Time.deltaTime * lerpSpeed);
    }
}
