using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using DG.Tweening;

[CommandInfo("Event", "Event Caller", "Calls a specific event from EventManager.")]
public class EventCaller : Command
{
    [Tooltip("The event data to load.")]
    public EventData specificEventData;

    [Header("切换岛屿用，一般不用配置")]
    [Tooltip("是否需要淡入淡出黑幕效果")]
    public bool needFadeTransition = false;

    [Tooltip("淡入淡出时间（秒）")]
    public float fadeDuration = 1f;

    [Tooltip("黑幕 SpriteRenderer")]
    public SpriteRenderer blackScreenRenderer;

    [Tooltip("是否需要禁用指定Tag的物体")]
    public bool needDisableTagObjects = false;

    [Tooltip("需要禁用的Tag名称")]
    public string tagNameToDisable;

    public override void OnEnter()
    {
        if (needFadeTransition && blackScreenRenderer != null)
        {
            StartCoroutine(FadeTransition());
        }
        else
        {
            CallEvent();
        }
    }

    private IEnumerator FadeTransition()
    {
        // 淡入
        blackScreenRenderer.color = new Color(0, 0, 0, 0);
        blackScreenRenderer.gameObject.SetActive(true);
        blackScreenRenderer.DOFade(1f, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        CallEvent(); // 调用事件

        // 淡出
        blackScreenRenderer.DOFade(0f, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        blackScreenRenderer.color = new Color(0, 0, 0, 0); // 淡出后清空
    }

    private void CallEvent()
    {
        // 处理禁用物体
        if (needDisableTagObjects && !string.IsNullOrEmpty(tagNameToDisable))
        {
            GameObject objectsToDisable = GameObject.FindGameObjectWithTag(tagNameToDisable);
            objectsToDisable.SetActive(false);
        }

        // 调用事件
        EventManager eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.LoadEvent(specificEventData);
            Debug.Log($"调用事件: {specificEventData.name}");
        }
        else
        {
            Debug.LogWarning("EventManager 实例未找到！");
        }

        Continue();
    }

    public override string GetSummary()
    {
        if (specificEventData != null)
        {
            return $"加载事件: {specificEventData.name}";
        }
        else
        {
            return "未配置事件";
        }
    }
}
