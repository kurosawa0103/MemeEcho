using UnityEngine;
using Fungus;

[CommandInfo("Camera", "Set Camera Zoom Z Range", "����������� Z �᷶Χ������Ŀ��λ��")]
public class SetCameraZoomZRange : Command
{
    [Tooltip("�µ���С Z ֵ")]
    public float newMinZ =-16;

    [Tooltip("�µ���� Z ֵ")]
    public float newMaxZ=-20f;

    [Tooltip("����Ŀ�� Z λ�ã���ѡ��")]
    public float setZ=-20;

    public override void OnEnter()
    {
        CameraZoomWithObjectScale zoomScript = FindObjectOfType<CameraZoomWithObjectScale>();
        if (zoomScript == null)
        {
            Debug.LogWarning("δ�ҵ� CameraZoomWithObjectScale �ű�����ȷ�������д��ڸ����");
            Continue();
            return;
        }

        // ���� minZ �� maxZ
        zoomScript.minZ = newMinZ;
        zoomScript.maxZ = newMaxZ;

        // ���� targetZ��������Ӧ��
        zoomScript.targetZ = setZ;

        Continue();
    }

    public override string GetSummary()
    {

        return $"����Z��Χ��{newMinZ} �� {newMaxZ}, ��ǰZ: {setZ}";
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 180, 75, 255); // �Զ�����ɫ
    }
}
