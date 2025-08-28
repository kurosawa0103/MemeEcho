using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeMap : MonoBehaviour
{
    public string targetMapName;  // 目标场景名称
    // Start is called before the first frame update
    public void ChangeScene()
    {
        SceneManager.LoadSceneAsync(targetMapName);
    }


}
