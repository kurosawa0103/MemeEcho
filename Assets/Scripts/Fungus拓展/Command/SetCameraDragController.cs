using UnityEngine;
using Fungus;

[CommandInfo("Camera", "SetCameraDragController", "ͨ�� Command �޸� CameraDragController ����")]
public class SetCameraDragController : Command
{

    [Tooltip("��ק�ٶ�")]
    public float dragSpeed = 0.002f;

    [Tooltip("X ����С����")]
    public float xMin = -2f;

    [Tooltip("X ���������")]
    public float xMax = 2f;

    [Tooltip("Y ����С����")]
    public float yMin = 0.5f;

    [Tooltip("Y ���������")]
    public float yMax = 1f;

    public override void OnEnter()
    {
        CameraDragController cameraDragController = FindObjectOfType<CameraDragController>();
        if (cameraDragController == null)
        {
            Debug.LogWarning("δ�ҵ� cameraDragController �ű�����ȷ�������д��ڸ����");
            Continue();
            return;
        }

        // �޸Ĳ���
        cameraDragController.dragSpeed = dragSpeed;
        cameraDragController.xLimit = new Vector2(xMin, xMax);
        cameraDragController.yLimit = new Vector2(yMin, yMax);

        Continue();
    }
    public override string GetSummary()
    {
        return $"������ק�ٶ�:{dragSpeed}, X��Χ:[{xMin},{xMax}], Y��Χ:[{yMin},{yMax}]";
    }

}
