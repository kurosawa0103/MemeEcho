using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExit : MonoBehaviour
{
    public GameObject exitPanel;
    public OpenSetting openSetting;
    void Start()
    {
        
    }
    void OnMouseDown()
    {
        OpenExit();
    }
    public void OpenExit()
    {
        //if(openSetting.isOn)
        //{
       //     exitPanel.SetActive(true);
       // }

    }

    public void CloseExit()
    {
        exitPanel.SetActive(false);
    }
    public void ExitGame()
    {
        // �ڱ༭��ģʽ���˳���Ϸʱ��Unity����ر�Ӧ�ó���������Ҫ�� #if UNITY_EDITOR ������
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �ڱ༭ģʽ��ֹͣ����
#else
        Application.Quit(); // �ڹ����İ汾���˳���Ϸ
#endif
        Debug.Log("��Ϸ���˳�");
    }
}
