using System.Linq;
using UnityEngine;

public class ClickPriority : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = 10f; // 设置 z 值为摄像机与物体的距离
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            //Debug.Log("Mouse Clicked at: " + mousePosition);

            // 射线检测
            var clickedObjects = Physics2D.RaycastAll(mousePosition, Vector2.zero);

            if (clickedObjects.Length == 0)
            {
                //Debug.Log("No objects were hit.");
                return;
            }

            // 输出所有被点击到的物体及其排序信息
            foreach (var hit in clickedObjects)
            {
                var spriteRenderer = hit.collider.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Debug.Log("Hit object: " + hit.collider.name + ", sortingLayer: " + spriteRenderer.sortingLayerName +
                              ", orderInLayer: " + spriteRenderer.sortingOrder);
                }
            }

            // 按照 `SpriteRenderer` 的排序信息进行排序
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
