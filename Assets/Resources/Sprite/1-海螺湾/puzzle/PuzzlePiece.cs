using UnityEngine;
public class PuzzlePiece : MonoBehaviour
{
    public Transform currentSlot;
    public int correctSlotIndex; // ��ȷλ�õ�����
    private Vector3 offset;

    public void OnDragStart(Vector3 worldPos)
    {
        offset = transform.position - worldPos;
    }

    public void OnDragging(Vector3 worldPos)
    {
        transform.position = worldPos + offset;
    }

    public void OnDragEnd()
    {
        Transform nearestSlot = PuzzleManager.Instance.GetNearestSlot(transform.position);
        if (nearestSlot != null)
        {
            PuzzlePiece other = PuzzleManager.Instance.GetPieceInSlot(nearestSlot);
            if (other != null && other != this)
            {
                Transform temp = currentSlot;
                MoveToSlot(nearestSlot);
                other.MoveToSlot(temp);
            }
            else
            {
                MoveToSlot(nearestSlot);
            }

            // ���ƴͼ�Ƿ����
            PuzzleManager.Instance.CheckPuzzleCompletion();
        }
    }

    public void MoveToSlot(Transform slot)
    {
        currentSlot = slot;
        transform.position = slot.position;
    }
}