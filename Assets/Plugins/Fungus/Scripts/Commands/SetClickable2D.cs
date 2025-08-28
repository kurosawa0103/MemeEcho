using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Sets multiple Clickable2D components to be clickable / non-clickable.
    /// </summary>
    [CommandInfo("Sprite",
                 "Set Multiple Clickable 2D",
                 "Sets multiple Clickable2D components to be clickable / non-clickable.")]
    [AddComponentMenu("")]
    public class SetClickable2D : Command
    {
        [Tooltip("References to Clickable2D components on gameobjects")]
        [SerializeField] protected List<Clickable2D> targetClickable2Ds = new List<Clickable2D>();

        [Tooltip("Set to true to enable the components")]
        [SerializeField] protected BooleanData activeState;

        [Tooltip("Whether to use parent-child hierarchy to find Clickable2D objects")]
        [SerializeField] protected bool useParentHierarchy = false;

        [Tooltip("Optional parent object to find Clickable2D objects within its children")]
        [SerializeField] protected Transform parentTransform;

        #region Public members

        public override void OnEnter()
        {
            if (useParentHierarchy && parentTransform != null)
            {
                // ��������˸������ң������������µ����������壬�ҵ� Clickable2D ���
                var clickable2DsInParent = parentTransform.GetComponentsInChildren<Clickable2D>();

                if (clickable2DsInParent != null && clickable2DsInParent.Length > 0)
                {
                    foreach (var clickable in clickable2DsInParent)
                    {
                        if (clickable != null)
                        {
                            clickable.ClickEnabled = activeState.Value;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("û���ҵ��ɵ���� Clickable2D �����");
                }
            }
            else if (targetClickable2Ds != null && targetClickable2Ds.Count > 0)
            {
                // ���û�����ø������ң�ʹ�����õ���Ʒ�б�
                foreach (var clickable in targetClickable2Ds)
                {
                    if (clickable != null)
                    {
                        clickable.ClickEnabled = activeState.Value;
                    }
                }
            }
            else
            {
                Debug.LogWarning("δָ��Ŀ�� Clickable2D �������δ���ø������ң�");
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (useParentHierarchy)
            {
                if (parentTransform != null)
                {
                    return $"�����壺{parentTransform.gameObject.name} �µ����������彫�ᱻ����";
                }
                else
                {
                    return "���󣺸�����δ����";
                }
            }

            if (targetClickable2Ds == null || targetClickable2Ds.Count == 0)
            {
                return "����û��ָ�� Clickable2D ���";
            }

            string summary = "Ŀ�����壺";
            foreach (var clickable in targetClickable2Ds)
            {
                if (clickable != null)
                {
                    summary += clickable.gameObject.name + ", ";
                }
            }

            return summary.TrimEnd(',', ' ');
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return activeState.booleanRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}
