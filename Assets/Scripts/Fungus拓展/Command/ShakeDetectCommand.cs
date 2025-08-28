using UnityEngine;
using Fungus;

[CommandInfo("�Զ���",
             "�ζ����",
             "�ȴ� UI ��ָ������ζ�ָ�����������")]
public class ShakeDetectCommand : Command
{
    private DraggableUIWindow draggableWindow;

    [Tooltip("��ⴹֱ����ˮƽ�ζ�")]
    public bool vertical = true;

    [Tooltip("��Ҫ�Ļζ�����")]
    public int requiredShakeCount = 3;

    [Tooltip("�ж�Ϊ�ζ�����С���루���أ�")]
    public float shakeDistance = 30f;

    [Tooltip("�Ƿ��ڳɹ�������ζ���¼")]
    public bool resetAfterSuccess = true;


    public override void OnEnter()
    {
        draggableWindow = GameObject.FindGameObjectWithTag("Root") .transform .GetComponent< DraggableUIWindow>();
        if (draggableWindow == null)
        {
            Debug.LogWarning("δ���� draggableWindow");
            Continue();
            return;
        }

        // ���ûζ����
        draggableWindow.enableShakeDetection = true;
        draggableWindow.shakeRequiredCount = requiredShakeCount;
        draggableWindow.shakeThreshold = shakeDistance;

        // �������ûζ�����

        FungusManager.Instance.StartCoroutine(WaitForShake());
    }

    private System.Collections.IEnumerator WaitForShake()
    {
        while (true)
        {
            if (draggableWindow.GetCurrentShakeCount(vertical) >= requiredShakeCount)
            {
                Debug.Log("�ζ����ɹ���");
                if (resetAfterSuccess)
                    draggableWindow.ResetShakeCount();

                break;
            }
            yield return null;
        }

        Continue();
    }

    public override string GetSummary()
    {

        return $"{(vertical ? "��ֱ" : "ˮƽ")}�ζ� {requiredShakeCount} �Σ���ֵ {shakeDistance}��";
    }

    public override Color GetButtonColor()
    {
        return new Color32(200, 100, 150, 255);
    }
}
