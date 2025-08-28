
using UnityEngine;
using Sirenix.OdinInspector;

public class MouseFollowController : MonoBehaviour
{
    [Header("Ŀ������")]
    [Tooltip("ͨ����ǩ����Ŀ�괰��")]
    public string targetWindowTag = "DragWindow";

    [Header("����ģʽ")]
    [Tooltip("����ʱ����Ϊģʽ")]
    public FollowMode followMode = FollowMode.Enable;

    [Header("��������")]
    [ShowIf("followMode", FollowMode.Enable)]
    [Tooltip("���ƫ����")]
    public Vector2 mouseOffset = Vector2.zero;

    [ShowIf("followMode", FollowMode.Disable)]
    [Tooltip("���ø�����Ƿ񱣳ִ����ڵ�ǰλ��")]
    public bool keepCurrentPosition = true;

    [Header("����ʱ����")]
    [Tooltip("ÿ�μ���ʱ���²���Ŀ��")]
    public bool refreshTargetOnEnable = true;

    [Header("������Ϣ")]
    [ReadOnly, ShowInInspector]
    private bool isWindowFollowing = false;

    [ReadOnly, ShowInInspector]
    private DraggableUIWindow currentTarget;

    public enum FollowMode
    {
        Enable,  // ��������
        Disable  // �رո���
    }

    [Button("����Ŀ�괰��")]
    private void FindTarget()
    {
        currentTarget = GetTargetWindow();
        if (currentTarget != null)
        {
            Debug.Log($"[MouseFollowController] �ҵ�Ŀ�괰��: {currentTarget.name}");
        }
        else
        {
            Debug.LogWarning($"[MouseFollowController] δ�ҵ���ǩΪ '{targetWindowTag}' ��Ŀ�괰�ڣ�");
        }
    }

    [Button("ִ�п���")]
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
        // �״β���Ŀ��
        FindTarget();
    }

    private void OnEnable()
    {
        // ���������ÿ�μ���ʱ���²��ң����ߵ�ǰû��Ŀ��
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
            Debug.LogWarning("[MouseFollowController] Ŀ�괰�ڱ�ǩΪ�գ�");
            return null;
        }

        GameObject taggedObj = GameObject.FindGameObjectWithTag(targetWindowTag);
        if (taggedObj != null)
        {
            DraggableUIWindow window = taggedObj.GetComponent<DraggableUIWindow>();
            if (window == null)
            {
                Debug.LogWarning($"[MouseFollowController] ��ǩΪ '{targetWindowTag}' �Ķ���û��DraggableUIWindow�����");
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
            Debug.LogWarning($"[MouseFollowController] δ�ҵ���ǩΪ '{targetWindowTag}' ��Ŀ�괰�ڣ�����: {gameObject.name}");
            return;
        }

        target.followMouse = true;
        target.mouseOffset = mouseOffset;
        isWindowFollowing = true;

        Debug.Log($"[MouseFollowController] �ѿ������� '{target.name}' �������湦��");
    }

    private void DisableFollowMouse()
    {
        DraggableUIWindow target = currentTarget ?? GetTargetWindow();

        if (target == null)
        {
            Debug.LogWarning($"[MouseFollowController] δ�ҵ���ǩΪ '{targetWindowTag}' ��Ŀ�괰�ڣ�����: {gameObject.name}");
            return;
        }

        // ���ѡ�񱣳ֵ�ǰλ�ã���¼��ǰλ��
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

        // ���ѡ�񱣳ֵ�ǰλ�ã�����λ��
        if (keepCurrentPosition)
        {
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = currentPosition;
            }
        }

        Debug.Log($"[MouseFollowController] �ѹرմ��� '{target.name}' �������湦��");
    }

    private void Update()
    {
        // ���µ�����Ϣ
        if (currentTarget != null)
        {
            isWindowFollowing = currentTarget.followMouse;
        }
    }
}