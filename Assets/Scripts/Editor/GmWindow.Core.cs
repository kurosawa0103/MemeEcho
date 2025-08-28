#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

public partial class GmWindow : OdinEditorWindow
{
    private Texture2D gmBanner;
    private Texture2D iconLeft, iconRight;

    [MenuItem("GM工具/GmWindow %#g")]
    private static void OpenWindow()
    {
        var window = GetWindow<GmWindow>();
        window.titleContent = new GUIContent("牛逼的GM指令大全");
        window.Show();
        window.RefreshItemList();   // 来自 InventoryTab
        window.RefreshEventList();  // 来自 EventTab
        window.LoadJsonData();      // 来自 StatusTab
        window.AutoFindTimeSystem();
    }

    private GUIStyle titleStyle;

    private void OnEnable()
    {
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
        }

        // 加载 Resources 下的图片资源，例如 Resources/Editor/gm_banner.png
        gmBanner = Resources.Load<Texture2D>("Editor/GmBackground");
        iconLeft = Resources.Load<Texture2D>("Editor/IconLeft");
        iconRight = Resources.Load<Texture2D>("Editor/IconRight");
    }

    // 在窗口最顶部绘制图片和标题
    protected override void OnImGUI()
    {
        Rect windowRect = new Rect(0, 0, position.width, position.height);

        if (gmBanner != null)
            GUI.DrawTexture(windowRect, gmBanner, ScaleMode.StretchToFill);

        // 半透明遮罩
        Color oldColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.7f);
        GUI.DrawTexture(windowRect, Texture2D.whiteTexture);
        GUI.color = oldColor;

        GUILayout.BeginVertical("box");
        GUILayout.Space(10);

        // 插入左右图标与标题
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        float iconSize = titleStyle.fontSize + 10;
        if (iconLeft != null)
            GUILayout.Label(iconLeft, GUILayout.Width(iconSize), GUILayout.Height(iconSize));

        GUILayout.Space(10);

        if (iconLeft != null)
            GUILayout.Label(iconLeft, GUILayout.Width(iconSize), GUILayout.Height(iconSize));

        GUILayout.Space(10);

        if (iconLeft != null)
            GUILayout.Label(iconLeft, GUILayout.Width(iconSize), GUILayout.Height(iconSize));

        GUILayout.Space(20);
        GUILayout.Label("牛逼的GM指令大全", titleStyle);
        GUILayout.Space(20);

        if (iconRight != null)
            GUILayout.Label(iconRight, GUILayout.Width(iconSize), GUILayout.Height(iconSize));

        GUILayout.Space(10);

        if (iconRight != null)
            GUILayout.Label(iconRight, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
        
        GUILayout.Space(6);

        if (iconRight != null)
            GUILayout.Label(iconRight, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        base.OnImGUI();
        GUILayout.EndVertical();
    }
}
#endif
