using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FirstEvent : MonoBehaviour

{
    [Tooltip("目标物体的 Tag")]
    [SerializeField] private string targetTag = "SubRoot";



    private void Awake()
    {
        GameObject target = GameObject.FindGameObjectWithTag(targetTag);
        if (target == null)
        {
            Debug.LogWarning($"ChangeScaleByTag: 找不到 Tag 为 [{targetTag}] 的物体。");
            return;
        }
        target.transform.localScale = new Vector3(0,0,0);
        target.transform.localPosition = new Vector3(0, 1000, 0);
    }
}
