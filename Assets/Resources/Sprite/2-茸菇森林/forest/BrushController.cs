using UnityEngine;

public class BrushController : MonoBehaviour
{
    public LayerMask paintableLayer;
    public LayerMask brushSelectorLayer; // 用于笔刷选择器的图层

    [Header("当前笔刷状态")]
    public string currentBrushTag = "Default";
    public Color currentBrushColor = Color.red;

    private Camera mainCamera;

    void Start()
    {
        // 缓存相机引用，避免每帧调用Camera.main
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        if (mainCamera == null)
        {
            Debug.LogError("找不到相机！请确保场景中有相机。");
        }

        Debug.Log($"初始笔刷: {currentBrushTag}, 颜色: {currentBrushColor}");
    }

    void Update()
    {
        if (mainCamera == null) return;

        // 修复鼠标坐标转换
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z; // 设置正确的Z值
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mousePos);

        // 调试输出鼠标坐标
        // Debug.Log($"鼠标屏幕坐标: {Input.mousePosition}, 世界坐标: {mouseWorldPos}");

        if (Input.GetMouseButtonDown(0))
        {
            // 首先检查是否点击了笔刷选择器
            RaycastHit2D brushHit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, brushSelectorLayer);
            if (brushHit.collider != null)
            {
                // 笔刷选择器会自己处理点击事件
                return;
            }
        }

        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, paintableLayer);
            if (hit.collider != null)
            {
                Debug.Log("点击到了: " + hit.collider.name);
                ColorableSprite colorable = hit.collider.GetComponent<ColorableSprite>();
                if (colorable != null)
                {
                    // 检查标签匹配
                    if (CanPaintWithCurrentBrush(colorable))
                    {
                        colorable.Paint(mouseWorldPos, currentBrushColor);
                    }
                    else
                    {
                        Debug.Log($"笔刷标签 '{currentBrushTag}' 不能给标签为 '{colorable.paintableTag}' 的物体上色");
                    }
                }
            }
            else
            {
                // Debug.Log("没有点击到可绘制物体");
            }
        }

        // 键盘快捷键
        HandleKeyboardInput(mouseWorldPos);
    }

    // 检查当前笔刷是否能给指定物体上色
    private bool CanPaintWithCurrentBrush(ColorableSprite colorable)
    {
        // 如果物体没有设置标签或标签为空，允许任何笔刷上色
        if (string.IsNullOrEmpty(colorable.paintableTag))
        {
            return true;
        }

        // 检查标签是否匹配
        return currentBrushTag == colorable.paintableTag;
    }

    // 选择笔刷
    public void SelectBrush(string brushTag, Color brushColor)
    {
        currentBrushTag = brushTag;
        currentBrushColor = brushColor;

        // 取消所有其他笔刷的选择状态
        BrushSelector.DeselectAll();

        Debug.Log($"切换到笔刷: {brushTag}, 颜色: {brushColor}");
    }

    private void HandleKeyboardInput(Vector2 mouseWorldPos)
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, paintableLayer);
            if (hit.collider != null)
            {
                ColorableSprite colorable = hit.collider.GetComponent<ColorableSprite>();
                if (colorable != null)
                {
                    colorable.SaveTextureToFile();
                }
            }
        }

        // 按R键重置为灰度
        if (Input.GetKeyDown(KeyCode.R))
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, paintableLayer);
            if (hit.collider != null)
            {
                ColorableSprite colorable = hit.collider.GetComponent<ColorableSprite>();
                if (colorable != null)
                {
                    colorable.ResetToGrayscale();
                    Debug.Log("重置为灰度模式");
                }
            }
        }

        // 按O键显示原始图像
        if (Input.GetKeyDown(KeyCode.O))
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, paintableLayer);
            if (hit.collider != null)
            {
                ColorableSprite colorable = hit.collider.GetComponent<ColorableSprite>();
                if (colorable != null)
                {
                    colorable.ShowOriginal();
                    Debug.Log("显示原始图像");
                }
            }
        }

        // 数字键快捷选择笔刷
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectBrushByIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectBrushByIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectBrushByIndex(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectBrushByIndex(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectBrushByIndex(4);
    }

    private void SelectBrushByIndex(int index)
    {
        BrushSelector[] selectors = FindObjectsOfType<BrushSelector>();
        if (index >= 0 && index < selectors.Length)
        {
            SelectBrush(selectors[index].brushTag, selectors[index].displayColor);
            selectors[index].SetSelected(true);
        }
    }
}