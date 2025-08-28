using UnityEngine;
using System.Collections;

public class CloudEmitter : MonoBehaviour
{
    public GameObject cloudPrefab;         // �ƶ�Ԥ����
    public float spawnInterval = 0.2f;     // ������
    public float moveSpeed = 1.5f;         // �ƶ��ƶ��ٶ�
    public float maxScale = 2f;            // �������
    public float growDuration = 1.5f;      // ����ʱ��
    public float fadeDuration = 1.5f;      // ����ʱ��
    public float emissionDuration = 3f;    // �ƶ䷢�����ʱ�䣨����������

    void Start()
    {
        StartCoroutine(SpawnClouds());
    }

    IEnumerator SpawnClouds()
    {
        float elapsed = 0f;
        while (elapsed < emissionDuration)
        {
            SpawnCloud();
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }
    }

    void SpawnCloud()
    {
        GameObject cloud = Instantiate(cloudPrefab, transform.position, Quaternion.identity);
        Vector2 randomDir = Random.insideUnitCircle.normalized; // 2D �������
        StartCoroutine(AnimateCloud(cloud, randomDir));
    }

    IEnumerator AnimateCloud(GameObject cloud, Vector2 direction)
    {
        SpriteRenderer sr = cloud.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("Cloud prefab missing SpriteRenderer.");
            yield break;
        }

        float timer = 0f;
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * maxScale;

        // ���� + �ƶ��׶�
        while (timer < growDuration)
        {
            float t = timer / growDuration;
            cloud.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            cloud.transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // �������ߴ磬��ʼ����
        timer = 0f;
        Color startColor = sr.color;
        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            sr.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            cloud.transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(cloud);
    }
}
