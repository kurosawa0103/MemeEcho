using UnityEngine;

public class CameraZoomWithObjectScale : MonoBehaviour
{
    public Transform targetObject;   // 需要同步缩放的物体
    public Camera targetCamera;      // 目标摄像机
    public float zoomSpeed = 5f;     // 视角缩放速度
    public float lerpSpeed = 10f;    // 平滑过渡速度

    public float minZ = -25f;        // 摄像机最靠近的Z轴位置
    public float maxZ = -13f;        // 摄像机最远的Z轴位置

    //public float minScale = 1.2f;    // 物体的最小缩放值
    //public float maxScale = 0.8f;    // 物体的最大缩放值

    public float targetZ;           // 目标Z轴位置
    private Vector3 targetScale;     // 目标缩放值
    private float currentZVelocity = 0f; // 平滑速度缓存

    void Start()
    {
        targetZ = targetCamera.transform.position.z;  // 初始化目标Z值
        //targetScale = targetObject.localScale;        // 初始化目标缩放
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            targetZ += scrollInput * zoomSpeed;
            targetZ = Mathf.Clamp(targetZ, minZ, maxZ);
        }

        // 使用 SmoothDamp 平滑过渡 Z 值
        float currentZ = targetCamera.transform.position.z;
        float newZ = Mathf.SmoothDamp(currentZ, targetZ, ref currentZVelocity, 1f / lerpSpeed);

        targetCamera.transform.position = new Vector3(
            targetCamera.transform.position.x,
            targetCamera.transform.position.y,
            newZ
        );
    }
}
