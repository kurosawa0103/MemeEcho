using UnityEngine;
using Fungus;

[CommandInfo("�Զ���ָ��", "InstanceSubRoot",
    "��ѡʱ����ָ��Tag�ĸ�������ʵ����Prefab������SmallBox��FollowUI��δ��ѡʱ�����ٸø����������д�ָ��Tag��������")]
public class InstanceSubRoot : Command
{
    [Tooltip("�Ƿ�ִ��ʵ����������ִ������")]
    public bool instantiate = true;

    [Tooltip("Ҫʵ������ Prefab")]
    public GameObject prefab;

    [Tooltip("����Ŀ�� Tag")]
    public string targetTag = "SubRoot";

    [Tooltip("������� Tag�����ĸ��������²�����")]
    public string parentTag = "Canvas";

    public override void OnEnter()
    {
        if (instantiate)
        {
            GameObject parentObj = GameObject.FindWithTag(parentTag);
            if (parentObj != null && prefab != null)
            {
                //  ʵ���� Prefab
                GameObject newInstance = Object.Instantiate(prefab, parentObj.transform);

                // ��ȡ��һ����������Ϊ uiElement
                RectTransform firstChild = null;
                if (newInstance.transform.childCount > 0)
                {
                    firstChild = newInstance.transform.GetChild(0).GetComponent<RectTransform>();
                }

                if (firstChild == null)
                {
                    Debug.LogWarning("InstanceSubRoot��ʵ��������û����Ч�������δ�� RectTransform");
                }
                else
                {
                    //  �������д� SmallBox ��ǩ�Ķ��󣨰������ã�
                    FollowUI[] allFollowUIs = Resources.FindObjectsOfTypeAll<FollowUI>();
                    foreach (var followUI in allFollowUIs)
                    {
                        if (followUI.CompareTag("SmallBox"))
                        {
                            followUI.uiElement = firstChild;
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("InstanceSubRoot��δ�ҵ�ָ���ĸ������ Prefab δ����");
            }
        }
        else
        {
            GameObject[] allWithTag = GameObject.FindGameObjectsWithTag(targetTag);
            foreach (GameObject go in allWithTag)
            {
                if (go.transform.parent != null && go.transform.parent.CompareTag(parentTag))
                {
                    Object.Destroy(go);
                }
            }
        }

        Continue();
    }
}
