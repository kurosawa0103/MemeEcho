using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������������ϣ��� Update �г������� targetRect Χ����Ļ���ĵ��ۻ��Ƕȣ�˳ʱ��Ϊ����
/// </summary>
public class UIOrbitAngleTracker : MonoBehaviour
{
    [Header("���� ���� ����ק�� UI ����")]
    [SerializeField] private RectTransform targetRect;

    [Header("��ѡ ���� ��� Canvas ���� Screen Space - Overlay������ Canvas")]
    [SerializeField] private Canvas targetCanvas;

    [Header("��ѡ ���� �����ڽ�������ʾ��ֵ")]
    [SerializeField] private Text debugText;

    /// <summary>˳ʱ���ۻ��Ƕȣ���λ���㣩</summary>
    public float CumulativeCWAngle { get; private set; }

    Vector2 screenCenter;      // ��Ļ���ģ��������꣩
    float lastAngleCCW;        // ��һ֡�� CCW����ʱ�룩�Ƕ�
    bool hasLastAngle;         // �Ƿ��Ѿ���¼����һ֡

    Camera canvasCam;          // �� Overlay Canvas ʱ��Ҫ�ṩ���
    /// <summary>���ٶ��¼������� = ˳ʱ�������Ƕȣ��㣩���� = ǰ������ = ����</summary>
    public event System.Action<float> OnAngleDeltaCW;
    void Start()
    {
        screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        if (targetCanvas && targetCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            canvasCam = targetCanvas.worldCamera;
        else
            canvasCam = null;  // Overlay ģʽ����Ҫ���
    }

    void Update()
    {
        if (!targetRect) return;

        // �� UI ��������ת������Ļ����
        Vector2 currentScreenPos = RectTransformUtility.WorldToScreenPoint(canvasCam, targetRect.position);
        Vector2 dirFromCenter = currentScreenPos - screenCenter;

        // ���պ����������޷�����Ƕ�
        if (dirFromCenter.sqrMagnitude < 1e-6f) return;

        // �� +X ��Ϊ 0�㣬��ʱ��Ϊ���õ���ǰ˲ʱ��
        float angleCCW = Mathf.Atan2(dirFromCenter.y, dirFromCenter.x) * Mathf.Rad2Deg;

        if (!hasLastAngle)
        {
            lastAngleCCW = angleCCW;
            hasLastAngle = true;
            return; // ��Ҫ��ǰһ֡����������
        }

        // �� = ��ǰ - ��һ֡��ȡ���·������Χ [-180, 180]��
        float deltaCCW = Mathf.DeltaAngle(lastAngleCCW, angleCCW);
        lastAngleCCW = angleCCW;

        // ��ĿҪ��˳ʱ��Ϊ�������԰���ʱ����ֵ��Ϊ����˳ʱ�븺ֵ��Ϊ��
        CumulativeCWAngle -= deltaCCW;
        float deltaCW = -deltaCCW;        // תΪ˳ʱ�������
        CumulativeCWAngle += deltaCW;

        // �㲥���ⲿ
        OnAngleDeltaCW?.Invoke(deltaCW);

        // ���� Debug ��� ���� //
        Debug.Log($"CW Sum: {CumulativeCWAngle:F2}��   ��Frame: {-deltaCCW:F2}��");

        if (debugText) debugText.text = $"{CumulativeCWAngle:F1}��";
    }

    /// <summary>������ק��ʼʱ���ã����������ۼ�ֵ</summary>
    public void ResetAngle()
    {
        CumulativeCWAngle = 0;
        hasLastAngle = false;
    }
}
