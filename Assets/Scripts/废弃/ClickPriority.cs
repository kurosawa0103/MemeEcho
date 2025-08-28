using System.Linq;
using UnityEngine;

public class ClickPriority : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = 10f; // ���� z ֵΪ�����������ľ���
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            //Debug.Log("Mouse Clicked at: " + mousePosition);

            // ���߼��
            var clickedObjects = Physics2D.RaycastAll(mousePosition, Vector2.zero);

            if (clickedObjects.Length == 0)
            {
                //Debug.Log("No objects were hit.");
                return;
            }

            // ������б�����������弰��������Ϣ
            foreach (var hit in clickedObjects)
            {
                var spriteRenderer = hit.collider.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Debug.Log("Hit object: " + hit.collider.name + ", sortingLayer: " + spriteRenderer.sortingLayerName +
                              ", orderInLayer: " + spriteRenderer.sortingOrder);
                }
            }

            // ���� `SpriteRenderer` ��������Ϣ��������
            var highestPriorityObject = clickedObjects
                .Select(hit => hit.collider.gameObject)
                .Where(go => go.GetComponent<SpriteRenderer>() != null)
                .OrderBy(go => go.GetComponent<SpriteRenderer>().sortingLayerID)
                .ThenBy(go => go.GetComponent<SpriteRenderer>().sortingOrder)
                .FirstOrDefault();

            if (highestPriorityObject != null)
            {
                Debug.Log("Highest priority object: " + highestPriorityObject.name);
                highestPriorityObject.SendMessage("OnMouseClick", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
