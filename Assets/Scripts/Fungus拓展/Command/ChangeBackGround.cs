using UnityEngine;
using Fungus;
using DG.Tweening;
using System.Collections.Generic;

[CommandInfo("�Զ���ָ��", "�����滻��ChangeBackGround��", "ʹ��Ԥ�����滻���������� SpriteRenderer ���룬ͬʱ�ɱ�����������")]
public class ChangeBackGround : Command
{
    [Tooltip("�±���Ԥ����")]
    public GameObject newBackgroundPrefab;

    [Tooltip("���������Tag")]
    public string parentTag = "Heart";

    [Tooltip("���뵭��ʱ��")]
    public float fadeDuration = 1f;

    [Tooltip("�Ƿ�ȴ���ɺ����ִ��")]
    public bool waitForCompletion = true;

    public override void OnEnter()
    {
        if (newBackgroundPrefab == null)
        {
            Debug.LogWarning("ChangeBackGround�������ñ���Ԥ���壡");
            Continue();
            return;
        }

        GameObject parentObj = GameObject.FindWithTag(parentTag);
        if (parentObj == null)
        {
            Debug.LogWarning($"ChangeBackGround��δ�ҵ� Tag Ϊ {parentTag} �����壡");
            Continue();
            return;
        }

        Transform parentTransform = parentObj.transform;

        // ��ȡ�ɱ��� GameObject������ÿ�α���Ԥ���嶼ֻ��һ�������壬��ʹ���ض����/������
        // �����ͨ������/Tag�ȷ�ʽʶ�𱳾���
        List<GameObject> oldBackgroundRoots = new List<GameObject>();
        foreach (Transform child in parentTransform)
        {
            if (child.GetComponent<SpriteRenderer>() != null || child.GetComponentInChildren<SpriteRenderer>() != null)
            {
                oldBackgroundRoots.Add(child.gameObject);
            }
        }

        // ʵ�����±���
        GameObject newBg = Object.Instantiate(newBackgroundPrefab, parentTransform);

        // ��ȡ�±����ڵ����� SpriteRenderer
        SpriteRenderer[] newRenderers = newBg.GetComponentsInChildren<SpriteRenderer>();

        List<Tween> tweens = new List<Tween>();

        foreach (var sr in newRenderers)
        {
            sr.color = new Color(1f, 1f, 1f, 0f);
            tweens.Add(sr.DOFade(1f, fadeDuration));
        }

        foreach (var oldRoot in oldBackgroundRoots)
        {
            SpriteRenderer[] oldRenderers = oldRoot.GetComponentsInChildren<SpriteRenderer>();
            foreach (var oldSr in oldRenderers)
            {
                tweens.Add(oldSr.DOFade(0f, fadeDuration));
            }

            // �ӳ����������������ڵ�
            DOVirtual.DelayedCall(fadeDuration, () =>
            {
                Object.Destroy(oldRoot);
            });
        }

        if (waitForCompletion)
        {
            DOTween.Sequence()
                .AppendInterval(fadeDuration)
                .OnComplete(() => Continue());
        }
        else
        {
            Continue();
        }
    }


    public override string GetSummary()
    {
        return newBackgroundPrefab != null ? $"�л���������{newBackgroundPrefab.name}" : "δ�����±���Ԥ����";
    }

    public override Color GetButtonColor()
    {
        return new Color32(180, 220, 255, 255);
    }
}
