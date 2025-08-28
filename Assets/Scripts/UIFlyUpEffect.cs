using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFlyUpEffect : MonoBehaviour
{
    public RectTransform vanishingPointUI;
    public float flySpeed = 500f;
    public float growSpeed = 1.5f;
    public float lifeTime = 3f;

    [Tooltip("���Ʒ��򶶶���0��ʾֱ�ߣ�180��ʾȫ����ɢ")]
    [Range(0, 180)]
    public float spreadAngle = 30f;

    private RectTransform rectTransform;
    private Vector2 direction;
    private float timer = 0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (vanishingPointUI == null)
        {
            Debug.LogWarning("δ������ʧ�㣬Ĭ��ʹ����Ļ����");
            var canvas = GetComponentInParent<Canvas>();
            vanishingPointUI = canvas.GetComponent<RectTransform>();
        }

        Vector2 vanishPoint = vanishingPointUI.anchoredPosition;

        // ��������
        direction = (rectTransform.anchoredPosition - vanishPoint).normalized;

        // �������ɢ�Ŷ�
        float angleOffset = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
        direction = Quaternion.Euler(0, 0, angleOffset) * direction;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        // Զ������
        rectTransform.anchoredPosition += direction * flySpeed * Time.deltaTime;

        // �Ŵ�
        float scale = 1 + growSpeed * timer;
        rectTransform.localScale = Vector3.one * scale;

        // ��������ѡ��
        var cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = Mathf.Clamp01(1f - timer / lifeTime);
        }
    }
}