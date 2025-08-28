using UnityEngine;
using System.Collections;

public class WaveHitFade : MonoBehaviour
{
    public SpriteRenderer wetPatch; // 拖拽你的深色色块
    public float fadeDuration = 2f;

    private Coroutine fadeCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("进入触发器: " + other.name);
        if (other.CompareTag("Wave"))
        {
            // 每次进入时立刻设为满透明度
            Color c = wetPatch.color;
            c.a = 1f;
            wetPatch.color = c;

            // 如果之前有渐隐，先停掉
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            // 开始渐隐
            fadeCoroutine = StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color c = wetPatch.color;
        float startAlpha = c.a;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            wetPatch.color = c;
            yield return null;
        }

        // 确保完全隐去
        c.a = 0f;
        wetPatch.color = c;
    }
}