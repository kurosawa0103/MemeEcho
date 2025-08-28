using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCycler : MonoBehaviour
{
    [Header("Sprite �л�����")]
    public List<Sprite> sprites;          // �洢Ҫѭ���л��� sprite �ز�
    public float switchInterval = 0.5f;   // �л�ʱ�������룩

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (sprites == null || sprites.Count == 0)
        {
            Debug.LogWarning("Sprite �б�Ϊ�գ�");
            enabled = false;
            return;
        }

        spriteRenderer.sprite = sprites[currentIndex];
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            timer = 0f;
            currentIndex = (currentIndex + 1) % sprites.Count;
            spriteRenderer.sprite = sprites[currentIndex];
        }
    }
}