using UnityEngine;
using Fungus;
using UnityEngine.SceneManagement;

[CommandInfo("Custom",
             "Change Map Scene",
             "根据 MapUIManager 当前地图信息，直接切换场景")]
public class ChangeMapSceneCommand : Command
{
    [Tooltip("场景中的 MapUIManager 对象")]
    public MapUIManager mapUIManager;

    public override void OnEnter()
    {
        if (mapUIManager == null)
        {
            Debug.LogWarning("未设置 MapUIManager 引用，无法切换场景");
            Continue();
            return;
        }

        var mapInfo = mapUIManager.currentMapInfo;
        if (mapInfo == null || string.IsNullOrEmpty(mapInfo.sceneName))
        {
            Debug.LogWarning("当前地图信息为空或场景名未配置");
            Continue();
            return;
        }

        Debug.Log($"切换场景到 {mapInfo.sceneName}");
        SceneManager.LoadScene(mapInfo.sceneName);

        Continue();
    }
}
