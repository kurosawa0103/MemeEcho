using UnityEngine;

public class MaskVisibilityController : MonoBehaviour
{
    public SpriteRenderer targetRenderer;  // 目标物体的 Sprite Renderer
    public SpriteMask mask1;               // 大圆 Sprite Mask
    public SpriteMask mask2;               // 小圆 Sprite Mask

    void Start()
    {
        // 确保物体在大圆 mask1 内显示，但在小圆 mask2 内不可见
        targetRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        // 设置物体只在 mask1 中可见，在 mask2 中不可见
        SetMaskVisibility(targetRenderer, mask1, true);
        SetMaskVisibility(targetRenderer, mask2, false);
    }

    void SetMaskVisibility(SpriteRenderer renderer, SpriteMask mask, bool isVisible)
    {
        // 根据需要的遮罩交互行为控制物体的渲染
        if (isVisible)
        {
            // 将目标物体设置为可见状态
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        else
        {
            // 将目标物体设置为不可见
            renderer.maskInteraction = SpriteMaskInteraction.None;
        }
    }
}
