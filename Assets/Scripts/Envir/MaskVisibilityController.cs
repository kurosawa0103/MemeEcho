using UnityEngine;

public class MaskVisibilityController : MonoBehaviour
{
    public SpriteRenderer targetRenderer;  // Ŀ������� Sprite Renderer
    public SpriteMask mask1;               // ��Բ Sprite Mask
    public SpriteMask mask2;               // СԲ Sprite Mask

    void Start()
    {
        // ȷ�������ڴ�Բ mask1 ����ʾ������СԲ mask2 �ڲ��ɼ�
        targetRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        // ��������ֻ�� mask1 �пɼ����� mask2 �в��ɼ�
        SetMaskVisibility(targetRenderer, mask1, true);
        SetMaskVisibility(targetRenderer, mask2, false);
    }

    void SetMaskVisibility(SpriteRenderer renderer, SpriteMask mask, bool isVisible)
    {
        // ������Ҫ�����ֽ�����Ϊ�����������Ⱦ
        if (isVisible)
        {
            // ��Ŀ����������Ϊ�ɼ�״̬
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        else
        {
            // ��Ŀ����������Ϊ���ɼ�
            renderer.maskInteraction = SpriteMaskInteraction.None;
        }
    }
}
