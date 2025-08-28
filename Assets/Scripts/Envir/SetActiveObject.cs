using System.Collections.Generic;
using UnityEngine;

public class SetActiveObjects : MonoBehaviour
{
    [Tooltip("调用时将这些物体设为启用")]
    public List<GameObject> objectsToEnable;

    [Tooltip("调用时将这些物体设为禁用")]
    public List<GameObject> objectsToDisable;

    /// <summary>
    /// 应用激活状态，reverse 为 true 时翻转启用/禁用逻辑
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
