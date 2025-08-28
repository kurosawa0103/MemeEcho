using UnityEngine;
using System.Collections.Generic;
using Fungus;

[EventHandlerInfo("Custom", "On Sprite Track Success", "Triggered when tracker detects required characters inside a specified collider.")]
public class OnSpriteTrackSuccess : EventHandler
{
    public SpriteTargetTracker tracker;

    [Tooltip("Ҫ��������б�")]
    public List<string> requiredViewNames;

    [Tooltip("��ѡ��ֻ����һ�Σ�δ��ѡ��ÿ����������������")]
    public bool oneShot = false;

    private bool triggered = false;

    void OnEnable()
    {
        if (tracker == null)
        {
            tracker = FindObjectOfType<SpriteTargetTracker>();
        }
        if (tracker != null)
        {
            tracker.OnTrackingSuccessOnce -= HandleSuccess; // ��ȡ�����ģ���ֹ�ظ�����
            tracker.OnTrackingSuccessOnce += HandleSuccess;
        }
    }

    void OnDisable()
    {
        if (tracker != null)
        {
            tracker.OnTrackingSuccessOnce -= HandleSuccess;
        }
    }


    void HandleSuccess()
    {
        if (tracker == null)
            return;

        // �����һ���Դ���ģʽ���Ѿ���������ֱ�ӷ���
        if (oneShot && triggered)
            return;
        if (IsAnyRequiredCharacterInside())
        {
            ExecuteBlock();

            if (oneShot)
            {
                triggered = true; // ����Ѿ�����������ֹ�ٴδ���
            }
        }
    }

    private bool IsAnyRequiredCharacterInside()
    {
        foreach (var character in tracker.viewTrackList)
        {
            if (character == null) continue;

            if (requiredViewNames.Contains(character.trackName))
            {
                return true;
            }
        }
        return false;
    }
}
