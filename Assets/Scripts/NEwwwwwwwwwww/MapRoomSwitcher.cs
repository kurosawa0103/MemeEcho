using UnityEngine;
using UnityEngine.SceneManagement;

public class MapRoomSwitcher : MonoBehaviour
{
    [Header("UI �������")]
    public GameObject panelMap;
    public GameObject panelRoom;

    void Start()
    {
        // ��ȡ��ǰ��������
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.StartsWith("Map-"))
        {
            if (panelMap != null) panelMap.SetActive(true);
            if (panelRoom != null) panelRoom.SetActive(false);
        }
        else if (sceneName.StartsWith("Room-"))
        {
            if (panelMap != null) panelMap.SetActive(false);
            if (panelRoom != null) panelRoom.SetActive(true);
        }
       
    }
}
