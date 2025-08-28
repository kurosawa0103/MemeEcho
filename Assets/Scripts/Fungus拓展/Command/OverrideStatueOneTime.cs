using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("储存",
             "一次性覆写状态",
             "覆写事件状态")]

public class OverrideStatueOneTime : Command
{
    private IEventStateDataHandler eventStateDataHandler;

    [Header("从 JSON 中获取的事件名字列表, 要和 Fungus 的名字一致")]
    public List<string> eventNames = new List<string>(); // 存储多个事件名

    private void Start()
    {
        eventStateDataHandler = new JsonGenerator();
    }

    public override void OnEnter()
    {
        Flowchart flowchart = GetComponent<Flowchart>();

        foreach (string eventName in eventNames)
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                // 获取 JSON 的值
                string eventStatus = eventStateDataHandler.GetEventStatus(eventName);

                // 把 Fungus 内自己的变量的值变为 JSON 中获取的值
                flowchart.SetStringVariable(eventName, eventStatus);
            }
        }

        Continue();
    }

    public override string GetSummary()
    {
        if (eventNames.Count > 0)
            return "覆写了事件: " + string.Join(", ", eventNames);
        else
            return "没有设置变量";
    }

    public override Color GetButtonColor()
    {
        return new Color32(0, 234, 255, 255);
    }
}
