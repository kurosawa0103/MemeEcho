using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExit : MonoBehaviour
{
    public GameObject exitPanel;
    public OpenSetting openSetting;
    void Start()
    {
        
    }
    void OnMouseDown()
    {
        OpenExit();
    }
    public void OpenExit()
    {
        //if(openSetting.isOn)
        //{
       //     exitPanel.SetActive(true);
       // }

    }

    public void CloseExit()
    {
        exitPanel.SetActive(false);
    }
    public void ExitGame()
    {
        // 在编辑器模式下退出游戏时，Unity不会关闭应用程序，所以需要用 #if UNITY_EDITOR 来处理
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 在编辑模式下停止播放
#else
        Application.Quit(); // 在构建的版本中退出游戏
#endif
        Debug.Log("游戏已退出");
    }
}
