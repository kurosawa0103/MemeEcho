using UnityEngine;
using Sirenix.OdinInspector; // 引入Odin Inspector命名空间

[CreateAssetMenu(fileName = "PageDataConfig", menuName = "创建/PageDataConfig", order = 1)]
public class Page : ScriptableObject
{
    [Header("页面数据列表")]
    public PageData[] pages; // 存放所有页面数据
}

[System.Serializable]
public class PageData
{
    public GameObject prefab; // 页面预制体
    public bool isLocked; // 是否解锁

    [ShowIf("isLocked", true)] // 当isUnlocked未勾选时显示
    public string unlockEvent; // 页面内容

    [ShowIf("isLocked", true)] // 当isUnlocked未勾选时显示
    public string unlockEventKey; // 解锁条件，比如完成某个任务
}
