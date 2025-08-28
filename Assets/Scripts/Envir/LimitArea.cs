using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitArea : MonoBehaviour
{
    public Vector2 center = new Vector2(0, 0);  // ���ĵ�
    public Vector2 size = new Vector2(10, 5);   // ��߳ߴ磨X �� Y��
    public Vector2 minSize = new Vector2(10, 5);   // ��߳ߴ磨X �� Y��
    public Vector2 maxSize = new Vector2(10, 5);   // ��߳ߴ磨X �� Y��
    private void OnDrawGizmos()
    {
        // ���� Gizmo ����ɫ
        Gizmos.color = Color.green;

        // ������εķ�Χ�������ĵ�Ϳ��Ϊ������
        Rect boundary = new Rect(center - size / 2, size);

        // ���ƾ��α߽�
        Gizmos.DrawWireCube(boundary.center, boundary.size);
    }
}
