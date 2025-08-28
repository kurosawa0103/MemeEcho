using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeChildren : MonoBehaviour
{
    public List<string> targetTags = new List<string>(); // �����ö��Ŀ��Tag
    public float fadeDuration = 1f;  // �����͵����ʱ��
    public Transform parent1;  // ��һ��������
    public Transform parent2;  // �ڶ���������
    private bool isFading = false; // ��ֹ�ظ��л�

    // ���ñ��������Ƶ��ʱִ���ĸ�����
    public bool executeFadeInOut1 = true;  // ������ʱִ�� FadeInOutSequence
    public bool executeFadeInOut2 = true;  // �Ҽ����ʱִ�� FadeInOutSequenceReverse
    public EventMaker eventMaker;
    public OpenCameraList openCameraList;
    private void Start()
    {
        // ��ʼ������������2��������͸��������Ϊ0
        List<SpriteRenderer> targetRenderers2 = GetTargetRenderers(parent2);
        parent2.gameObject.SetActive(false);
        foreach (var renderer in targetRenderers2)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0f); // ����͸����Ϊ0
        }
    }

    private void Update()
    {
        // ����������¼�
       
    }
    private void OnMouseDown()
    {
        if (!isFading && executeFadeInOut1 && openCameraList.isOn)
        {
            FadeInOutSequence();
        }

        if (!isFading && executeFadeInOut2 && openCameraList.isOn)
        {
            FadeInOutSequenceReverse();
        }
    }
    private void FadeInOutSequence()
    {
        if (isFading) return; // ��������л�����ִ��
        isFading = true; // �������ִ���л�

        List<SpriteRenderer> targetRenderers1 = GetTargetRenderers(parent1);
        List<SpriteRenderer> targetRenderers2 = GetTargetRenderers(parent2);

        // ���� parent1 ��������
        foreach (var renderer in targetRenderers1)
        {
            renderer.DOFade(0f, fadeDuration);
        }

        // �ȴ�������ɺ����� parent1����ʾ parent2��������
        DOVirtual.DelayedCall(fadeDuration, () =>
        {
            parent1.gameObject.SetActive(false);
            parent2.gameObject.SetActive(true);

            foreach (Transform child in parent1.transform.GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag("Event"))
                {
                    eventMaker = child.GetComponent<EventMaker>();
                    if (eventMaker != null)
                    {
                        eventMaker.RefreshEvent(); // ˢ���¼�
                        //Debug.Log("ˢ���¼�");
                        break; // �ҵ�������ѭ��
                    }
                }
            }
            foreach (var renderer in targetRenderers2)
            {
                renderer.DOFade(1f, fadeDuration);
            }

            // �ȴ�������ɺ�������һ���л�
            DOVirtual.DelayedCall(fadeDuration, () => isFading = false);
        });
    }

    private void FadeInOutSequenceReverse()
    {
        if (isFading) return; // ��������л�����ִ��
        isFading = true; // �������ִ���л�

        List<SpriteRenderer> targetRenderers1 = GetTargetRenderers(parent1);
        List<SpriteRenderer> targetRenderers2 = GetTargetRenderers(parent2);

        // ���� parent2 ��������
        foreach (var renderer in targetRenderers2)
        {
            renderer.DOFade(0f, fadeDuration);
        }

        // �ȴ�������ɺ����� parent2����ʾ parent1��������
        DOVirtual.DelayedCall(fadeDuration, () =>
        {
            parent2.gameObject.SetActive(false);
            parent1.gameObject.SetActive(true);

            foreach (Transform child in parent1.transform.GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag("Event"))
                {
                    eventMaker = child.GetComponent<EventMaker>();
                    if (eventMaker != null)
                    {
                        eventMaker.RefreshEvent(); // ˢ���¼�
                        //Debug.Log("ˢ���¼�");
                        break; // �ҵ�������ѭ��
                    }
                }
            }
           
            foreach (var renderer in targetRenderers1)
            {
                renderer.DOFade(1f, fadeDuration);
            }

            // �ȴ�������ɺ�������һ���л�
            DOVirtual.DelayedCall(fadeDuration, () => isFading = false);
        });
    }

    private List<SpriteRenderer> GetTargetRenderers(Transform parent)
    {
        List<SpriteRenderer> targetRenderers = new List<SpriteRenderer>();

        foreach (Transform child in parent)
        {
            if (targetTags.Contains(child.tag))
            {
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    targetRenderers.Add(spriteRenderer);
                }
            }

            if (child.childCount > 0)
            {
                targetRenderers.AddRange(GetTargetRenderers(child));
            }
        }

        return targetRenderers;
    }
}
