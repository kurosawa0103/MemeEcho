using UnityEngine;
using Fungus;

namespace Fungus
{
    [CommandInfo("GameObject",
                 "Toggle Component",
                 "Enable or disable a MonoBehaviour script on a GameObject.")]
    [AddComponentMenu("")]
    public class ToggleComponent : Command
    {
        [Tooltip("GameObject containing the component")]
        [SerializeField] protected GameObject targetObject;

        [Tooltip("Type name of the script (e.g. 'MyScript')")]
        [SerializeField] protected string componentTypeName;

        [Tooltip("Enable = true, Disable = false")]
        [SerializeField] protected bool enable = true;

        public override void OnEnter()
        {
            if (targetObject == null || string.IsNullOrEmpty(componentTypeName))
            {
                Continue();
                return;
            }

            var component = targetObject.GetComponent(componentTypeName);
            if (component != null && component is Behaviour behaviour)
            {
                behaviour.enabled = enable;
            }
            else
            {
                Debug.LogWarning($"Component '{componentTypeName}' not found or not a Behaviour on {targetObject.name}");
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (targetObject == null)
                return "No GameObject";
            if (string.IsNullOrEmpty(componentTypeName))
                return $"{targetObject.name} (No Type)";
            string action = enable ? "Enable" : "Disable";
            return $"{action} {componentTypeName} on {targetObject.name}";
        }

        public override Color GetButtonColor()
        {
            return new Color32(200, 220, 255, 255);
        }
    }
}