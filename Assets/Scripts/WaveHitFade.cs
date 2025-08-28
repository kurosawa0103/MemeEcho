using UnityEngine;
using System.Collections;

public class WaveHitFade : MonoBehaviour
{
    public SpriteRenderer wetPatch; // ��ק�����ɫɫ��
    public float fadeDuration = 2f;

    private Coroutine fadeCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("���봥����: " + other.name);
        if (other.CompareTag("Wave"))
        {
            // ÿ�ν���ʱ������Ϊ��͸����
            Color c = wetPatch.color;
            c.a = 1f;
            wetPatch.color = c;

            // ���֮ǰ�н�������ͣ��
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            // ��ʼ����
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

        // ȷ����ȫ��ȥ
        c.a = 0f;
        wetPatch.color = c;
    }
}