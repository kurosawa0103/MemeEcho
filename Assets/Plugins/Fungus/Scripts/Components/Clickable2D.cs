using UnityEngine;
using UnityEngine.EventSystems;

namespace Fungus
{
    /// <summary>
    /// Detects mouse clicks and touches on a Game Object, and sends an event to all Flowchart event handlers in the scene.
    /// The Game Object must have a Collider or Collider2D component attached.
    /// Use in conjunction with the ObjectClicked Flowchart event handler.
    /// </summary>
    /// 
    public enum AllowedClickAreaType
    {
        Root,
        SubRoot
    }
    public class Clickable2D : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("Is object clicking enabled")]
        [SerializeField] protected bool clickEnabled = true;

        [Tooltip("Mouse texture to use when hovering mouse over object")]
        [SerializeField] protected Texture2D hoverCursor;

        [Tooltip("Use the UI Event System to check for clicks. Clicks that hit an overlapping UI object will be ignored. Camera must have a PhysicsRaycaster component, or a Physics2DRaycaster for 2D colliders.")]
        [HideInInspector]
        [SerializeField] protected bool useEventSystem;
        
        [SerializeField] private AllowedClickAreaType allowedAreaType = AllowedClickAreaType.Root;
        public RectTransform allowedClickArea;

        protected bool isPressedDownOnMe = false;
        protected bool isMouseOverMe = false;


        private void Start()
        {
            // �Զ����Ҷ�ӦTag��UI������Ϊ allowedClickArea
            if (allowedClickArea == null)
            {
                string tagToFind = allowedAreaType == AllowedClickAreaType.Root ? "Root" :
                                   allowedAreaType == AllowedClickAreaType.SubRoot ? "SubRoot" : null;

                if (!string.IsNullOrEmpty(tagToFind))
                {
                    GameObject foundObj = GameObject.FindWithTag(tagToFind);
                    if (foundObj != null)
                    {
                        allowedClickArea = foundObj.GetComponent<RectTransform>();
                        if (allowedClickArea == null)
                            Debug.LogWarning($"TagΪ {tagToFind} ������û�� RectTransform �����");
                    }
                    else
                    {
                        Debug.LogWarning($"�Ҳ��� Tag Ϊ {tagToFind} �����壡");
                    }
                }
            }
        }
        private RectTransform FindRectTransformByTag(string tag)
        {
            GameObject obj = GameObject.FindWithTag(tag);
            if (obj != null)
            {
                var rt = obj.GetComponent<RectTransform>();
                if (rt != null)
                    return rt;
                else
                    Debug.LogWarning($"TagΪ{tag}������û��RectTransform�����");
            }
            else
            {
                Debug.LogWarning($"�Ҳ���TagΪ{tag}�����壡");
            }
            return null;
        }

        protected virtual void ChangeCursor(Texture2D cursorTexture)
        {
            if (!clickEnabled)
                return;

            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }

        protected virtual void RaiseClickEvent(ObjectClicked.ClickPhase phase)
        {
            if (!clickEnabled)
                return;

            ObjectClicked.RaiseClickEvent(this, phase);
        }

        protected virtual void DoPointerEnter()
        {
            ChangeCursor(hoverCursor);
            ObjectHovered.RaiseHoverEvent(this, ObjectHovered.HoverPhase.MouseEnter);
        }

        protected virtual void DoPointerExit()
        {
            SetMouseCursor.ResetMouseCursor();
            ObjectHovered.RaiseHoverEvent(this, ObjectHovered.HoverPhase.MouseExit);
        }

        #region Legacy OnMouseX methods

        protected virtual void OnMouseDown()
        {
            if (!useEventSystem)
            {
                if ( !IsPointerInsideAllowedArea())
                {
                    return;
                }

                isPressedDownOnMe = true;
                RaiseClickEvent(ObjectClicked.ClickPhase.MouseDown);
            }
        }

        protected virtual void OnMouseUp()
        {
            if (!useEventSystem)
            {
                isMouseOverMe = IsPointerOverThis(); // ������ ������У�ǿ���жϵ�ǰ����ǲ��ǻ�������

                bool insideAllowed = IsPointerInsideAllowedArea();

                if (!insideAllowed)
                {
                    isPressedDownOnMe = false;
                    return;
                }

                if (isPressedDownOnMe && isMouseOverMe)
                {
                    RaiseClickEvent(ObjectClicked.ClickPhase.MouseUp);
                }

                isPressedDownOnMe = false;
            }
        }


        protected virtual void OnMouseEnter()
        {
            if (!useEventSystem && IsPointerInsideAllowedArea())
            {
                isMouseOverMe = true; // ��������
                DoPointerEnter();
            }
        }

        protected virtual void OnMouseExit()
        {
            if (!useEventSystem && IsPointerInsideAllowedArea())
            {
                isMouseOverMe = false; // ����뿪��
                DoPointerExit();
            }
        }
        protected virtual bool IsPointerInsideAllowedArea()
        {
            if (allowedClickArea == null)
                return true; // ���û�������򣬾�Ĭ�ϲ�����

            Vector2 mousePos = Input.mousePosition;
            return RectTransformUtility.RectangleContainsScreenPoint(allowedClickArea, mousePos, null);
        }

        //��⵱ǰ����ǲ��ǻ����Լ�����
        protected virtual bool IsPointerOverThis()
        {
            Camera cam = Camera.main;
            if (cam == null) return false;

            Vector2 mousePos = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(mousePos);

            RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit2D.collider != null && hit2D.collider.gameObject == this.gameObject)
            {
                return true;
            }

            RaycastHit hit3D;
            if (Physics.Raycast(ray, out hit3D))
            {
                if (hit3D.collider.gameObject == this.gameObject)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Public members

        public bool ClickEnabled { set { clickEnabled = value; } }

        #endregion

        #region IPointer Interfaces (for UI events)

        public void OnPointerClick(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                RaiseClickEvent(ObjectClicked.ClickPhase.MouseUp); // UI���Ĭ����MouseUp
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoPointerEnter();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoPointerExit();
            }
        }

        #endregion
    }
}
