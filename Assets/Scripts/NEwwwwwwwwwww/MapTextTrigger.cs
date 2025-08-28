using UnityEngine;

public class MapTextTrigger : MonoBehaviour
{
    [Header("�������ͼ��Ϣ")]
    public MapInfo newMapInfo;
    public MapUIManager mapUIManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mapUIManager?.ShowMapInfo(newMapInfo);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mapUIManager?.ResetToDefault();
        }
    }
}
