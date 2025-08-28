using UnityEngine;
using Fungus;

[EventHandlerInfo("��չ", "С�����봥���� (2D)", "�� Tag Ϊ SmallBox �� 2D �������˴�������ʱ����")]
public class TriggerBoxEnter2D : EventHandler
{
    [Tooltip("Ŀ������� Tag")]
    [SerializeField] private string targetTag = "SmallBox";


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled || !gameObject.activeInHierarchy)
            return;

        if (other.CompareTag(targetTag))
        {
            Debug.Log($"TriggerBoxEnter2D: Tag Ϊ {targetTag} ��������봥�����򣬴����¼���");
            ExecuteBlock();
        }
    }
}
