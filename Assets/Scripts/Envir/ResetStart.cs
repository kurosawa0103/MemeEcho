using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening; // 别忘了引入 DOTween 命名空间

public class ResetStart : MonoBehaviour
{
    private IGameDataHandler gameDataHandler = new GameDataToJson();
    private IItemHandler itemHandler = new ItemToJson();
    private IEventStateDataHandler eventStateDataHandler = new JsonGenerator();

    public string sceneName; // 要加载的场景名
    public Image blackScreenImage; // 黑幕 UI 的 Image 组件
    public float fadeDuration = 2f; // 黑幕淡入时长

    private AsyncOperation preloadOperation;

    void Start()
    {


        // 预加载场景
        preloadOperation = SceneManager.LoadSceneAsync(sceneName);
        preloadOperation.allowSceneActivation = false;
    }

    public void StartNewGame()
    {
        itemHandler.ClearInventory();
        gameDataHandler.ClearGameData();
        eventStateDataHandler.ClearEventStateData();
        if (blackScreenImage != null)
        {
            blackScreenImage.gameObject.SetActive(true);

            // 执行 DOTween 淡入
            blackScreenImage.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                // 淡入完成后激活场景
                if (preloadOperation != null && preloadOperation.progress >= 0.9f)
                {
                    preloadOperation.allowSceneActivation = true;
                }
                else
                {
                    Debug.LogWarning("预加载未完成，直接加载场景。");
                    SceneManager.LoadScene(sceneName);
                }
            });
        }
        else
        {
            // 如果没有黑幕，也执行默认场景切换
            if (preloadOperation != null && preloadOperation.progress >= 0.9f)
            {
                preloadOperation.allowSceneActivation = true;
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
    public void ContinueGame()
    {
        if (blackScreenImage != null)
        {
            blackScreenImage.gameObject.SetActive(true);

            // 执行 DOTween 淡入
            blackScreenImage.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                // 淡入完成后激活场景
                if (preloadOperation != null && preloadOperation.progress >= 0.9f)
                {
                    preloadOperation.allowSceneActivation = true;
                }
                else
                {
                    Debug.LogWarning("预加载未完成，直接加载场景。");
                    SceneManager.LoadScene(sceneName);
                }
            });
        }
        else
        {
            // 如果没有黑幕，也执行默认场景切换
            if (preloadOperation != null && preloadOperation.progress >= 0.9f)
            {
                preloadOperation.allowSceneActivation = true;
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
