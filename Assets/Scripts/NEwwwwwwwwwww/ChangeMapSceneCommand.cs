using UnityEngine;
using Fungus;
using UnityEngine.SceneManagement;

[CommandInfo("Custom",
             "Change Map Scene",
             "���� MapUIManager ��ǰ��ͼ��Ϣ��ֱ���л�����")]
public class ChangeMapSceneCommand : Command
{
    [Tooltip("�����е� MapUIManager ����")]
    public MapUIManager mapUIManager;

    public override void OnEnter()
    {
        if (mapUIManager == null)
        {
            Debug.LogWarning("δ���� MapUIManager ���ã��޷��л�����");
            Continue();
            return;
        }

        var mapInfo = mapUIManager.currentMapInfo;
        if (mapInfo == null || string.IsNullOrEmpty(mapInfo.sceneName))
        {
            Debug.LogWarning("��ǰ��ͼ��ϢΪ�ջ򳡾���δ����");
            Continue();
            return;
        }

        Debug.Log($"�л������� {mapInfo.sceneName}");
        SceneManager.LoadScene(mapInfo.sceneName);

        Continue();
    }
}
