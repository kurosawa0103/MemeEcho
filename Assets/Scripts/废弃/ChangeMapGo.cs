using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeMapGo : MonoBehaviour
{
    public string targetMapName;  // Ŀ�곡������
    public Slider progressBar;    // UI ������
    private bool isLoading = false; // ����Ƿ����ڼ���
    public GameObject loadingIcon;

    public Button startButton;
    public Button resetButton;

    public void ChangeGo()
    {
        if (!isLoading)  // ����Ƿ��Ѿ��ڼ���
        {
            isLoading = true;  // ���Ϊ���ڼ���
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
