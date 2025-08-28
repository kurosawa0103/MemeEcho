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

    public float edgeThreshold = 50f; // ������Ļ��Ե����С���루���أ�
    public float timeThreshold = 2f; // ����ʱ�䣨�룩

    public float positionScale;

    public string layer;

    private IntPtr gameWindowHandle;
    private GameObject edgeObject;
    private BoxCollider2D edgeCollider;
    private Text uiText;
    private float timeSinceLastCheck = 0f;
    private float checkInterval = 1f; // ÿ��һ����һ��
    private float timeOutOfBounds = 0f;

    void Start()
    {
        // ��ȡ��Ϸ���ھ��
        gameWindowHandle = GetGameWindowHandle();

        // �����߽��� GameObject
        edgeObject = new GameObject("WindowEdge");
        edgeCollider = edgeObject.AddComponent<BoxCollider2D>();
        HideEdge();

        // ���� UI Text
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
        // ����һ�� Canvas
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.referencePixelsPerUnit = 100;
        canvasObject.AddComponent<GraphicRaycaster>();

        // ���� UI Text ����
        GameObject textObject = new GameObject("TopWindowText");
        textObject.transform.parent = canvasObject.transform;
        uiText = textObject.AddComponent<Text>();
        uiText.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); // ʹ������ Arial ����
        uiText.fontSize = 24;
        uiText.color = Color.white;
        uiText.alignment = TextAnchor.MiddleRight;
        RectTransform rectTransform = uiText.rectTransform;
        rectTransform.anchorMin = new Vector2(1, 0); // ���½�
        rectTransform.anchorMax = new Vector2(1, 0); // ���½�
        rectTransform.pivot = new Vector2(1, 0); // ���½�
        rectTransform.anchoredPosition = new Vector2(-10, 10); // ���½�ƫ����
        rectTransform.sizeDelta = new Vector2(300, 50); // ȷ���ı������㹻��
    }

    private IntPtr GetGameWindowHandle()
    {
        // ʹ����Ϸ���ڱ����ȡ���
        string windowTitle = "BeetleCraft"; // �뽫�˴��ı����Ϊʵ����Ϸ���ڵı���
        return FindWindow(null, windowTitle);
    }

    private void UpdateWindowTitle()
    {
        IntPtr handle = GetForegroundWindow();

        if (handle != IntPtr.Zero)
        {
            // �ų���Ϸ����
            if (handle == gameWindowHandle)
            {
                return;
            }

            StringBuilder windowTitle = new StringBuilder(256);
            if (GetWindowText(handle, windowTitle, 256) > 0)
            {
                // ���� UI �ı�
                if (uiText != null)
                {
                    //uiText.text = $"��ǰ�ö�����: {windowTitle}";
                }
            }
        }
        else
        {
            if (uiText != null)
            {
                //uiText.text = "��ǰû���ö����ڡ�";
            }
        }
    }

    private void CheckAndUpdateEdge()
    {
        IntPtr handle = GetForegroundWindow();

        if (handle != IntPtr.Zero)
        {
            // �ų���Ϸ����
            if (handle == gameWindowHandle)
            {
                return;
            }

            // ��鴰���Ƿ���С����ر�
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

                    // ȷ������λ�úʹ�Сת����ȷ
                    Vector2 screenPosition = new Vector2(rect.left, Screen.height - rect.bottom);

                    // ת����Ļ���굽��������
                    Vector3 screenPositionWithDepth = new Vector3(screenPosition.x + windowSize.x / 2f, screenPosition.y + windowSize.y / 2f, Camera.main.nearClipPlane);
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPositionWithDepth);
                    worldPosition.z = 0; // ȷ�� z ��Ϊ 0���Ա��� 2D ƽ������ȷ��ʾ

                    if (!IsWindowMaximized(windowSize) && IsWindowOutOfBounds(rect))
                    {
                        timeOutOfBounds += checkInterval;

                        if (timeOutOfBounds >= timeThreshold)
                        {
                            // ���±߽��� SpriteRenderer �� BoxCollider2D
                            UpdateEdge(rect, worldPosition, windowSize);
                            //Debug.Log($"��ʾ���� \"{windowTitle}\" �ı߽硣");
                        }
                    }
                    else
                    {
                        timeOutOfBounds = 0f;
                        HideEdge();
                        //Debug.Log($"���� \"{windowTitle}\" û�г����߽磬�Ƴ��߽硣");
                    }
                }
            }
        }
        else
        {
            timeOutOfBounds = 0f;
            HideEdge();
            //Debug.Log("��ǰû���ö����ڡ�");
        }
    }


    private void UpdateEdge(RECT rect, Vector3 worldPosition, Vector2 windowSize)
    {
        // ���㴰�ڵ�����λ��
        Vector3 centerWorldPosition = new Vector3(
            rect.left + windowSize.x / 2f,
            Screen.height - rect.top - windowSize.y / 2f,
            Camera.main.nearClipPlane
        );

        // ת����Ļ���굽��������
        Vector3 worldPositionCenter = Camera.main.ScreenToWorldPoint(centerWorldPosition);
        worldPositionCenter.z = 0; // ȷ�� z ��Ϊ 0���Ա��� 2D ƽ������ȷ��ʾ

        // ���� edgeObject ��λ��
        edgeObject.transform.position = worldPositionCenter * positionScale;

        // ���� BoxCollider2D �� size ���ԣ�ֻ�����߶�Ϊԭ����һ��
        edgeCollider.size = new Vector2(windowSize.x / 100f, windowSize.y / 200f); // �߶�Ϊ���ڵ�һ��

        // ���� BoxCollider2D �� offset ���ԣ�����ײ�����ƣ�ʹ��λ�ڴ��ڵ��ϰ벿��
        edgeCollider.offset = new Vector2(0, windowSize.y / 400f); // ���Ƹ߶ȵ�һ�룬���־���

        // ���� layer Ϊ Window
        edgeObject.layer = LayerMask.NameToLayer(layer);

        edgeCollider.enabled = true;

        //Debug.Log($"���±߽��λ�úͳߴ磺");
        //Debug.Log($"λ��: {edgeObject.transform.position}");
        //Debug.Log($"Collider �ߴ�: {edgeCollider.size}");
    }

    private void HideEdge()
    {
        if (edgeCollider != null) edgeCollider.enabled = false;
    }

    private bool IsWindowMaximized(Vector2 windowSize)
    {
        bool maximized = Mathf.Abs(windowSize.x - Screen.width) < 10 && Mathf.Abs(windowSize.y - Screen.height) < 10;
        //Debug.Log($"�����Ƿ����: {maximized}");
        return maximized;
    }

    private bool IsWindowOutOfBounds(RECT rect)
    {
        bool outOfBounds = rect.left > edgeThreshold && rect.top > edgeThreshold &&
                           Screen.width - rect.right > edgeThreshold &&
                           Screen.height - rect.bottom > edgeThreshold;
        //Debug.Log($"�����Ƿ񳬳��߽�: {outOfBounds}");
        return outOfBounds;
    }
}
