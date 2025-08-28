using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    Transform[] childs;
    public float smoothFactor = 0.1f; // 控制旋转插值的平滑度，越接近 1 越接近直接转向

    void Start()
    {
        // 获取所有子物体的 Transform
        childs = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            childs[i] = transform.GetChild(i);
        }

        // 在初始时，立即将所有子物体的旋转对准相机
        Quaternion initialRotation = Camera.main.transform.rotation;
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].rotation = initialRotation;
        }
    }

    void LateUpdate()
    {
        // 获取当前主相机的旋转
        Quaternion targetRotation = Camera.main.transform.rotation;

        // 遍历所有子物体并使其逐渐逼近相机的旋转
        for (int i = 0; i < childs.Length; i++)
        {
            // 使用 Slerp 平滑旋转到相机方向，插值比例为 smoothFactor
            childs[i].rotation = Quaternion.Slerp(childs[i].rotation, targetRotation, smoothFactor);
        }
    }
}
