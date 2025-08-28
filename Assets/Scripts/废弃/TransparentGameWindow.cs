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
    private bool isTransparent = false;  // ��ǰ��괩͸״̬
    private bool isFirstClick = false;   // �Ƿ��ѷ�����һ�ε��

    public List<GraphicRaycaster> raycasters;  // UI���߼��
    private EventSystem eventSystem;  // �Զ���ȡ EventSystem
    public LayerMask ignoreLayerMask; // Ҫ���ԵĲ㼶
    private void OnEnable()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("û���ҵ� EventSystem����ȷ����������һ�� EventSystem ����");
        }

        raycasters = new List<GraphicRaycaster>(FindObjectsOfType<GraphicRaycaster>());
        //if (raycasters.Count == 0)
        //{
        //    Debug.LogError("û���ҵ��κ� GraphicRaycaster����ȷ��Canvas���и������");
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
            Debug.LogError("û���ҵ� EventSystem����ȷ����������һ�� EventSystem ����");
        }

        raycasters = new List<GraphicRaycaster>(FindObjectsOfType<GraphicRaycaster>());
        //if (raycasters.Count == 0)
        //{
        //    Debug.LogError("û���ҵ��κ� GraphicRaycaster����ȷ��Canvas���и������");
        //}
        
        // �������غ���������͸���������Բ���ʼ��
        //SetTransparentWindow();
        DisableMouseTransparent();  // ��ʼ״̬����Ϊ����͸
        isFirstClick = false;  // ���õ�һ�ε��״̬
        
        
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
    SetTransparentWindow();  // **����ִ��**
#endif
    }
    private void Start()
    {
        // �Զ���ȡ��ǰ�����е� EventSystem 
        if (eventSystem == null)
        {
            eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
            {
                DontDestroyOnLoad(eventSystem.gameObject);
            }
            else
            {
                Debug.LogError("û���ҵ� EventSystem����ȷ����������һ�� EventSystem ����");
            }
        }

        raycasters = new List<GraphicRaycaster>(FindObjectsOfType<GraphicRaycaster>());
        //if (raycasters.Count == 0)
        //{
        //   Debug.LogError("û���ҵ��κ� GraphicRaycaster����ȷ��Canvas���и������");
        //}

        StartCoroutine(InitializeTransparency());
        Application.runInBackground = true;
    }
    // **�ȴ���һ֡������������͸��**
    private IEnumerator InitializeTransparency()
    {
        yield return new WaitForEndOfFrame();  // **�ȴ���һִ֡����**
        while (!IsWindowTransparent())
        {
            SetTransparentWindow();
            yield return null;
        }

        Debug.Log("����͸��������ɣ�");
    }

    private bool IsWindowTransparent()
    {
        int style = GetWindowLong(hWnd, GWL_EXSTYLE);
        return (style & WS_EX_TRANSPARENT) != 0;
    }
    void Update()
    {
        // ����һ�ε���Խ�����͸����
        if (!isFirstClick && Input.GetMouseButtonDown(0))
        {
            isFirstClick = true;  // ���Ϊ�ѷ�����һ�ε��
        }

        if (isFirstClick)
        {
            bool isHoveringUI = IsHoveringUI();
            bool isHoveringRigidbody2D = IsHovering3DObject();

            // ֻ�е����û��ָ��UI�����Rigidbody2D������ʱ������͸
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
                //Debug.Log($"��⵽UIԪ��: {results[0].gameObject.name}");
                return true;
            }
        }
        return false;
    }
    private bool IsHovering3DObject()
    {
        // �����λ�÷������ߵ������е�����
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // ������ߵ����ͷ��򣬼���Ƿ���ȷ
        //Debug.Log("�������: " + ray.origin + " ���߷���: " + ray.direction);

        // ʹ�� Physics.Raycast ������ײ��⣬����Ƿ���3D������ײ
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayerMask))  // ʹ�� 3D �����߼��
        {
            // �������߸�����
            //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);  // ��ɫ���ߣ����� 100

            // ��������������壬������������
            //Debug.Log("��������������: " + hit.collider.name);
            return true;
        }
        else
        {
            // �������߸�����
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);  // ��ɫ���ߣ����� 100
            //Debug.Log("����û����������");
        }

        return false;
    }




    private void EnableMouseTransparent()
    {
        if (!isTransparent)
        {
#if !UNITY_EDITOR
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
            Debug.Log("����Ϊ��͸");
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
            Debug.Log("����Ϊ����͸");
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
