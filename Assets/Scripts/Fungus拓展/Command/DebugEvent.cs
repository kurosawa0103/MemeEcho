using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Event", "DebugEvent", ".")]
public class DebugEvent : Command
{
    private ShowDebug showDebug;
    [TextArea(3, 4)]
    public string debugLog;
    public override void OnEnter()
    {
        showDebug = GameObject.FindGameObjectWithTag("GM").GetComponent<ShowDebug>();
        showDebug.stateInfo = debugLog;
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(220, 220, 220, 255);
    }

    public override string GetSummary()
    {

        if (string.IsNullOrEmpty(debugLog))
        {
            return "null";
        }

        return "-----<<<" + debugLog + ">>>----- ";
    }

}