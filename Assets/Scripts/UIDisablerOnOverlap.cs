using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIDisablerOnOverlap : MonoBehaviour
{
    [Tooltip("Ҫ����Ƿ񱻸��ǵ�Ŀ�� UI")]
    public RectTransform otherUI;

    [Tooltip("�Լ���� UI �� CanvasGroup�����ڽ��ã�")]
    public CanvasGroup selfGroup;

    [Tooltip("X ��������������")]
    public float shrinkX = 160f;

    [Tooltip("Y ��������������")]
    public float shrinkY = 60f;

    RectTransform selfRect;
    bool isHidden = false;

    void Start()
    {
        selfRect = GetComponent<RectTransform>();
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        otherUI = canvas.transform.GetChild(1).GetComponent<RectTransform>();
    }

    void Update()
    {
        bool overlapped = RectTransformOverlap(selfRect, otherUI);

        if (overlapped && !isHidden)
        {
            isHidden = true;
            FadeOut();
        }
        else if (!overlapped && isHidden)
        {
            isHidden = false;
            FadeIn();
        }
    }

    void FadeOut()
    {
        selfGroup.DOFade(0f, 0.5f);
        selfGroup.interactable = false;
        selfGroup.blocksRaycasts = false;
    }

    void FadeIn()
    {
        selfGroup.DOFade(1f, 0.5f);
        selfGroup.interactable = true;
        selfGroup.blocksRaycasts = true;
    }

    bool RectTransformOverlap(RectTransform r1, RectTransform r2)
    {
        Vector3[] corners1 = new Vector3[4];
        Vector3[] corners2 = new Vector3[4];

        r1.GetWorldCorners(corners1);
        r2.GetWorldCorners(corners2);

        Rect rect1 = new Rect(corners1[0], corners1[2] - corners1[0]);
        Rect rect2 = new Rect(corners2[0], corners2[2] - corners2[0]);

        // ��С rect1
        rect1.xMin += shrinkX;
        rect1.xMax -= shrinkX;
        rect1.yMin += shrinkY;
        rect1.yMax -= shrinkY;

        return rect1.Overlaps(rect2);
    }
}
