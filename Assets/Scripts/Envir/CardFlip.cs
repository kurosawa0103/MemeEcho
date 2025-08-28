using UnityEngine;
using System.Collections;

public class CardFlip : MonoBehaviour
{
    public GameObject front; // ����ͼ
    public GameObject back;  // ����ͼ
    public float flipDuration = 0.5f;

    private bool isFront = true;
    private bool isFlipping = false;

    private Transform rootObject; // tag Ϊ Root ��Ψһ����
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

        // ǰ��Σ����� X �� 1 �� 0
        while (time < half)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 0f, time / half);
            transform.localScale = new Vector3(scale, 1f, 1f);
            if (rootObject != null)
                rootObject.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        // �л�������
        isFront = !isFront;
        front.SetActive(isFront);
        back.SetActive(!isFront);

        time = 0f;

        // ���Σ����� X �� 0 �� 1
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
