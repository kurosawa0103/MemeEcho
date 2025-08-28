using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Fungus
{
    [CommandInfo("Variable", "Set Variable List", "批量设置多个变量，每个变量可使用不同的操作符和值")]
    [AddComponentMenu("")]
    public class SetVariableList : Command
    {
        [System.Serializable]
        public class SetVariableItem
        {
            [Tooltip("变量和值")]
            public AnyVariableAndDataPair variableAndData = new AnyVariableAndDataPair();

#if UNITY_EDITOR
            [ValueDropdown(nameof(GetOperatorDropdown))]
#endif
            public SetOperator setOperator = SetOperator.Assign;

#if UNITY_EDITOR
            private static IEnumerable<ValueDropdownItem<SetOperator>> GetOperatorDropdown()
            {
                yield return new ValueDropdownItem<SetOperator>("= （赋值）", SetOperator.Assign);
                yield return new ValueDropdownItem<SetOperator>("+= （增加）", SetOperator.Add);
                yield return new ValueDropdownItem<SetOperator>("-= （减少）", SetOperator.Subtract);
                yield return new ValueDropdownItem<SetOperator>("*= （乘）", SetOperator.Multiply);
                yield return new ValueDropdownItem<SetOperator>("/= （除）", SetOperator.Divide);
            }
#endif
        }

        [Tooltip("批量设置变量列表")]
        [SerializeField]
        public List<SetVariableItem> variableItems = new List<SetVariableItem>();

        protected virtual void DoSetOperations()
        {
            foreach (var item in variableItems)
            {
                if (item.variableAndData.variable == null)
                    continue;

                item.variableAndData.SetOp(item.setOperator);
            }
        }

        public override void OnEnter()
        {
            DoSetOperations();
            Continue();
        }

        public override string GetSummary()
        {
            if (variableItems == null || variableItems.Count == 0)
                return "Error: No variables selected";

            string summary = "";
            foreach (var item in variableItems)
            {
                if (item.variableAndData.variable == null)
                {
                    summary += "[None]\n";
                    continue;
                }

                string desc = item.variableAndData.variable.Key + "" +
                              GetOperatorSymbol(item.setOperator) + "" +
                              item.variableAndData.GetDataDescription();

                summary += desc + "   ";
            }

            return summary.TrimEnd();
        }

        private string GetOperatorSymbol(SetOperator op)
        {
            return op switch
            {
                SetOperator.Assign => "=",
                SetOperator.Add => "+=",
                SetOperator.Subtract => "-=",
                SetOperator.Multiply => "*=",
                SetOperator.Divide => "/=",
                _ => "?"
            };
        }

        public override bool HasReference(Variable variable)
        {
            foreach (var item in variableItems)
            {
                if (item.variableAndData.HasReference(variable))
                    return true;
            }

            return false;
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            if (variableItems != null)
            {
                foreach (var item in variableItems)
                {
                    item.variableAndData.RefreshVariableCacheHelper(GetFlowchart(), ref referencedVariables);
                }
            }
        }
#endif
    }
}
