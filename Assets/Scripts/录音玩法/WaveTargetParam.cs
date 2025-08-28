using UnityEngine;

public class WaveTargetParam : MonoBehaviour
{
    [Header("����Ŀ��ֵA")]
    [Range(0f, 1f)]
    public float targetSliderValueA = 0.5f;

    [Header("��Ļ�ж�")]
    public float maxFadeDistance = 5f;      // ��Զ����ʱ��Ļȫ��
    public float fullClearDistance = 1f;    // �����㹻��ʱ��Ļȫ͸��

    [Header("��ɳɹ�ʱ�ϱ����� Fungus")]
    public string successKey = "WavePointClear";

    [Header("Ŀ�����ӻ������ԣ�")]
    public Color gizmoColor = Color.green;
    public float gizmoRadius = 0.2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}
