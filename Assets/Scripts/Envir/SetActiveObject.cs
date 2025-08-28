using System.Collections.Generic;
using UnityEngine;

public class SetActiveObjects : MonoBehaviour
{
    [Tooltip("����ʱ����Щ������Ϊ����")]
    public List<GameObject> objectsToEnable;

    [Tooltip("����ʱ����Щ������Ϊ����")]
    public List<GameObject> objectsToDisable;

    /// <summary>
    /// Ӧ�ü���״̬��reverse Ϊ true ʱ��ת����/�����߼�
    /// </summary>
    public void ApplySetActive(bool reverse = false)
    {
        if (!reverse)
        {
            foreach (var obj in objectsToEnable)
            {
                if (obj != null) obj.SetActive(true);
            }

            foreach (var obj in objectsToDisable)
            {
                if (obj != null) obj.SetActive(false);
            }
        }
        else
        {
            foreach (var obj in objectsToEnable)
            {
                if (obj != null) obj.SetActive(false);
            }

            foreach (var obj in objectsToDisable)
            {
                if (obj != null) obj.SetActive(true);
            }
        }
    }
}
