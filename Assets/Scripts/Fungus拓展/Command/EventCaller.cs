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

    [Header("�л������ã�һ�㲻������")]
    [Tooltip("�Ƿ���Ҫ���뵭����ĻЧ��")]
    public bool needFadeTransition = false;

    [Tooltip("���뵭��ʱ�䣨�룩")]
    public float fadeDuration = 1f;

    [Tooltip("��Ļ SpriteRenderer")]
    public SpriteRenderer blackScreenRenderer;

    [Tooltip("�Ƿ���Ҫ����ָ��Tag������")]
    public bool needDisableTagObjects = false;

    [Tooltip("��Ҫ���õ�Tag����")]
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
        // ����
        blackScreenRenderer.color = new Color(0, 0, 0, 0);
        blackScreenRenderer.gameObject.SetActive(true);
        blackScreenRenderer.DOFade(1f, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        CallEvent(); // �����¼�

        // ����
        blackScreenRenderer.DOFade(0f, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        blackScreenRenderer.color = new Color(0, 0, 0, 0); // ���������
    }

    private void CallEvent()
    {
        // �����������
        if (needDisableTagObjects && !string.IsNullOrEmpty(tagNameToDisable))
        {
            GameObject objectsToDisable = GameObject.FindGameObjectWithTag(tagNameToDisable);
            objectsToDisable.SetActive(false);
        }

        // �����¼�
        EventManager eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.LoadEvent(specificEventData);
            Debug.Log($"�����¼�: {specificEventData.name}");
        }
        else
        {
            Debug.LogWarning("EventManager ʵ��δ�ҵ���");
        }

        Continue();
    }

    public override string GetSummary()
    {
        if (specificEventData != null)
        {
            return $"�����¼�: {specificEventData.name}";
        }
        else
        {
            return "δ�����¼�";
        }
    }
}
