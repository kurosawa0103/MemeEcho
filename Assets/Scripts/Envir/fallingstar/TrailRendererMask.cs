using UnityEngine;

public class TrailRendererMask : MonoBehaviour
{
    public TrailRenderer trailRenderer;  // TrailRenderer ���
    public Vector3 minBounds;           // ��Χ����С�߽�
    public Vector3 maxBounds;           // ��Χ�����߽�

    // �ڱ༭���л������ɷ�Χ�Ŀ��ӻ�����
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue; // ������ɫΪ��ɫ

        // ���㷶Χ�����ĺʹ�С
        Vector3 center = (minBounds + maxBounds) / 2;
        Vector3 size = maxBounds - minBounds;

        // ����һ���߿������壬��ʾ��Χ
        Gizmos.DrawWireCube(center, size);
    }

    void Update()
    {
        if (trailRenderer != null)
        {
            // ��ȡ TrailRenderer ��ǰ���еĹ켣��
            Vector3[] trailPositions = new Vector3[trailRenderer.positionCount];
            trailRenderer.GetPositions(trailPositions);

            for (int i = 0; i < trailPositions.Length; i++)
            {
                if (IsPointOutsideBounds(trailPositions[i]))
                {
                    // ����㳬����Χ������õ�
                    trailPositions[i] = Vector3.zero; // ����Խ�������Ϊ��Чλ�ã�����ֱ��ɾ��
                }
            }

            // ���� TrailRenderer �ĵ�
            trailRenderer.SetPositions(trailPositions);
        }
    }

    // �����Ƿ���ָ����Χ��
    bool IsPointOutsideBounds(Vector3 point)
    {
        return point.x < minBounds.x || point.x > maxBounds.x ||
               point.y < minBounds.y || point.y > maxBounds.y ||
               point.z < minBounds.z || point.z > maxBounds.z;
    }
}
