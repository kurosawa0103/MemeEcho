using UnityEngine;

public class ExpandUIOnTrigger : MonoBehaviour
{
    public Vector3 targetScale = new Vector3(2f, 2f, 1f);
    public float expandDuration = 2f;

    private Vector3 originalScale;
    private float timer = 0f;
    private bool expanding = false;

    void Start()
    {
        originalScale = transform.localScale;
    }
#if UNITY_EDITOR
    [ContextMenu("Test Expand")]
    void TestExpand()
    {
        StartExpand();
    }
#endif
    void Update()
    {
        if (expanding)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / expandDuration);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, EaseOutCubic(t));
        }
    }

    public void StartExpand()
    {
        timer = 0f;
        expanding = true;
    }

    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3);
    }
}