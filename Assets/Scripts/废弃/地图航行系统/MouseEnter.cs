using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEnter : MonoBehaviour
{
    public Color hoverColor = Color.green; // �����ͣʱ����ɫ
    public Color disabledColor = Color.gray; // ����״̬�µ���ɫ
    private Color originalColor; // ԭʼ��ɫ
    private SpriteRenderer spriteRenderer; // SpriteRenderer

    public bool isDisabled = false; // ���ڱ�Ƕ����Ƿ���ã��ֶ�����

    void Start()
    {
        // ��ȡ SpriteRenderer ���
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // ����ԭʼ��ɫ
            originalColor = spriteRenderer.color;

            // ���� isDisabled ��������ʼ��ɫ
            if (isDisabled)
            {
                spriteRenderer.color = disabledColor;
            }
        }
    }

    // ����������ʱ�ı���ɫ
    void OnMouseEnter()
    {
        if (spriteRenderer != null && !isDisabled) // ȷ������δ������
        {
            spriteRenderer.color = hoverColor;
        }
    }

    // ����뿪����ʱ�ָ�ԭʼ��ɫ
    void OnMouseExit()
    {
        if (spriteRenderer != null && !isDisabled) // ȷ������δ������
        {
            spriteRenderer.color = originalColor;
        }
    }

    // �ⲿ����ͨ���˷����ֶ����û�����
    public void SetDisabled(bool disabled)
    {
        isDisabled = disabled;

        if (spriteRenderer != null)
        {
            if (isDisabled)
            {
                spriteRenderer.color = disabledColor; // ����ʱ��ɫ��Ϊ����ɫ
            }
            else
            {
                spriteRenderer.color = originalColor; // �ָ�ԭʼ��ɫ
            }
        }
    }
}
