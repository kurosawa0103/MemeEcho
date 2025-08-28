using UnityEngine;

public class DynamicTextureResize : MonoBehaviour
{
    public Renderer targetRenderer;
    public RenderTexture sourceRenderTexture; // ԭʼ RenderTexture
    private RenderTexture runtimeRT;
    private int lastWidth, lastHeight;

    public Camera targetCamera; // �����

    void Start()
    {
        if (sourceRenderTexture == null || targetRenderer == null)
        {
            Debug.LogWarning("��� Renderer �� RenderTexture");
            return;
        }

        lastWidth = Screen.width;
        lastHeight = Screen.height;

        // ��ʼ����
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
        // ���پɵ�
        if (runtimeRT != null)
            runtimeRT.Release();

        // �����µ� RenderTexture
        runtimeRT = new RenderTexture(width, height, sourceRenderTexture.depth);
        runtimeRT.enableRandomWrite = sourceRenderTexture.enableRandomWrite;
        runtimeRT.format = sourceRenderTexture.format;
        runtimeRT.filterMode = sourceRenderTexture.filterMode;
        runtimeRT.wrapMode = sourceRenderTexture.wrapMode;
        runtimeRT.Create();

        // ������뿽��ԭʼ���ݣ������� Blit
        Graphics.Blit(sourceRenderTexture, runtimeRT);

        // Ӧ�õ�����
        targetRenderer.material.SetTexture("_SreenTexture2D", runtimeRT);

        if (targetCamera != null)
            targetCamera.targetTexture = runtimeRT;

        Debug.Log($"�Ѹ��� RenderTexture �ߴ磺{width}x{height}");
    }
}
