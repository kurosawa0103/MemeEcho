using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainZDistance : MonoBehaviour
{
    public float zOffset = -5f; // ��������Z��ƫ�ƣ����� -5f ��ʾ�����ǰ�� 5 ��λ��

    void LateUpdate()
    {
        if (Camera.main == null) return;

        Transform cam = Camera.main.transform;
        Vector3 targetPos = cam.position + cam.forward * Mathf.Abs(zOffset);

        transform.position = new Vector3(transform.position.x, transform.position.y, targetPos.z);
    }
}
