using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Fungus;
public class WaveTriggerCounter : MonoBehaviour
{
    [Header("配置参数")]
    public int currentCount = 0;
    public int targetCount = 5;
    public Vector3 finalScale = new Vector3(2f, 2f, 1f);
    public float scaleTime = 5;

    [Header("目标对象")]
    private Transform subRootTransform;

    [HideInInspector]
    public bool hasScaled = false;
    private GameObject obj;

    public Flowchart fungusFlowchart;
    public string blockToExecute; // 要执行的 Block 名称

    private void Awake()
    {
        if (subRootTransform == null)
        {
            obj = GameObject.FindGameObjectWithTag("SubRoot");
            if (obj != null)
                subRootTransform = obj.transform;
            else
                Debug.LogError("未找到 tag 为 SubRoot 的物体");
        }
    }

    public void TryScale()
    {
        if (!hasScaled && currentCount >= targetCount)
        {
            hasScaled = true;
            if (subRootTransform != null)
            {
                subRootTransform.DOLocalMove(new Vector3(0, 0, 0), scaleTime);
                subRootTransform.DOScale(finalScale, scaleTime);
                obj.transform.GetComponent<DraggableUIWindow>().canDrag = false;
                GameObject border = GameObject.FindGameObjectWithTag("BoxBorder");
                if (border != null)
                {
                    Image img = border.GetComponent<Image>();
                    if (img != null)
                    {
                        //img.DOFade(0f, scaleTime);
                        fungusFlowchart.ExecuteBlock(blockToExecute);
                    }
                }
            }
        }
    }
}
