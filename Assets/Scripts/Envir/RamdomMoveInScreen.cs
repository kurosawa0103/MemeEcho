using UnityEngine;

public class RandomMoveInScreen : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float bounceAngleVariance = 30f;

    public bool enableRotation = false;
    public float rotationSpeed = 25f; // Ã¿ÃëÐý×ª½Ç¶È

    private Vector3 moveDirection;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        moveDirection = GetRandomDirection();
    }

    void Update()
    {
        float depthFromCamera = Vector3.Dot(
            transform.position - cam.transform.position,
            cam.transform.forward
        );

        Vector3 worldMin = cam.ViewportToWorldPoint(new Vector3(0, 0, depthFromCamera));
        Vector3 worldMax = cam.ViewportToWorldPoint(new Vector3(1, 1, depthFromCamera));

        Vector3 pos = transform.position;
        pos += moveDirection * moveSpeed * Time.deltaTime;

        float margin = 0.01f;

        if (pos.x < worldMin.x)
        {
            pos.x = worldMin.x + margin;
            moveDirection.x *= -1;
            moveDirection = RandomizeDirection(moveDirection);
        }
        else if (pos.x > worldMax.x)
        {
            pos.x = worldMax.x - margin;
            moveDirection.x *= -1;
            moveDirection = RandomizeDirection(moveDirection);
        }

        if (pos.y < worldMin.y)
        {
            pos.y = worldMin.y + margin;
            moveDirection.y *= -1;
            moveDirection = RandomizeDirection(moveDirection);
        }
        else if (pos.y > worldMax.y)
        {
            pos.y = worldMax.y - margin;
            moveDirection.y *= -1;
            moveDirection = RandomizeDirection(moveDirection);
        }

        transform.position = pos;

        if (enableRotation)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    Vector3 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;
    }

    Vector3 RandomizeDirection(Vector3 dir)
    {
        float angle = Random.Range(-bounceAngleVariance, bounceAngleVariance);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        return rotation * dir;
    }
}
