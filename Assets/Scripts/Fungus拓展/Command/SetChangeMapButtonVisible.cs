using UnityEngine;
using Fungus;

[CommandInfo("Custom", "Set ChangeMapButton Visible", "���� ChangeMapButton �ű��� canShow ����")]
public class SetChangeMapButtonVisible : Command
{
    [Tooltip("Ҫ���õ�Ŀ������")]
    public GameObject targetObject;

    [Tooltip("�Ƿ�������ʾ targetObject")]
    public BooleanData canShow = new BooleanData(true);

    public override void OnEnter()
    {
        if (targetObject != null)
        {
            ChangeMapButton cmb = targetObject.GetComponent<ChangeMapButton>();
            if (cmb != null)
            {
                cmb.canShow = canShow.Value;
                cmb.RefreshShowStatus();
            }
            else
            {
                Debug.LogWarning("Ŀ��������û�� ChangeMapButton �����");
            }
        }

        Continue();
    }

    public override string GetSummary()
    {
        if (targetObject == null)
            return "δ����Ŀ������";
        return targetObject.name + ".canShow = " + canShow.Value;
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255); // ����ɫ
    }
}
