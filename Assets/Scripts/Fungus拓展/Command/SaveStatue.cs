using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("储存",
             "储存状态",
             "储存NPC状态和事件状态")]

public class SaveStatue : Command
{
    private IEventStateDataHandler eventStateDataHandler;
    [Header("事件名字")]
    public string eventName;
    [Header("保存的值，string类型")]
    public string eventStatus;

    public override void OnEnter()
    {
        eventStateDataHandler = new JsonGenerator();


        // 判断是否配了事件，配了就保存事件状态
        eventStateDataHandler.ModifyEventStatus(eventName, eventStatus);

        //fungus内也给我储存一下
        GetComponent<Flowchart>().SetStringVariable(eventName, eventStatus);
        Continue();
    }
    public override string GetSummary()
    {

        if (eventName != null)
            return "储存了事件：" + eventName + "，它的值为：" + eventStatus;

        else
            return "没有设置键";
    }
    public override Color GetButtonColor()
    {
        return new Color32(0, 234, 255, 255);
    }
}
