using UnityEngine;
using UnityEngine.UI;

public class StartChat : MonoBehaviour
{
    public bool isChatActive = false; // ����״̬
    public Button chatButton; // ��ť
    public DialogManager dialogManager;
    public CharacterState characterState;

    void Start()
    {

    }

    public void ToggleChat()
    {
        isChatActive = !isChatActive; // �л�����״̬



        dialogManager.RefreshDialogList();
        Debug.Log("����״̬��" + (isChatActive ? "����" : "�ر�"));
    }

    
}
