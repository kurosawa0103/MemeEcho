using System.Collections;
using UnityEngine;
using TMPro;
using Fungus;
using DG.Tweening;

[CommandInfo("UI", "PopupTextDOTween", "��ʾ�������ֿ򣬴�DOTween��Ч���Զ���ʧ")]
public class PopupTextDOTween : Command
{
    [Tooltip("��ʾ����������")]
    [TextArea]
    public string message;

    [Tooltip("��ʾ�� Panel (�����������ı�)")]
    public GameObject popupPanel;

    [Tooltip("TMP_Text �����������ʾ����")]
    public TMP_Text popupText;

    [Tooltip("������������ʱ��")]
    public float punchDuration = 0.5f;

    [Tooltip("ͣ��ʱ�䣬��")]
    public float displayTime = 3f;

    [Tooltip("���ŵ�������")]
    public float punchStrength = 1.1f;

    [Tooltip("�Ƿ�ȴ���������ʾ�������ټ�������")]
    public bool waitForComplete = true;

    public override void OnEnter()
    {
        if (popupPanel == null || popupText == null)
        {
            Debug.LogWarning("PopupTextDOTween: Panel �� Text δ���ã�");
            Continue();
            return;
        }

        // ��������
        popupText.text = message;

        // ���� Panel
        popupPanel.SetActive(true);

        // ��������
        popupPanel.transform.localScale = Vector3.zero;

        // DOTween ��������
        popupPanel.transform
            .DOScale(Vector3.one * punchStrength, punchDuration / 2)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                popupPanel.transform.DOScale(Vector3.one, punchDuration / 2).SetEase(Ease.OutBack);
            });

        if (waitForComplete)
        {
            StartCoroutine(HideAfterDelay());
        }
        else
        {
            // ���ȴ���ֱ�Ӽ���
            Continue();
            StartCoroutine(HideAfterDelay(false));
        }
    }

    private IEnumerator HideAfterDelay(bool callContinue = true)
    {
        yield return new WaitForSeconds(displayTime);

        // ��С��ʧ
        popupPanel.transform.DOScale(Vector3.zero, punchDuration).SetEase(Ease.InBack);

        yield return new WaitForSeconds(punchDuration);

        popupPanel.SetActive(false);

        if (callContinue && waitForComplete)
        {
            Continue();
        }
    }

    public override string GetSummary()
    {
        string panelName = popupPanel != null ? popupPanel.name : "δ����Panel";
        string textPreview = !string.IsNullOrEmpty(message) ? message : "δ��������";
        return $"Panel: {panelName}, Text: {textPreview}, �ȴ�����: {waitForComplete}";
    }
}
