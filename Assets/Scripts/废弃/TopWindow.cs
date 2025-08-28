using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TopWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsWindow(IntPtr hWnd);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public float edgeThreshold = 50f; // 距离屏幕边缘的最小距离（像素）
    public float timeThreshold = 2f; // 持续时间（秒）

    public float positionScale;

    public string layer;

    private IntPtr gameWindowHandle;
    private GameObject edgeObject;
    private BoxCollider2D edgeCollider;
    private Text uiText;
    private float timeSinceLastCheck = 0f;
    private float checkInterval = 1f; // 每隔一秒检查一次
    private float timeOutOfBounds = 0f;

    void Start()
    {
        // 获取游戏窗口句柄
        gameWindowHandle = GetGameWindowHandle();

        // 创建边界块的 GameObject
        edgeObject = new GameObject("WindowEdge");
        edgeCollider = edgeObject.AddComponent<BoxCollider2D>();
        HideEdge();

        // 创建 UI Text
        CreateUIText();
    }

    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;

        if (timeSinceLastCheck >= checkInterval)
        {
            timeSinceLastCheck = 0f;
            UpdateWindowTitle();
            CheckAndUpdateEdge();
        }
    }

    private void CreateUIText()
    {
        // 创建一个 Canvas
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.referencePixelsPerUnit = 100;
        canvasObject.AddComponent<GraphicRaycaster>();

        // 创建 UI Text 对象
        GameObject textObject = new GameObject("TopWindowText");
        textObject.transform.parent = canvasObject.transform;
        uiText = textObject.AddComponent<Text>();
        uiText.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); // 使用内置 Arial 字体
        uiText.fontSize = 24;
        uiText.color = Color.white;
        uiText.alignment = TextAnchor.MiddleRight;
        RectTransform rectTransform = uiText.rectTransform;
        rectTransform.anchorMin = new Vector2(1, 0); // 右下角
        rectTransform.anchorMax = new Vector2(1, 0); // 右下角
        rectTransform.pivot = new Vector2(1, 0); // 右下角
        rectTransform.anchoredPosition = new Vector2(-10, 10); // 右下角偏移量
        rectTransform.sizeDelta = new Vector2(300, 50); // 确保文本区域足够大
    }

    private IntPtr GetGameWindowHandle()
    {
        // 使用游戏窗口标题获取句柄
        string windowTitle = "BeetleCraft"; // 请将此处的标题改为实际游戏窗口的标题
        return FindWindow(null, windowTitle);
    }

    private void UpdateWindowTitle()
    {
        IntPtr handle = GetForegroundWindow();

        if (handle != IntPtr.Zero)
        {
            // 排除游戏窗口
            if (handle == gameWindowHandle)
            {
                return;
            }

            StringBuilder windowTitle = new StringBuilder(256);
            if (GetWindowText(handle, windowTitle, 256) > 0)
            {
                // 更新 UI 文本
                if (uiText != null)
                {
                    //uiText.text = $"当前置顶窗口: {windowTitle}";
                }
            }
        }
        else
        {
            if (uiText != null)
            {
                //uiText.text = "当前没有置顶窗口。";
            }
        }
    }

    private void CheckAndUpdateEdge()
    {
        IntPtr handle = GetForegroundWindow();

        if (handle != IntPtr.Zero)
        {
            // 排除游戏窗口
            if (handle == gameWindowHandle)
            {
                return;
            }

            // 检查窗口是否最小化或关闭
            if (IsIconic(handle) || !IsWindow(handle))
            {
                timeOutOfBounds = 0f;
                HideEdge();
                return;
            }

            StringBuilder windowTitle = new StringBuilder(256);
            if (GetWindowText(handle, windowTitle, 256) > 0)
            {
                RECT rect;
                if (GetWindowRect(handle, out rect))
                {
                    Vector2 windowSize = new Vector2(rect.right - rect.left, rect.bottom - rect.top);

                    // 确保窗口位置和大小转换正确
                    Vector2 screenPosition = new Vector2(rect.left, Screen.height - rect.bottom);

                    // 转换屏幕坐标到世界坐标
                    Vector3 screenPositionWithDepth = new Vector3(screenPosition.x + windowSize.x / 2f, screenPosition.y + windowSize.y / 2f, Camera.main.nearClipPlane);
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPositionWithDepth);
                    worldPosition.z = 0; // 确保 z 轴为 0，以便在 2D 平面上正确显示

                    if (!IsWindowMaximized(windowSize) && IsWindowOutOfBounds(rect))
                    {
                        timeOutOfBounds += checkInterval;

                        if (timeOutOfBounds >= timeThreshold)
                        {
                            // 更新边界块的 SpriteRenderer 和 BoxCollider2D
                            UpdateEdge(rect, worldPosition, windowSize);
                            //Debug.Log($"显示窗口 \"{windowTitle}\" 的边界。");
                        }
                    }
                    else
                    {
                        timeOutOfBounds = 0f;
                        HideEdge();
                        //Debug.Log($"窗口 \"{windowTitle}\" 没有超出边界，移除边界。");
                    }
                }
            }
        }
        else
        {
            timeOutOfBounds = 0f;
            HideEdge();
            //Debug.Log("当前没有置顶窗口。");
        }
    }


    private void UpdateEdge(RECT rect, Vector3 worldPosition, Vector2 windowSize)
    {
        // 计算窗口的中心位置
        Vector3 centerWorldPosition = new Vector3(
            rect.left + windowSize.x / 2f,
            Screen.height - rect.top - windowSize.y / 2f,
            Camera.main.nearClipPlane
        );

        // 转换屏幕坐标到世界坐标
        Vector3 worldPositionCenter = Camera.main.ScreenToWorldPoint(centerWorldPosition);
        worldPositionCenter.z = 0; // 确保 z 轴为 0，以便在 2D 平面上正确显示

        // 更新 edgeObject 的位置
        edgeObject.transform.position = worldPositionCenter * positionScale;

        // 更新 BoxCollider2D 的 size 属性，只调整高度为原来的一半
        edgeCollider.size = new Vector2(windowSize.x / 100f, windowSize.y / 200f); // 高度为窗口的一半

        // 更新 BoxCollider2D 的 offset 属性，将碰撞器上移，使其位于窗口的上半部分
        edgeCollider.offset = new Vector2(0, windowSize.y / 400f); // 上移高度的一半，保持居中

        // 设置 layer 为 Window
        edgeObject.layer = LayerMask.NameToLayer(layer);

        edgeCollider.enabled = true;

        //Debug.Log($"更新边界块位置和尺寸：");
        //Debug.Log($"位置: {edgeObject.transform.position}");
        //Debug.Log($"Collider 尺寸: {edgeCollider.size}");
    }

    private void HideEdge()
    {
        if (edgeCollider != null) edgeCollider.enabled = false;
    }

    private bool IsWindowMaximized(Vector2 windowSize)
    {
        bool maximized = Mathf.Abs(windowSize.x - Screen.width) < 10 && Mathf.Abs(windowSize.y - Screen.height) < 10;
        //Debug.Log($"窗口是否最大化: {maximized}");
        return maximized;
    }

    private bool IsWindowOutOfBounds(RECT rect)
    {
        bool outOfBounds = rect.left > edgeThreshold && rect.top > edgeThreshold &&
                           Screen.width - rect.right > edgeThreshold &&
                           Screen.height - rect.bottom > edgeThreshold;
        //Debug.Log($"窗口是否超出边界: {outOfBounds}");
        return outOfBounds;
    }
}
