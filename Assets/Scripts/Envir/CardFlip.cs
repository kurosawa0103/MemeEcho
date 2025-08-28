using UnityEngine;
using System.Collections;

public class CardFlip : MonoBehaviour
{
    public GameObject front; // 正面图
    public GameObject back;  // 背面图
    public float flipDuration = 0.5f;

    private bool isFront = true;
    private bool isFlipping = false;

    private Transform rootObject; // tag 为 Root 的唯一对象
    public string tagName = "Root";
    void Start()
    {
        GameObject rootGO = GameObject.FindGameObjectWithTag(tagName);
        if (rootGO != null)
        {
            rootObject = rootGO.transform;
        }
    }

    public void Flip()
    {
        if (isFlipping) return;
        StartCoroutine(FlipCard());
    }

    private IEnumerator FlipCard()
    {
        isFlipping = true;

        float time = 0f;
        float half = flipDuration / 2f;

        // 前半段：缩放 X 从 1 到 0
        while (time < half)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 0f, time / half);
            transform.localScale = new Vector3(scale, 1f, 1f);
            if (rootObject != null)
                rootObject.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        // 切换正反面
        isFront = !isFront;
        front.SetActive(isFront);
        back.SetActive(!isFront);

        time = 0f;

        // 后半段：缩放 X 从 0 到 1
        while (time < half)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(0f, 1f, time / half);
            transform.localScale = new Vector3(scale, 1f, 1f);
            if (rootObject != null)
                rootObject.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        isFlipping = false;
    }
}
