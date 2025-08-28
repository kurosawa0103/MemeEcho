using UnityEngine;
using System.Collections.Generic;
using Fungus;

[EventHandlerInfo("Custom", "On Sprite Track Success", "Triggered when tracker detects required characters inside a specified collider.")]
public class OnSpriteTrackSuccess : EventHandler
{
    public SpriteTargetTracker tracker;

    [Tooltip("要求的名称列表")]
    public List<string> requiredViewNames;

    [Tooltip("勾选后只触发一次，未勾选则每次满足条件都触发")]
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
            tracker.OnTrackingSuccessOnce -= HandleSuccess; // 先取消订阅，防止重复订阅
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

        // 如果是一次性触发模式且已经触发过，直接返回
        if (oneShot && triggered)
            return;
        if (IsAnyRequiredCharacterInside())
        {
            ExecuteBlock();

            if (oneShot)
            {
                triggered = true; // 标记已经触发过，防止再次触发
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
