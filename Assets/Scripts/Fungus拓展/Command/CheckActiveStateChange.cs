using UnityEngine;
using Fungus;
using System.Collections;

[CommandInfo("GameObject",
             "Check Active State Change",
             "Waits until a GameObject changes to the target active state.")]
public class CheckActiveStateChange : Command
{
    [Tooltip("The GameObject to monitor")]
    [SerializeField] private GameObject targetObject;

    [Tooltip("Wait until it becomes active (true) or inactive (false)")]
    [SerializeField] private bool targetState = true;


    public override void OnEnter()
    {

        if (targetObject != null)
        {
            GetFlowchart().StartCoroutine(WaitForStateChange());
            return;
        }

        Debug.LogWarning("CheckActiveStateChange: targetObject is null.");
        Continue();
    }

    private IEnumerator WaitForStateChange()
    {
        bool previous = targetObject.activeInHierarchy;

        while (targetObject != null)
        {
            bool current = targetObject.activeInHierarchy;

            if (current != previous && current == targetState)
            {
                break;
            }

            previous = current;
            yield return null;
        }


        Continue();
    }

    


  

    public override Color GetButtonColor()
    {
        return new Color32(180, 220, 255, 255); // 淡蓝色
    }

    public override string GetSummary()
    {
        if (targetObject == null)
            return "Error: No target GameObject";

        string stateText = targetState ? "激活 (On)" : "未激活 (Off)";
        return $"等待 {targetObject.name} 变为 {stateText}";
    }
}
