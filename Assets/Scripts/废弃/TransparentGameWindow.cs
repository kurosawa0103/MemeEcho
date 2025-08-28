using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransparentGameWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    const int GWL_EXSTYLE = -20;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    private IntPtr hWnd;
    private bool isTransparent = false;  // 当前鼠标穿透状态
    private bool isFirstClick = false;   // 是否已发生第一次点击

    public List<GraphicRaycaster> raycasters;  // UI射线检测
    private EventSystem eventSystem;  // 自动获取 EventSystem
    public LayerMask ignoreLayerMask; // 要忽略的层级
    private void OnEnable()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("没有找到 EventSystem，请确保场景中有一个 EventSystem 对象。");
        }

        raycasters = new List<GraphicRaycaster>(FindObjectsOfType<GraphicRaycaster>());
        //if (raycasters.Count == 0)
        //{
        //    Debug.LogError("没有找到任何 GraphicRaycaster，请确保Canvas上有该组件。");
        //}

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("没有找到 EventSystem，请确保场景中有一个 EventSystem 对象。");
        }

        raycasters = new List<GraphicRaycaster>(FindObjectsOfType<GraphicRaycaster>());
        //if (raycasters.Count == 0)
        //{
        //    Debug.LogError("没有找到任何 GraphicRaycaster，请确保Canvas上有该组件。");
        //}
        
        // 场景加载后重新设置透明窗口属性并初始化
        //SetTransparentWindow();
        DisableMouseTransparent();  // 初始状态设置为不穿透
        isFirstClick = false;  // 重置第一次点击状态
        
        
    }

    private void SetTransparentWindow()
    {
#if !UNITY_EDITOR
        hWnd = GetActiveWindow();
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        SetLayeredWindowAttributes(hWnd, 0, 255, 0);
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif
    }
    private void Awake()
    {
#if !UNITY_EDITOR
    SetTransparentWindow();  // **最早执行**
#endif
    }
    private void Start()
    {
        // 自动获取当前场景中的 EventSystem 
        if (eventSystem == null)
        {
            eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
            {
                DontDestroyOnLoad(eventSystem.gameObject);
            }
            else
            {
                Debug.LogError("没有找到 EventSystem，请确保场景中有一个 EventSystem 对象。");
            }
        }

        raycasters = new List<GraphicRaycaster>(FindObjectsOfType<GraphicRaycaster>());
        //if (raycasters.Count == 0)
        //{
        //   Debug.LogError("没有找到任何 GraphicRaycaster，请确保Canvas上有该组件。");
        //}

        StartCoroutine(InitializeTransparency());
        Application.runInBackground = true;
    }
    // **等待第一帧结束后再设置透明**
    private IEnumerator InitializeTransparency()
    {
        yield return new WaitForEndOfFrame();  // **等待第一帧执行完**
        while (!IsWindowTransparent())
        {
            SetTransparentWindow();
            yield return null;
        }

        Debug.Log("窗口透明设置完成！");
    }

    private bool IsWindowTransparent()
    {
        int style = GetWindowLong(hWnd, GWL_EXSTYLE);
        return (style & WS_EX_TRANSPARENT) != 0;
    }
    void Update()
    {
        // 检测第一次点击以解锁穿透功能
        if (!isFirstClick && Input.GetMouseButtonDown(0))
        {
            isFirstClick = true;  // 标记为已发生第一次点击
        }

        if (isFirstClick)
        {
            bool isHoveringUI = IsHoveringUI();
            bool isHoveringRigidbody2D = IsHovering3DObject();

            // 只有当鼠标没有指向UI或带有Rigidbody2D的物体时才允许穿透
            if (!isHoveringUI && !isHoveringRigidbody2D)
            {
                EnableMouseTransparent();
            }
            else
            {
                DisableMouseTransparent();
            }
        }
    }

    private bool IsHoveringUI()
    {
        if (eventSystem == null)
            return false;

        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        foreach (GraphicRaycaster raycaster in raycasters)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            if (results.Count > 0)
            {
                //Debug.Log($"检测到UI元素: {results[0].gameObject.name}");
                return true;
            }
        }
        return false;
    }
    private bool IsHovering3DObject()
    {
        // 从鼠标位置发射射线到场景中的物体
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 输出射线的起点和方向，检查是否正确
        //Debug.Log("射线起点: " + ray.origin + " 射线方向: " + ray.direction);

        // 使用 Physics.Raycast 进行碰撞检测，检测是否与3D物体碰撞
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayerMask))  // 使用 3D 的射线检测
        {
            // 绘制射线辅助线
            //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);  // 红色射线，长度 100

            // 如果射线碰到物体，输出物体的名字
            //Debug.Log("射线碰到的物体: " + hit.collider.name);
            return true;
        }
        else
        {
            // 绘制射线辅助线
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);  // 红色射线，长度 100
            //Debug.Log("射线没有碰到物体");
        }

        return false;
    }




    private void EnableMouseTransparent()
    {
        if (!isTransparent)
        {
#if !UNITY_EDITOR
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
            Debug.Log("设置为穿透");
#endif
            isTransparent = true;
        }
    }

    private void DisableMouseTransparent()
    {
        if (isTransparent)
        {
#if !UNITY_EDITOR
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
            Debug.Log("设置为不穿透");
#endif
            isTransparent = false;
        }
    }

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }
}
