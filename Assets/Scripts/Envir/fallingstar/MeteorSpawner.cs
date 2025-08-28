using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class MeteorSpawner : MonoBehaviour
{
    [LabelText("���ǵ�Ԥ�Ƽ�")]
    public GameObject meteorPrefab;

    [LabelText("���ɼ��")]
    public Vector2 intervalRange = new Vector2(1f, 5f);

    [LabelText("���ɷ�Χ�ĳߴ�")]
    public Vector3 spawnAreaSize = new Vector3(10f, 10f, 10f); // �޸�Ϊ Vector3

    [LabelText("���ǵ��ƶ�����")]
    public Vector3 moveDirection = Vector3.down;

    [LabelText("�ƶ����������Χ")]
    public Vector3 randomRange = Vector3.zero;

    [LabelText("ÿ���������ǵ�����")]
    public Vector2 meteorCountRange = new Vector2(1, 5);

    [LabelText("���ǵ��ƶ��ٶȷ�Χ")]
    public Vector2 meteorSpeedRange = new Vector2(3f, 8f);

    [LabelText("��ֱY����С�н�")]
    public float minVerticalAngle = 30f; // ��С��ֱ����н�Ϊ30��

    [LabelText("��ֱY�����н�")]
    public float maxVerticalAngle = 60f; // �����ֱ����н�Ϊ60��


    private float nextSpawnTime;
    public bool canMeteor; //�Ƿ��������

    void Start()
    {

        // ������һ���������ǵ�ʱ��
        nextSpawnTime = Time.time + Random.Range(intervalRange.x, intervalRange.y);

        // ��ʼ�������ǵ�Э��
        StartCoroutine(SpawnMeteorCoroutine());
    }

    IEnumerator SpawnMeteorCoroutine()
    {
        while (canMeteor)
        {
            if (Time.time >= nextSpawnTime)
            {
                // ���������������
                int meteorCount = Random.Range((int)meteorCountRange.x, (int)meteorCountRange.y + 1);
                for (int i = 0; i < meteorCount; i++)
                {
                    // ��������
                    SpawnMeteor();
                    yield return new WaitForSeconds(Random.Range(intervalRange.x, intervalRange.y));
                }
                // ������һ���������ǵ�ʱ��
                nextSpawnTime = Time.time + Random.Range(intervalRange.x, intervalRange.y);
            }
            yield return null;
        }
    }

    void SpawnMeteor()
    {
        // �����ɷ�Χ�����λ����������
        Vector3 spawnPosition = transform.position + new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f),
            Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)); // �޸�����λ�õ� Z ��Χ

        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity,transform);

        // �������ǵ��ƶ��ٶȺͷ���
        MeteorController meteorController = meteor.GetComponent<MeteorController>();
        if (meteorController != null)
        {
            float randomSpeed = Random.Range(meteorSpeedRange.x, meteorSpeedRange.y);
            meteorController.speed = randomSpeed;

            // ���������ֱ����н�
            float verticalAngle = Random.Range(minVerticalAngle, maxVerticalAngle);

            // ������ֱ����нǵĵ�λ����
            Vector3 verticalDirection = Quaternion.AngleAxis(verticalAngle, Vector3.Cross(moveDirection, Vector3.down)) * Vector3.down;

            // ��������ƶ�����
            float randomX = Random.Range(-randomRange.x, randomRange.x);
            float randomY = Mathf.Clamp(Random.Range(-randomRange.y, randomRange.y), Mathf.NegativeInfinity, 0f);
            float randomZ = Random.Range(-randomRange.z, randomRange.z);

            Vector3 randomDirection = new Vector3(
                verticalDirection.x + randomX,
                verticalDirection.y + randomY,
                verticalDirection.z + randomZ);

            // �������ǵ��ƶ�����
            meteorController.direction = randomDirection;
        }
    }

    // �ڱ༭���л������ɷ�Χ�Ŀ��ӻ�����
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
