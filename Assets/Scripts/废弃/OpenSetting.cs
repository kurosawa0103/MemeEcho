using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSetting : MonoBehaviour
{
    public bool settingIsOn = false;
    public GameObject settingPanel;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 编辑器中停止游戏
#else
        Application.Quit(); // 构建后退出游戏
#endif
    }
    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }
    public void OpenSettingPanel()
    {
        if (!settingIsOn)
        {
            //radarChart.RefreshRadar();
            settingIsOn = true;
            settingPanel.SetActive(true);
        }
        else if (settingIsOn)
        {
            settingIsOn = false;
            settingPanel.SetActive(false);
        }

    }
}
