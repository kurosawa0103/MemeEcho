using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FirstEvent : MonoBehaviour

{
    [Tooltip("Ŀ������� Tag")]
    [SerializeField] private string targetTag = "SubRoot";



    private void Awake()
    {
        GameObject target = GameObject.FindGameObjectWithTag(targetTag);
        if (target == null)
        {
            Debug.LogWarning($"ChangeScaleByTag: �Ҳ��� Tag Ϊ [{targetTag}] �����塣");
            return;
        }
        target.transform.localScale = new Vector3(0,0,0);
        target.transform.localPosition = new Vector3(0, 1000, 0);
    }
}
