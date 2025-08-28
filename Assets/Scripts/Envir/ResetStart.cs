using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening; // ���������� DOTween �����ռ�

public class ResetStart : MonoBehaviour
{
    private IGameDataHandler gameDataHandler = new GameDataToJson();
    private IItemHandler itemHandler = new ItemToJson();
    private IEventStateDataHandler eventStateDataHandler = new JsonGenerator();

    public string sceneName; // Ҫ���صĳ�����
    public Image blackScreenImage; // ��Ļ UI �� Image ���
    public float fadeDuration = 2f; // ��Ļ����ʱ��

    private AsyncOperation preloadOperation;

    void Start()
    {


        // Ԥ���س���
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

            // ִ�� DOTween ����
            blackScreenImage.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                // ������ɺ󼤻��
                if (preloadOperation != null && preloadOperation.progress >= 0.9f)
                {
                    preloadOperation.allowSceneActivation = true;
                }
                else
                {
                    Debug.LogWarning("Ԥ����δ��ɣ�ֱ�Ӽ��س�����");
                    SceneManager.LoadScene(sceneName);
                }
            });
        }
        else
        {
            // ���û�к�Ļ��Ҳִ��Ĭ�ϳ����л�
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

            // ִ�� DOTween ����
            blackScreenImage.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                // ������ɺ󼤻��
                if (preloadOperation != null && preloadOperation.progress >= 0.9f)
                {
                    preloadOperation.allowSceneActivation = true;
                }
                else
                {
                    Debug.LogWarning("Ԥ����δ��ɣ�ֱ�Ӽ��س�����");
                    SceneManager.LoadScene(sceneName);
                }
            });
        }
        else
        {
            // ���û�к�Ļ��Ҳִ��Ĭ�ϳ����л�
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
