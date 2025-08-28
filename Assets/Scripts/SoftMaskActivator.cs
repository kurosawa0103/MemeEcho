using UnityEngine;
using SoftMasking; // �ǵ����� SoftMask ���������ռ�
using System.Collections;
public class SoftMaskActivator : MonoBehaviour
{
    public GameObject maskObject;
    public string targetTag = "TargetCollider";

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("������ײ��" + other.name);

        if (other.CompareTag(targetTag))
        {
            if (maskObject == null)
            {
                Debug.LogWarning("maskObject û�����ã�");
                return;
            }

            SoftMask softMask = maskObject.GetComponent<SoftMask>();
            if (softMask != null)
            {
                softMask.enabled = true;
                Debug.Log("SoftMask �����ã�");
            }
            else
            {
                Debug.LogWarning("Ŀ������û�� SoftMask �����");
            }
        }
    }
}