using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartLogo : MonoBehaviour
{
    //logo内容
    public Image spaceBunny;
    public Image backGround;

    //跳转地图
    public string sceneName;

    private Coroutine logoCoroutine; // 用于存储 Coroutine 的引用

    private void Start()
    {
        logoCoroutine = StartCoroutine(ShowLogo());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 当鼠标左键点击时
        {
            if (logoCoroutine != null) // 如果 Coroutine 正在运行
            {
                StopCoroutine(logoCoroutine); // 停止 Coroutine
                spaceBunny.DOFade(0, 0); // 立即隐藏 spaceBunny
                SceneManager.LoadScene(sceneName); // 加载场景
            }
        }
    }

    IEnumerator ShowLogo()
    {
        yield return new WaitForSeconds(1f);
        spaceBunny.DOFade(1, 1f);
        yield return new WaitForSeconds(3f);
        spaceBunny.DOFade(0, 1f);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(sceneName);
    }
}
