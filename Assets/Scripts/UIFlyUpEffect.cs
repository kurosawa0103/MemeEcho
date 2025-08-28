using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFlyUpEffect : MonoBehaviour
{
    public RectTransform vanishingPointUI;
    public float flySpeed = 500f;
    public float growSpeed = 1.5f;
    public float lifeTime = 3f;

    [Tooltip("控制方向抖动，0表示直线，180表示全向扩散")]
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
            Debug.LogWarning("未设置消失点，默认使用屏幕中心");
            var canvas = GetComponentInParent<Canvas>();
            vanishingPointUI = canvas.GetComponent<RectTransform>();
        }

        Vector2 vanishPoint = vanishingPointUI.anchoredPosition;

        // 基础方向
        direction = (rectTransform.anchoredPosition - vanishPoint).normalized;

        // 方向加扩散扰动
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

        // 远离中心
        rectTransform.anchoredPosition += direction * flySpeed * Time.deltaTime;

        // 放大
        float scale = 1 + growSpeed * timer;
        rectTransform.localScale = Vector3.one * scale;

        // 渐隐（可选）
        var cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = Mathf.Clamp01(1f - timer / lifeTime);
        }
    }
}