using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fungus;
public class TunerController : MonoBehaviour
{
    [Header("滑块和目标值（控制文字乱码）")]
    public Slider sliderA;
    [Range(0f, 1f)] public float targetA = 0.3f;

    [Header("黑幕控制对象（基于距离）")]
    public Transform movingObject;     // 需要判断的物体，例如波中心、角色等
    public Transform targetArea;       // 目标区域
    public float maxFadeDistance = 5f; // 距离大于此值时黑幕完全不透明
    public float minFadeDistance = 1f; // 距离小于此值时黑幕完全透明

    [Header("黑幕图像")]
    public Image blackoutImage;
    public float fadeSpeed = 2f;
    private float currentAlpha = 1f;

    [Header("乱码文字")]
    public TextMeshProUGUI glitchText;
    [TextArea] public string originalText = "很久很久以前，这里是一座微风山丘，这里没有一个人";

    [Header("文字乱码控制参数")]
    public float glitchUpdateInterval = 0.2f; // 刷新频率
    private float glitchTimer = 0f;
    private string cachedGlitchedText = "";

    [Range(0f, 1f)] public float maxGlitchRatio = 0.8f;
    [Range(0f, 0.5f)] public float glitchStartRange = 0.2f;
    [Range(0f, 0.1f)] public float fullCorrectRange = 0.01f;

    private char[] glitchChars = new char[] { '█', '@', '#', '$', '%', '&', '?', '*', '！', 'x', 'Ω', '§', 'Δ' };

    public Flowchart fungusFlowchart;
    public bool hasReportedSuccess = false;

    private void Start()
    {
            
    }
    void Update()
    {
        // 黑幕控制（基于世界坐标距离）
        if (movingObject != null && targetArea != null)
        {
            //  正常黑幕计算
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
            // 目标点未配置：黑幕最大
            currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, Time.deltaTime * fadeSpeed);
        }

        SetBlackoutAlpha(currentAlpha);

        // 文字乱码控制（仅使用 sliderA）
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
        // 检查是否达成：黑幕透明 + 乱码为0 + 仅触发一次
        if (!hasReportedSuccess)
        {
            if (currentAlpha < 0.1f&& glitchRatio < 0.1f)
            {
                fungusFlowchart.ExecuteBlock(targetArea.GetComponent<WaveTargetParam>().successKey);
                hasReportedSuccess = true;
                Debug.Log("[TunerController] Fungus Block 调用完毕");
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
