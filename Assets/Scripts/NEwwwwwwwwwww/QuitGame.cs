using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class QuitGame : MonoBehaviour
{
    public void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ShowPanel: Panel 为 null！");
        }
    }
    public void HidePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("HidePanel: Panel 为 null！");
        }
    }
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("LoadScene: 场景名称为空！");
        }
    }
}
