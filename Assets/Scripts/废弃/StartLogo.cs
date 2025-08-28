using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartLogo : MonoBehaviour
{
    //logo����
    public Image spaceBunny;
    public Image backGround;

    //��ת��ͼ
    public string sceneName;

    private Coroutine logoCoroutine; // ���ڴ洢 Coroutine ������

    private void Start()
    {
        logoCoroutine = StartCoroutine(ShowLogo());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // �����������ʱ
        {
            if (logoCoroutine != null) // ��� Coroutine ��������
            {
                StopCoroutine(logoCoroutine); // ֹͣ Coroutine
                spaceBunny.DOFade(0, 0); // �������� spaceBunny
                SceneManager.LoadScene(sceneName); // ���س���
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
