using UnityEngine;
using UnityEngine.UI;

public class ShakeAlpha : MonoBehaviour
{
    [Header("Ŀ�� Image ���")]
    public Image targetImage;

    [Header("��ק�������")]
    public DraggableUIWindow window;

    [Tooltip("�ж�Ϊ�ƶ�����С���루���أ�")]
    public float moveDistanceThreshold = 30f;

    [Header("�ƶ�����͸����")]
    public float moveAlphaDecrease = 0.2f;

    [Header("͸���Ȼָ�����")]
    public float alphaRecoveryPerSecond = 0.5f;

    [Header("�Ƿ���͸����Ϊ0�����")]
    public bool disableWhenTransparent = true;

    private float currentAlpha = 1f;
    private bool isDisabled = false;

    private Vector2 lastPosition;

    void Awake()
    {
        // �Զ���ȡ targetImage
        if (targetImage == null)
        {
            GameObject boxBorder = GameObject.FindGameObjectWithTag("BoxBorder");
            if (boxBorder != null && boxBorder.transform.childCount > 0)
            {
                Transform firstChild = boxBorder.transform.GetChild(0);
                foreach (Transform child in firstChild)
                {
                    if (child.name == "��Ļ")
                    {
                        targetImage = child.GetComponent<Image>();
                        break;
                    }
                }
            }
        }


        // �Զ���ȡ DraggableUIWindow����ѡ��
        if (window == null)
        {
            window = GameObject.FindGameObjectWithTag("Root").transform.GetComponent<DraggableUIWindow>();
        }

        currentAlpha = 1f;
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(true);
            UpdateImageAlpha();
        }

        lastPosition = window.transform.position; // ��ʼ������λ��
    }
    void OnEnable()
    {
        if (window != null)
        {
            lastPosition = window.transform.position; // ȷ����ǰ������λ��
        }

        isDisabled = false;
        currentAlpha = 1f;

        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(true);
            UpdateImageAlpha();
        }
    }
    void Update()
    {
        if (isDisabled || targetImage == null || window == null) return;

        // ��ȡ��ǰ���ڵ�λ��
        Vector2 currentPosition = window.transform.position;

        // ��鴰���Ƿ��ƶ�����ָ���ľ���
        if (Vector2.Distance(currentPosition, lastPosition) > moveDistanceThreshold)
        {
            // ��������ƶ��ˣ�����͸����
            currentAlpha -= moveAlphaDecrease;
            currentAlpha = Mathf.Clamp01(currentAlpha); // ȷ��͸������0��1֮��
            UpdateImageAlpha();

            if (disableWhenTransparent && currentAlpha <= 0f)
            {
                isDisabled = true;
                targetImage.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }
        else if (currentAlpha < 1f)
        {
            // �������û���ƶ����ָ�͸����
            currentAlpha += alphaRecoveryPerSecond * Time.deltaTime;
            currentAlpha = Mathf.Clamp01(currentAlpha);
            UpdateImageAlpha();

            if (!targetImage.gameObject.activeSelf && currentAlpha > 0f)
                targetImage.gameObject.SetActive(true);
        }

        // ��������λ��
        lastPosition = currentPosition;
    }

    public void ResetAlpha()
    {
        isDisabled = false;
        currentAlpha = 1f;
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(true);
            UpdateImageAlpha();
        }

        lastPosition = window.transform.position; // ��������λ��
    }

    private void UpdateImageAlpha()
    {
        if (targetImage != null)
        {
            var color = targetImage.color;
            color.a = currentAlpha;
            targetImage.color = color;
        }
    }
}
