using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeMapGo : MonoBehaviour
{
    public string targetMapName;  // 目标场景名称
    public Slider progressBar;    // UI 进度条
    private bool isLoading = false; // 标记是否正在加载
    public GameObject loadingIcon;

    public Button startButton;
    public Button resetButton;

    public void ChangeGo()
    {
        if (!isLoading)  // 检查是否已经在加载
        {
            isLoading = true;  // 标记为正在加载
            loadingIcon.SetActive(true);
            startButton.interactable = false;
            resetButton.interactable = false;
            StartCoroutine(LoadSceneWithProgress());
        }
    }

    IEnumerator LoadSceneWithProgress()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetMapName);
        operation.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            displayedProgress = Mathf.Lerp(displayedProgress, targetProgress, Time.deltaTime * 2);
            progressBar.value = displayedProgress;

            if (operation.progress >= 0.9f && displayedProgress >= 0.99f)
            {
                progressBar.value = 1f;
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
