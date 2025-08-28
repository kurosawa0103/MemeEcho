using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.EventSystems; 

/// <summary>
/// 用于根据遮罩区域截取画面，判断是否满足某个拍照条件，满足则截图并加密保存图片，并加入 PhotoSystem 背包
/// </summary>
public class MaskedScreenshot : MonoBehaviour
{
    public Camera mainCamera;             // 主摄像机，用于将世界坐标转换为屏幕坐标
    public Collider maskCollider3D;      // 3D 遮罩区域的碰撞盒（用于确定截图中心）
    public SpriteRenderer flashRenderer; // 用于截图时的白色闪光效果
    public float doubleClickThreshold = 0.5f; // 双击检测阈值（当前未使用）
    private float lastClickTime = 0f;

    [Header("截图参数")]
    public int screenshotWidth = 512;  // 截图宽度
    public int screenshotHeight = 512; // 截图高度

    public PhotoSystem photoSystem;  // PhotoSystem 引用，用于将截图加入背包
    public List<PhotoCaptureCondition> conditionConfigs; // 所有截图条件配置
    public SpriteTargetTracker spriteTargetTracker; // 当前追踪对象列表

    [Header("截图时剔除的 Layer")]
    public LayerMask excludedLayerMask;  // 配置想要在截图中剔除的 Layer


    [Tooltip("点击忽略的 UI 层")]
    public LayerMask uiLayerMask = 1 << 5; // 默认 UI 层

    [Header("加密保存")]
    public string encryptedImageFolder = "EncryptedPhotos"; // 加密图片保存的文件夹名

    // 添加一个事件或回调委托
    public event System.Action<PhotoCaptureCondition> OnScreenshotTaken;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 首先检查是否点击到了 UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                // 获取所有的 UI 点击结果
                var raycastResults = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, raycastResults);

                foreach (var result in raycastResults)
                {
                    // 如果点击的 UI 元素在指定的 UI 层
                    if (((1 << result.gameObject.layer) & uiLayerMask) != 0)
                    {
                        //Debug.Log($"[ClickTriggerCollider] 点击到了指定的 UI 层：{result.gameObject.name}，忽略处理。");

                        return; // 如果点击到忽略的 UI 层，直接返回
                    }
                }
            }

            // Raycast 检测点击是否命中遮罩区域
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (maskCollider3D != null && maskCollider3D.Raycast(ray, out RaycastHit hit, 100f))
            {
                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick <= doubleClickThreshold)
                {
                    Debug.Log("🖱️ 成功双击遮罩区域，执行拍照");
                    TakePhoto();
                }

                lastClickTime = Time.time;
            }
            else
            {
                Debug.Log("点击未命中遮罩区域，不执行拍照");
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(CaptureMaskedArea());
        }
    }
    public void TakePhoto()
    {
        StartCoroutine(CaptureMaskedArea());
    }
    /// <summary>
    /// 根据遮罩区域截图，并判断是否满足任意拍摄条件，若满足则保存并添加到 PhotoSystem
    /// </summary>
    IEnumerator CaptureMaskedArea()
    {
        PhotoCaptureCondition matchedCondition = null;
        int highestPriority = int.MinValue;

        foreach (var config in conditionConfigs)
        {
            if (CheckCondition(config) && config.priority > highestPriority)
            {
                matchedCondition = config;
                highestPriority = config.priority;
            }
        }

        if (matchedCondition == null)
        {
            Debug.Log("没有满足的条件，取消截图");
            yield break;
        }

        flashRenderer.enabled = false;
        yield return new WaitForEndOfFrame();

        // ===== 计算中心点屏幕坐标（根据遮罩中心） =====
        Vector3 centerWorld = maskCollider3D.bounds.center;
        Vector3 centerScreen = mainCamera.WorldToScreenPoint(centerWorld);

        // ===== 创建渲染用 RenderTexture（保持和屏幕一样大）=====
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        RenderTexture rt = new RenderTexture(screenWidth, screenHeight, 24);
        RenderTexture.active = rt;

        // ===== 设置摄像机，剔除特定 Layer 并渲染 =====
        int originalCullingMask = mainCamera.cullingMask;
        RenderTexture originalTarget = mainCamera.targetTexture;

        mainCamera.cullingMask = originalCullingMask & ~excludedLayerMask.value;
        mainCamera.targetTexture = rt;
        mainCamera.Render();

        // ===== 在中心点处裁剪截图区域（自定义尺寸）=====
        int x = Mathf.Clamp((int)(centerScreen.x - screenshotWidth / 2f), 0, screenWidth - screenshotWidth);
        int y = Mathf.Clamp((int)(centerScreen.y - screenshotHeight / 2f), 0, screenHeight - screenshotHeight);

        Texture2D tex = new Texture2D(screenshotWidth, screenshotHeight, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(x, y, screenshotWidth, screenshotHeight), 0, 0);
        tex.Apply();

        // ===== 恢复设置 =====
        mainCamera.cullingMask = originalCullingMask;
        mainCamera.targetTexture = originalTarget;
        RenderTexture.active = null;
        rt.Release();

        // ===== 加密保存 =====
        byte[] pngData = tex.EncodeToPNG();
        byte[] encryptedData = EncryptImage(pngData);

        string folder = Path.Combine(Application.persistentDataPath, encryptedImageFolder);
        Directory.CreateDirectory(folder);
        string conditionHash = matchedCondition.conditionID.GetHashCode().ToString();
        string path = Path.Combine(folder, $"Photo_{conditionHash}.dat");
        if (File.Exists(path)) File.Delete(path);

        // 新增字段：照片名（使用条件ID作为示例）
        string photoName = matchedCondition.conditionID;

        // 编码字段
        byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(matchedCondition.description);
        byte[] photoNameBytes = System.Text.Encoding.UTF8.GetBytes(photoName);

        byte[] descLengthBytes = BitConverter.GetBytes(descriptionBytes.Length);
        byte[] nameLengthBytes = BitConverter.GetBytes(photoNameBytes.Length);

        // 构建完整数据
        byte[] fullData = new byte[
            descLengthBytes.Length +
            descriptionBytes.Length +
            nameLengthBytes.Length +
            photoNameBytes.Length +
            encryptedData.Length
        ];

        int offset = 0;
        Buffer.BlockCopy(descLengthBytes, 0, fullData, offset, descLengthBytes.Length);
        offset += descLengthBytes.Length;

        Buffer.BlockCopy(descriptionBytes, 0, fullData, offset, descriptionBytes.Length);
        offset += descriptionBytes.Length;

        Buffer.BlockCopy(nameLengthBytes, 0, fullData, offset, nameLengthBytes.Length);
        offset += nameLengthBytes.Length;

        Buffer.BlockCopy(photoNameBytes, 0, fullData, offset, photoNameBytes.Length);
        offset += photoNameBytes.Length;

        Buffer.BlockCopy(encryptedData, 0, fullData, offset, encryptedData.Length);

        // 写入 .dat 文件
        File.WriteAllBytes(path, fullData);

        Debug.Log($"照片满足条件 [{matchedCondition.conditionID}] 已保存到：{path}");

        if (photoSystem != null)
        {
            photoSystem.AddPhoto(path);
        }

        flashRenderer.enabled = true;
        StartCoroutine(FlashEffect());

        // 通知所有订阅者截图已完成
        OnScreenshotTaken?.Invoke(matchedCondition);
        Debug.Log($"截图完成，触发 ScreenshotTaken 事件，匹配条件：{matchedCondition.conditionID}");
    }

    /// <summary>
    /// 闪光淡出动画效果
    /// </summary>
    IEnumerator FlashEffect()
    {
        Color c = flashRenderer.color;
        c.a = 1f;
        flashRenderer.color = c;

        yield return new WaitForSeconds(0.1f);

        float fadeDuration = 0.5f;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            flashRenderer.color = c;
            yield return null;
        }

        c.a = 0f;
        flashRenderer.color = c;
    }

    /// <summary>
    /// 判断一个条件是否满足
    /// </summary>
    public bool CheckCondition(PhotoCaptureCondition condition)
    {
        List<string> missing = new List<string>();

        foreach (var required in condition.requiredObjectsName)
        {
            bool found = spriteTargetTracker.photoTrackList.Exists(p => p.photoTargetName == required);
            if (!found)
            {
                missing.Add(required);
            }
        }

        if (missing.Count > 0)
        {
            Debug.Log($"条件 [{condition.conditionID}] 未满足，缺少：{string.Join(",", missing)}");
            Debug.Log("当前追踪到的对象有：");
            foreach (var p in spriteTargetTracker.photoTrackList)
            {
                Debug.Log($" -> {p.photoTargetName}");
            }
            return false;
        }

        return true;
    }

    /// <summary>
    /// 简单异或加密函数（单字节密钥加密 PNG 数据）
    /// </summary>
    private byte[] EncryptImage(byte[] data)
    {
        byte key = 0xAA; // 固定密钥
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key;
        }
        return data;
    }
}
