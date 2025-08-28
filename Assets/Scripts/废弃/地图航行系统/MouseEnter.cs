using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEnter : MonoBehaviour
{
    public Color hoverColor = Color.green; // 鼠标悬停时的颜色
    public Color disabledColor = Color.gray; // 禁用状态下的颜色
    private Color originalColor; // 原始颜色
    private SpriteRenderer spriteRenderer; // SpriteRenderer

    public bool isDisabled = false; // 用于标记对象是否禁用，手动控制

    void Start()
    {
        // 获取 SpriteRenderer 组件
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // 保存原始颜色
            originalColor = spriteRenderer.color;

            // 根据 isDisabled 来决定初始颜色
            if (isDisabled)
            {
                spriteRenderer.color = disabledColor;
            }
        }
    }

    // 鼠标进入区域时改变颜色
    void OnMouseEnter()
    {
        if (spriteRenderer != null && !isDisabled) // 确保对象未被禁用
        {
            spriteRenderer.color = hoverColor;
        }
    }

    // 鼠标离开区域时恢复原始颜色
    void OnMouseExit()
    {
        if (spriteRenderer != null && !isDisabled) // 确保对象未被禁用
        {
            spriteRenderer.color = originalColor;
        }
    }

    // 外部可以通过此方法手动禁用或启用
    public void SetDisabled(bool disabled)
    {
        isDisabled = disabled;

        if (spriteRenderer != null)
        {
            if (isDisabled)
            {
                spriteRenderer.color = disabledColor; // 禁用时颜色变为禁用色
            }
            else
            {
                spriteRenderer.color = originalColor; // 恢复原始颜色
            }
        }
    }
}
