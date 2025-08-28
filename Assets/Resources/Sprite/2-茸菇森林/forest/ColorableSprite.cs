using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorableSprite : MonoBehaviour
{
    private Texture2D texture;
    private Texture2D originalTexture; // 存储原始彩色纹理
    private SpriteRenderer sr;

    public int brushSize = 8;
    public Color brushColor = Color.red;
    [Header("灰度设置")]
    public bool startWithGrayscale = true; // 是否开始时显示灰度
    [Header("标签系统")]
    public string paintableTag = ""; // 这个物体可以被什么标签的笔刷上色

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // 保存原始纹理
        Texture2D original = sr.sprite.texture;

        // 创建原始纹理的副本
        originalTexture = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
        originalTexture.filterMode = FilterMode.Point;
        originalTexture.wrapMode = TextureWrapMode.Clamp;
        Color[] originalPixels = original.GetPixels();
        originalTexture.SetPixels(originalPixels);
        originalTexture.Apply();

        // 创建工作纹理
        texture = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        // 根据设置决定初始状态
        if (startWithGrayscale)
        {
            // 转换为灰度
            Color[] grayscalePixels = ConvertToGrayscale(originalPixels);
            texture.SetPixels(grayscalePixels);
        }
        else
        {
            // 使用原始颜色
            texture.SetPixels(originalPixels);
        }

        texture.Apply();

        // 创建新的Sprite并替换
        var newSprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            sr.sprite.pixelsPerUnit
        );
        sr.sprite = newSprite;

        Debug.Log($"纹理初始化完成，尺寸: {texture.width}x{texture.height}, 灰度模式: {startWithGrayscale}");
    }

    // 将像素数组转换为灰度
    private Color[] ConvertToGrayscale(Color[] pixels)
    {
        Color[] grayscalePixels = new Color[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            Color original = pixels[i];
            // 使用标准的灰度转换公式
            float gray = original.r * 0.299f + original.g * 0.587f + original.b * 0.114f;
            grayscalePixels[i] = new Color(gray, gray, gray, original.a);
        }
        return grayscalePixels;
    }

    public void SaveTextureToFile()
    {
        byte[] bytes = texture.EncodeToPNG();
        string path = Application.dataPath + "/SavedPaint.png";
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log("贴图已保存到：" + path);
    }

    public void Paint(Vector2 worldPos)
    {
        Paint(worldPos, brushColor);
    }

    public void Paint(Vector2 worldPos, Color paintColor)
    {
        // 使用SpriteRenderer的bounds进行坐标转换
        Bounds bounds = sr.bounds;

        // Debug.Log($"精灵bounds: min={bounds.min}, max={bounds.max}, size={bounds.size}");

        // 检查点击是否在精灵范围内
        if (!bounds.Contains(worldPos))
        {
            Debug.Log($"点击位置 {worldPos} 不在精灵范围内");
            return;
        }

        // 将世界坐标转换为相对于bounds的比例坐标(0-1)
        float normalizedX = (worldPos.x - bounds.min.x) / bounds.size.x;
        float normalizedY = (worldPos.y - bounds.min.y) / bounds.size.y;

        // 确保坐标在有效范围内
        normalizedX = Mathf.Clamp01(normalizedX);
        normalizedY = Mathf.Clamp01(normalizedY);

        // 转换为纹理像素坐标
        int texX = Mathf.FloorToInt(normalizedX * texture.width);
        int texY = Mathf.FloorToInt(normalizedY * texture.height);

        // 确保纹理坐标在有效范围内
        texX = Mathf.Clamp(texX, 0, texture.width - 1);
        texY = Mathf.Clamp(texY, 0, texture.height - 1);

        Debug.Log($"使用 {paintableTag} 标签上色，世界坐标: {worldPos}, 纹理坐标: ({texX}, {texY})");

        // 绘制圆形笔刷 - 恢复原始颜色
        bool pixelsChanged = false;
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                int px = texX + i;
                int py = texY + j;

                // 检查边界
                if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                {
                    // 计算距离，创建圆形笔刷
                    float dist = Mathf.Sqrt(i * i + j * j);
                    if (dist <= brushSize)
                    {
                        // 获取原始像素颜色
                        Color originalColor = originalTexture.GetPixel(px, py);

                        // 根据距离中心的远近创建渐变效果
                        float falloff = 1.0f - (dist / brushSize);
                        falloff = Mathf.Clamp01(falloff);

                        // 当前像素颜色
                        Color currentColor = texture.GetPixel(px, py);

                        // 混合原始颜色和当前颜色
                        Color newColor = Color.Lerp(currentColor, originalColor, falloff * 0.8f);

                        texture.SetPixel(px, py, newColor);
                        pixelsChanged = true;
                    }
                }
            }
        }

        if (pixelsChanged)
        {
            texture.Apply();
            Debug.Log($"颜色恢复完成！标签: {paintableTag}");
        }
        else
        {
            Debug.Log("没有像素被修改");
        }
    }

    // 重置为灰度模式
    public void ResetToGrayscale()
    {
        if (originalTexture != null)
        {
            Color[] originalPixels = originalTexture.GetPixels();
            Color[] grayscalePixels = ConvertToGrayscale(originalPixels);
            texture.SetPixels(grayscalePixels);
            texture.Apply();
            Debug.Log("重置为灰度模式");
        }
    }

    // 显示完整的原始图像
    public void ShowOriginal()
    {
        if (originalTexture != null)
        {
            Color[] originalPixels = originalTexture.GetPixels();
            texture.SetPixels(originalPixels);
            texture.Apply();
            Debug.Log("显示原始图像");
        }
    }
}