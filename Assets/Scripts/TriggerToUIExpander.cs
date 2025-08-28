using UnityEngine;

public class TriggerToUIExpander : MonoBehaviour
{
    [Tooltip("����Ŀ�� UI �����ϵ� Expand �ű�")]
    public ExpandUIOnTrigger uiExpander;

    [Tooltip("����������")]
    public GameObject triggerObject;

    [Tooltip("��Ҫ���õĽű�")]
    public MonoBehaviour scriptToEnable;

    [Tooltip("��Ҫ���õĽű�")]
    public MonoBehaviour scriptToDisable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == triggerObject)
        {
            Debug.Log("[TriggerToUIExpander] ������������ִ�в���");

            // ���ýű�
            if (scriptToEnable != null)
            {
                scriptToEnable.enabled = true;
                Debug.Log("[TriggerToUIExpander] �����ýű���" + scriptToEnable.GetType().Name);
            }

            // ���ýű�
            if (scriptToDisable != null)
            {
                scriptToDisable.enabled = false;
                Debug.Log("[TriggerToUIExpander] �ѽ��ýű���" + scriptToDisable.GetType().Name);
            }

            // ֪ͨ UI ִ������
            if (uiExpander != null)
            {
                uiExpander.StartExpand();
                Debug.Log("[TriggerToUIExpander] ������ UI ��չ����");
            }
        }
    }
}