using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target; // Ҫ�����Ŀ��
    public Vector3 offset;   // ƫ����

    void Update()
    {
        if (target != null)
        {
            // ��UIͼƬ��λ������ΪĿ�������λ�ü���ƫ����
            transform.position = target.position + offset;
        }
    }
}
