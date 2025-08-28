using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class MeteorSpawner : MonoBehaviour
{
    [LabelText("流星的预制件")]
    public GameObject meteorPrefab;

    [LabelText("生成间隔")]
    public Vector2 intervalRange = new Vector2(1f, 5f);

    [LabelText("生成范围的尺寸")]
    public Vector3 spawnAreaSize = new Vector3(10f, 10f, 10f); // 修改为 Vector3

    [LabelText("流星的移动方向")]
    public Vector3 moveDirection = Vector3.down;

    [LabelText("移动方向随机范围")]
    public Vector3 randomRange = Vector3.zero;

    [LabelText("每次生成流星的数量")]
    public Vector2 meteorCountRange = new Vector2(1, 5);

    [LabelText("流星的移动速度范围")]
    public Vector2 meteorSpeedRange = new Vector2(3f, 8f);

    [LabelText("垂直Y轴最小夹角")]
    public float minVerticalAngle = 30f; // 最小竖直方向夹角为30度

    [LabelText("垂直Y轴最大夹角")]
    public float maxVerticalAngle = 60f; // 最大竖直方向夹角为60度


    private float nextSpawnTime;
    public bool canMeteor; //是否产生流星

    void Start()
    {

        // 设置下一次生成流星的时间
        nextSpawnTime = Time.time + Random.Range(intervalRange.x, intervalRange.y);

        // 开始生成流星的协程
        StartCoroutine(SpawnMeteorCoroutine());
    }

    IEnumerator SpawnMeteorCoroutine()
    {
        while (canMeteor)
        {
            if (Time.time >= nextSpawnTime)
            {
                // 随机生成流星数量
                int meteorCount = Random.Range((int)meteorCountRange.x, (int)meteorCountRange.y + 1);
                for (int i = 0; i < meteorCount; i++)
                {
                    // 生成流星
                    SpawnMeteor();
                    yield return new WaitForSeconds(Random.Range(intervalRange.x, intervalRange.y));
                }
                // 更新下一次生成流星的时间
                nextSpawnTime = Time.time + Random.Range(intervalRange.x, intervalRange.y);
            }
            yield return null;
        }
    }

    void SpawnMeteor()
    {
        // 在生成范围内随机位置生成流星
        Vector3 spawnPosition = transform.position + new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f),
            Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)); // 修改生成位置的 Z 范围

        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity,transform);

        // 设置流星的移动速度和方向
        MeteorController meteorController = meteor.GetComponent<MeteorController>();
        if (meteorController != null)
        {
            float randomSpeed = Random.Range(meteorSpeedRange.x, meteorSpeedRange.y);
            meteorController.speed = randomSpeed;

            // 随机生成竖直方向夹角
            float verticalAngle = Random.Range(minVerticalAngle, maxVerticalAngle);

            // 计算竖直方向夹角的单位向量
            Vector3 verticalDirection = Quaternion.AngleAxis(verticalAngle, Vector3.Cross(moveDirection, Vector3.down)) * Vector3.down;

            // 计算随机移动方向
            float randomX = Random.Range(-randomRange.x, randomRange.x);
            float randomY = Mathf.Clamp(Random.Range(-randomRange.y, randomRange.y), Mathf.NegativeInfinity, 0f);
            float randomZ = Random.Range(-randomRange.z, randomRange.z);

            Vector3 randomDirection = new Vector3(
                verticalDirection.x + randomX,
                verticalDirection.y + randomY,
                verticalDirection.z + randomZ);

            // 设置流星的移动方向
            meteorController.direction = randomDirection;
        }
    }

    // 在编辑器中绘制生成范围的可视化区域
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
