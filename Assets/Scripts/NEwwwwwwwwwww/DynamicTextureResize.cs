using UnityEngine;

public class DynamicTextureResize : MonoBehaviour
{
    public Renderer targetRenderer;
    public RenderTexture sourceRenderTexture; // 原始 RenderTexture
    private RenderTexture runtimeRT;
    private int lastWidth, lastHeight;

    public Camera targetCamera; // 主相机

    void Start()
    {
        if (sourceRenderTexture == null || targetRenderer == null)
        {
            Debug.LogWarning("请绑定 Renderer 和 RenderTexture");
            return;
        }

        lastWidth = Screen.width;
        lastHeight = Screen.height;

        // 初始创建
        ResizeRenderTexture(lastWidth, lastHeight);
    }

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            ResizeRenderTexture(lastWidth, lastHeight);
        }
    }

    void ResizeRenderTexture(int width, int height)
    {
        // 销毁旧的
        if (runtimeRT != null)
            runtimeRT.Release();

        // 创建新的 RenderTexture
        runtimeRT = new RenderTexture(width, height, sourceRenderTexture.depth);
        runtimeRT.enableRandomWrite = sourceRenderTexture.enableRandomWrite;
        runtimeRT.format = sourceRenderTexture.format;
        runtimeRT.filterMode = sourceRenderTexture.filterMode;
        runtimeRT.wrapMode = sourceRenderTexture.wrapMode;
        runtimeRT.Create();

        // 如果你想拷贝原始内容，可以用 Blit
        Graphics.Blit(sourceRenderTexture, runtimeRT);

        // 应用到材质
        targetRenderer.material.SetTexture("_SreenTexture2D", runtimeRT);

        if (targetCamera != null)
            targetCamera.targetTexture = runtimeRT;

        Debug.Log($"已更新 RenderTexture 尺寸：{width}x{height}");
    }
}
