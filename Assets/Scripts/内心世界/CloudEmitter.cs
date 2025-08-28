using UnityEngine;
using System.Collections;

public class CloudEmitter : MonoBehaviour
{
    public GameObject cloudPrefab;         // 云朵预制体
    public float spawnInterval = 0.2f;     // 发射间隔
    public float moveSpeed = 1.5f;         // 云朵移动速度
    public float maxScale = 2f;            // 最大缩放
    public float growDuration = 1.5f;      // 缩放时间
    public float fadeDuration = 1.5f;      // 渐隐时间
    public float emissionDuration = 3f;    // 云朵发射持续时间（新增参数）

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
        Vector2 randomDir = Random.insideUnitCircle.normalized; // 2D 随机方向
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

        // 缩放 + 移动阶段
        while (timer < growDuration)
        {
            float t = timer / growDuration;
            cloud.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            cloud.transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // 保持最大尺寸，开始淡出
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
