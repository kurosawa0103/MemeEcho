using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fungus;
public class TunerController : MonoBehaviour
{
    [Header("�����Ŀ��ֵ�������������룩")]
    public Slider sliderA;
    [Range(0f, 1f)] public float targetA = 0.3f;

    [Header("��Ļ���ƶ��󣨻��ھ��룩")]
    public Transform movingObject;     // ��Ҫ�жϵ����壬���粨���ġ���ɫ��
    public Transform targetArea;       // Ŀ������
    public float maxFadeDistance = 5f; // ������ڴ�ֵʱ��Ļ��ȫ��͸��
    public float minFadeDistance = 1f; // ����С�ڴ�ֵʱ��Ļ��ȫ͸��

    [Header("��Ļͼ��")]
    public Image blackoutImage;
    public float fadeSpeed = 2f;
    private float currentAlpha = 1f;

    [Header("��������")]
    public TextMeshProUGUI glitchText;
    [TextArea] public string originalText = "�ܾúܾ���ǰ��������һ��΢��ɽ������û��һ����";

    [Header("����������Ʋ���")]
    public float glitchUpdateInterval = 0.2f; // ˢ��Ƶ��
    private float glitchTimer = 0f;
    private string cachedGlitchedText = "";

    [Range(0f, 1f)] public float maxGlitchRatio = 0.8f;
    [Range(0f, 0.5f)] public float glitchStartRange = 0.2f;
    [Range(0f, 0.1f)] public float fullCorrectRange = 0.01f;

    private char[] glitchChars = new char[] { '��', '@', '#', '$', '%', '&', '?', '*', '��', 'x', '��', '��', '��' };

    public Flowchart fungusFlowchart;
    public bool hasReportedSuccess = false;

    private void Start()
    {
            
    }
    void Update()
    {
        // ��Ļ���ƣ���������������룩
        if (movingObject != null && targetArea != null)
        {
            //  ������Ļ����
            float distance = Vector3.Distance(movingObject.position, targetArea.position);

            if (distance > maxFadeDistance)
            {
                currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, Time.deltaTime * fadeSpeed);
            }
            else if (distance <= minFadeDistance)
            {
                currentAlpha = Mathf.MoveTowards(currentAlpha, 0f, Time.deltaTime * fadeSpeed);
            }
            else
            {
                float t = Mathf.InverseLerp(maxFadeDistance, minFadeDistance, distance);
                float targetAlpha = Mathf.Lerp(1f, 0f, t);
                currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
            }
        }
        else
        {
            // Ŀ���δ���ã���Ļ���
            currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, Time.deltaTime * fadeSpeed);
        }

        SetBlackoutAlpha(currentAlpha);

        // ����������ƣ���ʹ�� sliderA��
        float diffA = Mathf.Abs(sliderA.value - targetA);
        float glitchRatio;

        if (diffA < glitchStartRange)
        {
            glitchRatio = Mathf.InverseLerp(glitchStartRange, fullCorrectRange, diffA);
            glitchRatio = 1f - glitchRatio;
        }
        else
        {
            glitchRatio = maxGlitchRatio;
        }

        glitchTimer -= Time.deltaTime;
        if (glitchTimer <= 0f)
        {
            glitchTimer = glitchUpdateInterval;
            cachedGlitchedText = GenerateGlitchText(originalText, glitchRatio);
        }

        glitchText.text = cachedGlitchedText;
        // ����Ƿ��ɣ���Ļ͸�� + ����Ϊ0 + ������һ��
        if (!hasReportedSuccess)
        {
            if (currentAlpha < 0.1f&& glitchRatio < 0.1f)
            {
                fungusFlowchart.ExecuteBlock(targetArea.GetComponent<WaveTargetParam>().successKey);
                hasReportedSuccess = true;
                Debug.Log("[TunerController] Fungus Block �������");
            }
        }

    }

    void SetBlackoutAlpha(float alpha)
    {
        if (blackoutImage != null)
        {
            Color c = blackoutImage.color;
            c.a = alpha;
            blackoutImage.color = c;
        }
    }

    string GenerateGlitchText(string baseText, float glitchRatio)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (char c in baseText)
        {
            if (char.IsWhiteSpace(c))
            {
                sb.Append(c);
                continue;
            }

            if (Random.value < glitchRatio)
            {
                char glitchChar = glitchChars[Random.Range(0, glitchChars.Length)];
                sb.Append(glitchChar);
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
