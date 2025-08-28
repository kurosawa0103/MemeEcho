using UnityEngine;
using UnityEngine.UI;
using TMPro;  

public class SettingMenu : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Button applyButton;

    [Header("Camera")]
    public Camera mainCamera;
    public RenderTexture rt1920;
    public RenderTexture rt2560;

    [Header("Quad")]
    public Renderer quadRenderer;
    public Material mat1920;
    public Material mat2560;

    private void Start()
    {
        // 初始化下拉菜单
        resolutionDropdown.ClearOptions();
        resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1920 x 1080"));
        resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("2560 x 1440"));
        resolutionDropdown.value = 0;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;
    }

    public void ApplySettings()
    {
        int index = resolutionDropdown.value;
        bool isFull = fullscreenToggle.isOn;

        if (index == 0) // 1920
        {
            Screen.SetResolution(1920, 1080, isFull);
            mainCamera.targetTexture = rt1920;
            quadRenderer.material = mat1920;
        }
        else if (index == 1) // 2560
        {
            Screen.SetResolution(2560, 1440, isFull);
            mainCamera.targetTexture = rt2560;
            quadRenderer.material = mat2560;
        }

        Debug.Log($"应用分辨率 {(index == 0 ? "1920x1080" : "2560x1440")} | 全屏 {isFull}");
    }
}
