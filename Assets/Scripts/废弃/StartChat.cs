using UnityEngine;
using UnityEngine.UI;

public class StartChat : MonoBehaviour
{
    public bool isChatActive = false; // ÁÄÌì×´Ì¬
    public Button chatButton; // °´Å¥
    public DialogManager dialogManager;
    public CharacterState characterState;

    void Start()
    {

    }

    public void ToggleChat()
    {
        isChatActive = !isChatActive; // ÇĞ»»ÁÄÌì×´Ì¬



        dialogManager.RefreshDialogList();
        Debug.Log("ÁÄÌì×´Ì¬£º" + (isChatActive ? "¿ªÆô" : "¹Ø±Õ"));
    }

    
}
