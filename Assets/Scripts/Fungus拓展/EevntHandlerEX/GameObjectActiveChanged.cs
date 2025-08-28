using UnityEngine;
using Fungus;

[EventHandlerInfo("GameObject",
                  "Game Object Active Changed",
                  "Fires when a specified GameObject changes its active state (Enable or Disable).")]
[AddComponentMenu("")]
public class GameObjectActiveChanged : EventHandler
{
    public enum TriggerCondition
    {
        OnEnabled,
        OnDisabled
    }

    [Tooltip("The GameObject to watch for active state changes")]
    [SerializeField] private GameObject targetObject;

    [Tooltip("Which kind of active state change should trigger the event")]
    [SerializeField] private TriggerCondition triggerOn = TriggerCondition.OnEnabled;

    private bool lastState;

    protected virtual void Start()
    {
        if (targetObject != null)
        {
            lastState = targetObject.activeSelf;
        }
    }

    protected virtual void Update()
    {
        if (targetObject == null)
            return;

        bool currentState = targetObject.activeSelf;

        if (currentState != lastState)
        {
            // ״̬�仯
            if (triggerOn == TriggerCondition.OnEnabled && currentState)
            {
                ExecuteBlock();
            }
            else if (triggerOn == TriggerCondition.OnDisabled && !currentState)
            {
                ExecuteBlock();
            }

            lastState = currentState;
        }
    }
}
