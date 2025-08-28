using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChat : MonoBehaviour
{
    public bool chatIsOn = false;
    public GameObject chatPanel;
    public DialogManager dialogManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenChatPanel()
    {
        if(!chatIsOn)
        {
            dialogManager.RefreshDialogList();
            chatIsOn = true;
            chatPanel.SetActive(true);
        }
        else if(chatIsOn)
        {
            chatIsOn = false;
            chatPanel.SetActive(false);
        }

    }
}
