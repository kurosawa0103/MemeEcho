using UnityEngine;
using UnityEngine.UI;

public class ShakeAlpha : MonoBehaviour
{
    [Header("目标 Image 组件")]
    public Image targetImage;

    [Header("拖拽窗口组件")]
    public DraggableUIWindow window;

    [Tooltip("判定为移动的最小距离（像素）")]
    public float moveDistanceThreshold = 30f;

    [Header("移动减少透明度")]
    public float moveAlphaDecrease = 0.2f;

    [Header("透明度恢复速率")]
    public float alphaRecoveryPerSecond = 0.5f;

    [Header("是否在透明度为0后禁用")]
    public bool disableWhenTransparent = true;

    private float currentAlpha = 1f;
    private bool isDisabled = false;

    private Vector2 lastPosition;

    void Awake()
    {
        // 自动获取 targetImage
        if (targetImage == null)
        {
            GameObject boxBorder = GameObject.FindGameObjectWithTag("BoxBorder");
            if (boxBorder != null && boxBorder.transform.childCount > 0)
            {
                Transform firstChild = boxBorder.transform.GetChild(0);
                foreach (Transform child in firstChild)
                {
                    if (child.name == "黑幕")
                    {
                        targetImage = child.GetComponent<Image>();
                        break;
                    }
                }
            }
        }


        // 自动获取 DraggableUIWindow（可选）
        if (window == null)
        {
            window = GameObject.FindGameObjectWithTag("Root").transform.GetComponent<DraggableUIWindow>();
        }

        currentAlpha = 1f;
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(true);
            UpdateImageAlpha();
        }

        lastPosition = window.transform.position; // 初始化最后的位置
    }
    void OnEnable()
    {
        if (window != null)
        {
            lastPosition = window.transform.position; // 确保当前是最新位置
        }

        isDisabled = false;
        currentAlpha = 1f;

        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(true);
            UpdateImageAlpha();
        }
    }
    void Update()
    {
        if (isDisabled || targetImage == null || window == null) return;

        // 获取当前窗口的位置
        Vector2 currentPosition = window.transform.position;

        // 检查窗口是否移动超过指定的距离
        if (Vector2.Distance(currentPosition, lastPosition) > moveDistanceThreshold)
        {
            // 如果窗口移动了，减少透明度
            currentAlpha -= moveAlphaDecrease;
            currentAlpha = Mathf.Clamp01(currentAlpha); // 确保透明度在0到1之间
            UpdateImageAlpha();

            if (disableWhenTransparent && currentAlpha <= 0f)
            {
                isDisabled = true;
                targetImage.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }
        else if (currentAlpha < 1f)
        {
            // 如果窗口没有移动，恢复透明度
            currentAlpha += alphaRecoveryPerSecond * Time.deltaTime;
            currentAlpha = Mathf.Clamp01(currentAlpha);
            UpdateImageAlpha();

            if (!targetImage.gameObject.activeSelf && currentAlpha > 0f)
                targetImage.gameObject.SetActive(true);
        }

        // 更新最后的位置
        lastPosition = currentPosition;
    }

    public void ResetAlpha()
    {
        isDisabled = false;
        currentAlpha = 1f;
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(true);
            UpdateImageAlpha();
        }

        lastPosition = window.transform.position; // 重置最后的位置
    }

    private void UpdateImageAlpha()
    {
        if (targetImage != null)
        {
            var color = targetImage.color;
            color.a = currentAlpha;
            targetImage.color = color;
        }
    }
}
