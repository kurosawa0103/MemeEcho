using UnityEngine;
using System.Collections.Generic;
using Fungus;
using System;

public class SpriteTargetTracker : MonoBehaviour
{
    public List<CharacterObject> trackedTargets = new List<CharacterObject>(); // 当前锁定的小人列表
    public List<CharacterObject> photoTrackList = new List<CharacterObject>(); // 拍照达成检测用列表
    public List<CharacterObject> viewTrackList = new List<CharacterObject>(); // 拍照达成检测用列表

    private IEventStateDataHandler eventStateHandler = new JsonGenerator();
    public SpriteRenderer targetSpriteRenderer;  // 目标Sprite的SpriteRenderer

    public event Action OnTrackingSuccessOnce;
    private int lastTriggeredFrame = -1;

    void Start()
    {
       
    }

    void OnTriggerEnter(Collider other)
    {
        CharacterObject characterBehavior = other.GetComponent<CharacterObject>();
        if (characterBehavior == null)
            return;

        UpdateClickability(characterBehavior, true);

        if (!trackedTargets.Contains(characterBehavior) && !characterBehavior.hasRecorded && characterBehavior.needRecord)
        {
            trackedTargets.Add(characterBehavior);
            Debug.Log($"Target {characterBehavior.name} started tracking.");
        }

        // 只有在 needPhoto 为 true 时才将其添加到 photoTrackList
        if (characterBehavior.needPhoto && !photoTrackList.Contains(characterBehavior))
        {
            photoTrackList.Add(characterBehavior);
            Debug.Log($"Target {characterBehavior.name} added to photo track list.");
        }
        if (!viewTrackList.Contains(characterBehavior))
        {
            // 进入后立即尝试触发一次判断
            TryTriggerFungusEvent(characterBehavior);
        }


    }


    void OnTriggerExit(Collider other)
    {
        CharacterObject characterBehavior = other.GetComponent<CharacterObject>();
        UpdateClickability(characterBehavior, false);

        if (characterBehavior != null && trackedTargets.Contains(characterBehavior))
        {
            trackedTargets.Remove(characterBehavior);
            //Debug.Log($"Target {characterBehavior.name} stopped tracking.");
        }

        if (photoTrackList.Contains(characterBehavior))
        {
            photoTrackList.Remove(characterBehavior);
            //Debug.Log($"Target {characterBehavior.name} removed from photo track list.");
        }

        if (viewTrackList.Contains(characterBehavior))
        {
            viewTrackList.Remove(characterBehavior);
            //Debug.Log($"Target {characterBehavior.name} removed .");
        }
    }


    void Update()
    {
        TrackTargets();
    }
    private void TryTriggerFungusEvent(CharacterObject behavior)
    {
        viewTrackList.Add(behavior);

        if (!viewTrackList.Contains(behavior))
        {
            viewTrackList.Add(behavior);
        }

        // 同一帧内只允许触发一次
        if (Time.frameCount != lastTriggeredFrame)
        {
            lastTriggeredFrame = Time.frameCount;
            OnTrackingSuccessOnce?.Invoke();
        }
    }
    private void TrackTargets()
    {
        // 清除已经被销毁的目标
        trackedTargets.RemoveAll(target => target == null);
        photoTrackList.RemoveAll(target => target == null);
        viewTrackList.RemoveAll(target => target == null);

        // 遍历当前正在被追踪的小人，处理计时和记录
        foreach (var target in trackedTargets)
        {
            // 如果目标没有完成计时，则继续累积计时
            if (target.accumulatedLockTime < target.targetTime)
            {
                target.accumulatedLockTime += Time.deltaTime;
            }

            // 达到记录时间后，记录日记，仅第一次记录
            if (target.HasReachedRecordTime() && !target.hasRecorded)
            {
                RecordBehavior(target);
                target.hasRecorded = true;  // 标记为已记录
            }
        }
    }

    private void RecordBehavior(CharacterObject behavior)
    {
        eventStateHandler.ModifyEventStatus(behavior.eventName, behavior.eventValue);
        Debug.Log($"抓取事件: {behavior.eventName}+值：{behavior.eventValue}");
    }

    private void UpdateClickability(CharacterObject behavior, bool isClickable)
    {
        if (behavior == null)
        {
            return;
        }

        Clickable2D clickable = behavior.GetComponent<Clickable2D>();
        CharacterObject characterBehavior = behavior.GetComponent<CharacterObject>();
        if (clickable != null)
        {
            clickable.ClickEnabled = isClickable;
        }
        if (characterBehavior != null)
        {
            characterBehavior.canClickTalk = isClickable;
        }
    }
    public bool HasRequiredTargetsInPhotoFrame(List<string> requiredNames)
    {
        foreach (string name in requiredNames)
        {
            bool found = photoTrackList.Exists(cb => cb.name == name);
            if (!found)
                return false; // 任意一个未找到则返回 false
        }
        return true;
    }

}
