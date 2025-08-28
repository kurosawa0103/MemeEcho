using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeChildren : MonoBehaviour
{
    public List<string> targetTags = new List<string>(); // 可配置多个目标Tag
    public float fadeDuration = 1f;  // 淡出和淡入的时间
    public Transform parent1;  // 第一个父物体
    public Transform parent2;  // 第二个父物体
    private bool isFading = false; // 防止重复切换

    // 配置变量，控制点击时执行哪个操作
    public bool executeFadeInOut1 = true;  // 左键点击时执行 FadeInOutSequence
    public bool executeFadeInOut2 = true;  // 右键点击时执行 FadeInOutSequenceReverse
    public EventMaker eventMaker;
    public OpenCameraList openCameraList;
    private void Start()
    {
        // 初始化：将父物体2的子物体透明度设置为0
        List<SpriteRenderer> targetRenderers2 = GetTargetRenderers(parent2);
        parent2.gameObject.SetActive(false);
        foreach (var renderer in targetRenderers2)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0f); // 设置透明度为0
        }
    }

    private void Update()
    {
        // 监听鼠标点击事件
       
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
        if (isFading) return; // 如果正在切换，则不执行
        isFading = true; // 标记正在执行切换

        List<SpriteRenderer> targetRenderers1 = GetTargetRenderers(parent1);
        List<SpriteRenderer> targetRenderers2 = GetTargetRenderers(parent2);

        // 淡出 parent1 的子物体
        foreach (var renderer in targetRenderers1)
        {
            renderer.DOFade(0f, fadeDuration);
        }

        // 等待淡出完成后，隐藏 parent1，显示 parent2，并淡入
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
                        eventMaker.RefreshEvent(); // 刷新事件
                        //Debug.Log("刷新事件");
                        break; // 找到后跳出循环
                    }
                }
            }
            foreach (var renderer in targetRenderers2)
            {
                renderer.DOFade(1f, fadeDuration);
            }

            // 等待淡入完成后，允许下一次切换
            DOVirtual.DelayedCall(fadeDuration, () => isFading = false);
        });
    }

    private void FadeInOutSequenceReverse()
    {
        if (isFading) return; // 如果正在切换，则不执行
        isFading = true; // 标记正在执行切换

        List<SpriteRenderer> targetRenderers1 = GetTargetRenderers(parent1);
        List<SpriteRenderer> targetRenderers2 = GetTargetRenderers(parent2);

        // 淡出 parent2 的子物体
        foreach (var renderer in targetRenderers2)
        {
            renderer.DOFade(0f, fadeDuration);
        }

        // 等待淡出完成后，隐藏 parent2，显示 parent1，并淡入
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
                        eventMaker.RefreshEvent(); // 刷新事件
                        //Debug.Log("刷新事件");
                        break; // 找到后跳出循环
                    }
                }
            }
           
            foreach (var renderer in targetRenderers1)
            {
                renderer.DOFade(1f, fadeDuration);
            }

            // 等待淡入完成后，允许下一次切换
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
