using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainZDistance : MonoBehaviour
{
    public float zOffset = -5f; // 相对相机的Z轴偏移（例如 -5f 表示在相机前方 5 单位）

    void LateUpdate()
    {
        if (Camera.main == null) return;

        Transform cam = Camera.main.transform;
        Vector3 targetPos = cam.position + cam.forward * Mathf.Abs(zOffset);

        transform.position = new Vector3(transform.position.x, transform.position.y, targetPos.z);
    }
}
