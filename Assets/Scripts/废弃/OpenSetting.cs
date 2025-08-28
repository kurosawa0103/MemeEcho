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
        UnityEditor.EditorApplication.isPlaying = false; // �༭����ֹͣ��Ϸ
#else
        Application.Quit(); // �������˳���Ϸ
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
