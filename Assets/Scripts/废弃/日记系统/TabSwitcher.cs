using UnityEngine;
using UnityEngine.UI;

public class TabSwitcher : MonoBehaviour
{
    [Header("ҳǩ��ť")]
    public Button tab1Button;
    public Button tab2Button;
    public Button tab3Button;

    [Header("ҳǩ��Ӧ�Ľ���")]
    public GameObject panelTab1;
    public GameObject panelTab2;
    //public GameObject panelTab3;

    [Header("��Ƭϵͳ�����У�")]
    public PhotoSystem photoSystem;

    void Start()
    {
        tab1Button.onClick.AddListener(() => SwitchTab(1));
        tab2Button.onClick.AddListener(() => SwitchTab(2));
        //tab3Button.onClick.AddListener(() => SwitchTab(3));

        SwitchTab(1); // Ĭ����ʾ��һ��
    }

    void SwitchTab(int tabIndex)
    {
        panelTab1.SetActive(tabIndex == 1);
        panelTab2.SetActive(tabIndex == 2);
        //panelTab3.SetActive(tabIndex == 3);

        // ����򿪵�����Ƭҳǩ
        if (tabIndex == 2 && photoSystem != null)
        {
            photoSystem.DisplayPhotos();
        }
        // ����򿪵�������ҳǩ
        else if (tabIndex == 1 )
        {

        }
    }
}
