
using UnityEngine;
using Sirenix.OdinInspector;

public class MouseFollowController : MonoBehaviour
{
    [Header("目标设置")]
    [Tooltip("通过标签查找目标窗口")]
    public string targetWindowTag = "DragWindow";

    [Header("控制模式")]
    [Tooltip("激活时的行为模式")]
    public FollowMode followMode = FollowMode.Enable;

    [Header("跟随设置")]
    [ShowIf("followMode", FollowMode.Enable)]
    [Tooltip("鼠标偏移量")]
    public Vector2 mouseOffset = Vector2.zero;

    [ShowIf("followMode", FollowMode.Disable)]
    [Tooltip("禁用跟随后是否保持窗口在当前位置")]
    public bool keepCurrentPosition = true;

    [Header("运行时设置")]
    [Tooltip("每次激活时重新查找目标")]
    public bool refreshTargetOnEnable = true;

    [Header("调试信息")]
    [ReadOnly, ShowInInspector]
    private bool isWindowFollowing = false;

    [ReadOnly, ShowInInspector]
    private DraggableUIWindow currentTarget;

    public enum FollowMode
    {
        Enable,  // 开启跟随
        Disable  // 关闭跟随
    }

    [Button("查找目标窗口")]
    private void FindTarget()
    {
        currentTarget = GetTargetWindow();
        if (currentTarget != null)
        {
            Debug.Log($"[MouseFollowController] 找到目标窗口: {currentTarget.name}");
        }
        else
        {
            Debug.LogWarning($"[MouseFollowController] 未找到标签为 '{targetWindowTag}' 的目标窗口！");
        }
    }

    [Button("执行控制")]
    private void ExecuteControl()
    {
        if (followMode == FollowMode.Enable)
        {
            EnableFollowMouse();
        }
        else
        {
            DisableFollowMouse();
        }
    }

    private void Start()
    {
        // 首次查找目标
        FindTarget();
    }

    private void OnEnable()
    {
        // 如果设置了每次激活时重新查找，或者当前没有目标
        if (refreshTargetOnEnable || currentTarget == null)
        {
            FindTarget();
        }

        ExecuteControl();
    }

    private DraggableUIWindow GetTargetWindow()
    {
        if (string.IsNullOrEmpty(targetWindowTag))
        {
            Debug.LogWarning("[MouseFollowController] 目标窗口标签为空！");
            return null;
        }

        GameObject taggedObj = GameObject.FindGameObjectWithTag(targetWindowTag);
        if (taggedObj != null)
        {
            DraggableUIWindow window = taggedObj.GetComponent<DraggableUIWindow>();
            if (window == null)
            {
                Debug.LogWarning($"[MouseFollowController] 标签为 '{targetWindowTag}' 的对象没有DraggableUIWindow组件！");
            }
            return window;
        }

        return null;
    }

    private void EnableFollowMouse()
    {
        DraggableUIWindow target = currentTarget ?? GetTargetWindow();

        if (target == null)
        {
            Debug.LogWarning($"[MouseFollowController] 未找到标签为 '{targetWindowTag}' 的目标窗口！物体: {gameObject.name}");
            return;
        }

        target.followMouse = true;
        target.mouseOffset = mouseOffset;
        isWindowFollowing = true;

        Debug.Log($"[MouseFollowController] 已开启窗口 '{target.name}' 的鼠标跟随功能");
    }

    private void DisableFollowMouse()
    {
        DraggableUIWindow target = currentTarget ?? GetTargetWindow();

        if (target == null)
        {
            Debug.LogWarning($"[MouseFollowController] 未找到标签为 '{targetWindowTag}' 的目标窗口！物体: {gameObject.name}");
            return;
        }

        // 如果选择保持当前位置，记录当前位置
        Vector2 currentPosition = Vector2.zero;
        if (keepCurrentPosition && target.followMouse)
        {
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                currentPosition = rectTransform.anchoredPosition;
            }
        }

        target.followMouse = false;
        isWindowFollowing = false;

        // 如果选择保持当前位置，设置位置
        if (keepCurrentPosition)
        {
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = currentPosition;
            }
        }

        Debug.Log($"[MouseFollowController] 已关闭窗口 '{target.name}' 的鼠标跟随功能");
    }

    private void Update()
    {
        // 更新调试信息
        if (currentTarget != null)
        {
            isWindowFollowing = currentTarget.followMouse;
        }
    }
}