using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RandomMoveInUI : MonoBehaviour
{
    public float moveSpeed = 200f;
    public float bounceAngleVariance = 30f;

    public bool enableRotation = false;
    public float rotationSpeed = 30f;
    public float oscillationAmplitude = 20f;
    public float oscillationFrequency = 10f;

    private float noiseOffset;
    [Tooltip("可选：指定要旋转的目标（如图标、子物体）。为空时旋转自身。")]
    public Transform rotationTarget;

    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;
    private Vector2 moveDirection;

    public bool movementEnabled = true;

    private RectTransform rootRect;
    public float avoidStrength = 100f; // 越大越强烈排斥

    void Start()
    {
        GameObject rootObj = GameObject.FindWithTag("Root");
        if (rootObj != null)
        {
            rootRect = rootObj.GetComponent<RectTransform>();
        }

        rectTransform = GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas == null || canvas.renderMode == RenderMode.WorldSpace)
        {
            Debug.LogError("必须在 Screen Space 或 Overlay 模式的 Canvas 中使用此脚本！");
            enabled = false;
            return;
        }

        canvasRectTransform = canvas.GetComponent<RectTransform>();
        moveDirection = GetRandomDirection();

        noiseOffset = Random.Range(0f, 1000f); // 每个实例不一样的振动起点
    }

    void Update()
    {
        if (!movementEnabled) return;

        if (rootRect != null)
        {
            Vector2 myPos = rectTransform.anchoredPosition;
            Vector2 rootPos = rootRect.anchoredPosition;
            Vector2 dirFromRoot = (myPos - rootPos).normalized;
            float dist = Vector2.Distance(myPos, rootPos);

            float repelForce = avoidStrength / Mathf.Max(dist, 1f); // 防止除 0
            moveDirection += dirFromRoot * repelForce * Time.deltaTime;
            moveDirection = moveDirection.normalized;
        }

        Vector2 anchoredPos = rectTransform.anchoredPosition;
        anchoredPos += moveDirection * moveSpeed * Time.deltaTime;

        //  加入振翅震荡
        float t = Time.time * oscillationFrequency + noiseOffset;
        float xOscillation = (Mathf.PerlinNoise(t, 0f) - 0.5f) * 2f * oscillationAmplitude;
        float yOscillation = (Mathf.PerlinNoise(0f, t) - 0.5f) * 2f * oscillationAmplitude;
        anchoredPos += new Vector2(xOscillation, yOscillation) * Time.deltaTime;

        Vector2 size = rectTransform.rect.size;
        Vector2 canvasSize = canvasRectTransform.rect.size;
        float margin = 1f;

        // 左右边界检测
        if (anchoredPos.x - size.x * 0.5f < -canvasSize.x * 0.5f)
        {
            anchoredPos.x = -canvasSize.x * 0.5f + size.x * 0.5f + margin;
            moveDirection.x *= -1;
            moveDirection = RandomizeDirection(moveDirection);
        }
        else if (anchoredPos.x + size.x * 0.5f > canvasSize.x * 0.5f)
        {
            anchoredPos.x = canvasSize.x * 0.5f - size.x * 0.5f - margin;
            moveDirection.x *= -1;
            moveDirection = RandomizeDirection(moveDirection);
        }

        // 上下边界检测
        if (anchoredPos.y - size.y * 0.5f < -canvasSize.y * 0.5f)
        {
            anchoredPos.y = -canvasSize.y * 0.5f + size.y * 0.5f + margin;
            moveDirection.y *= -1;
            moveDirection = RandomizeDirection(moveDirection);
        }
        else if (anchoredPos.y + size.y * 0.5f > canvasSize.y * 0.5f)
        {
            anchoredPos.y = canvasSize.y * 0.5f - size.y * 0.5f - margin;
            moveDirection.y *= -1;
            moveDirection = RandomizeDirection(moveDirection);
        }

        rectTransform.anchoredPosition = anchoredPos;

        // 旋转（保留）
        if (enableRotation)
        {
            Transform target = rotationTarget != null ? rotationTarget : transform;
            target.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    Vector2 RandomizeDirection(Vector2 dir)
    {
        float angle = Random.Range(-bounceAngleVariance, bounceAngleVariance);
        return Quaternion.Euler(0, 0, angle) * dir;
    }
}
