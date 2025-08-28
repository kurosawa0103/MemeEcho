using UnityEngine;

public class PuzzleDragController : MonoBehaviour
{
    private PuzzlePiece selectedPiece;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world = GetMouseWorldPos();
            RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero);

            if (hit.collider != null)
            {
                selectedPiece = hit.collider.GetComponent<PuzzlePiece>();
                if (selectedPiece != null)
                    selectedPiece.OnDragStart(world);
            }
        }

        if (Input.GetMouseButton(0) && selectedPiece != null)
        {
            Vector3 world = GetMouseWorldPos();
            selectedPiece.OnDragging(world);
        }

        if (Input.GetMouseButtonUp(0) && selectedPiece != null)
        {
            selectedPiece.OnDragEnd();
            selectedPiece = null;
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 screen = Input.mousePosition;
        screen.z = Mathf.Abs(Camera.main.transform.position.z); // 保证透视正确
        return Camera.main.ScreenToWorldPoint(screen);
    }
}