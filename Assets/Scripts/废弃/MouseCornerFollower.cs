using UnityEngine;
using UnityEngine.UI;

public class MouseCornerFollower : MonoBehaviour
{
    // 乘数，用于放大移动的影响
    public float multiplier = 0.1f;

    // 记录初始位置
    private Vector3 initialPosition;

    // 配置移动方向
    public Vector2 moveDirection = new Vector2(1, 1);

    void Start()
    {
        // 记录初始位置
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // 获取当前帧的鼠标位置
        Vector3 currentMousePosition = Input.mousePosition;

        // 计算UI图片的偏移量
        Vector3 offset = currentMousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

        // 限制偏移量的幅度
        offset = Vector3.ClampMagnitude(offset, 100f);

        // 根据配置的移动方向调整偏移量
        offset.x *= moveDirection.x;
        offset.y *= moveDirection.y;

        // 根据鼠标移动的距离计算UI图片的偏移量
        Vector3 finalOffset = offset * multiplier;

        // 加上初始位置
        finalOffset += initialPosition;

        // 应用偏移量到UI图片位置
        transform.localPosition = finalOffset;
    }
}
