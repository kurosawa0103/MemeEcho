using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCycler : MonoBehaviour
{
    [Header("Sprite 切换设置")]
    public List<Sprite> sprites;          // 存储要循环切换的 sprite 素材
    public float switchInterval = 0.5f;   // 切换时间间隔（秒）

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (sprites == null || sprites.Count == 0)
        {
            Debug.LogWarning("Sprite 列表为空！");
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