using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fungus;

[EventHandlerInfo("拓展",
                  "Player Check",
                  "当带有指定Tag的物体进入触发器后执行")]
public class PlayerCheck : EventHandler
{
    [Header("触发检测的Tag")]
    [Tooltip("设置检测的Tag，例如 'Player'")]
    [SerializeField] private string tagToCheck = "Player";

    [Header("延迟执行的帧数")]
    [Tooltip("在触发后延迟指定帧数再执行块")]
    [SerializeField] private int delayFrames = 0;

    [Header("只允许触发一次")]
    [Tooltip("如果勾选，触发后不会再次响应")]
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered = false; // 是否已经触发过

    private void OnTriggerEnter(Collider other)
    {
        // 检查物体的Tag是否匹配
        if (other.CompareTag(tagToCheck))
        {
            if (triggerOnce && hasTriggered)
            {
                Debug.Log("已经触发过，忽略后续触发");
                return;
            }

            Debug.Log($"Tag匹配: {tagToCheck}，触发事件");
            hasTriggered = true;

            // 启动延迟执行块的协程
            StartCoroutine(ExecuteBlockWithDelay());
        }
    }

    private IEnumerator ExecuteBlockWithDelay()
    {
        // 等待指定帧数
        for (int i = 0; i < delayFrames; i++)
        {
            yield return null;
        }

        // 执行关联的 Fungus 块
        ExecuteBlock();
    }

    private void OnDisable()
    {
        Debug.Log("PlayerCheck 已禁用，重置触发状态");
        hasTriggered = false; // 重置触发状态以便重新启用时可以再次触发
    }

}
