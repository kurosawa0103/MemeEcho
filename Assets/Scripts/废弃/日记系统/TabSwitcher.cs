using UnityEngine;
using UnityEngine.UI;

public class TabSwitcher : MonoBehaviour
{
    [Header("页签按钮")]
    public Button tab1Button;
    public Button tab2Button;
    public Button tab3Button;

    [Header("页签对应的界面")]
    public GameObject panelTab1;
    public GameObject panelTab2;
    //public GameObject panelTab3;

    [Header("照片系统（如有）")]
    public PhotoSystem photoSystem;

    void Start()
    {
        tab1Button.onClick.AddListener(() => SwitchTab(1));
        tab2Button.onClick.AddListener(() => SwitchTab(2));
        //tab3Button.onClick.AddListener(() => SwitchTab(3));

        SwitchTab(1); // 默认显示第一个
    }

    void SwitchTab(int tabIndex)
    {
        panelTab1.SetActive(tabIndex == 1);
        panelTab2.SetActive(tabIndex == 2);
        //panelTab3.SetActive(tabIndex == 3);

        // 如果打开的是照片页签
        if (tabIndex == 2 && photoSystem != null)
        {
            photoSystem.DisplayPhotos();
        }
        // 如果打开的是主线页签
        else if (tabIndex == 1 )
        {

        }
    }
}
