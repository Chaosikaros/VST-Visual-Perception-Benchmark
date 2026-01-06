using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SnapToGrid : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public float gridSize = 10f;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Disable layout element to allow free movement
        var layoutElement = GetComponent<LayoutElement>();
        if (layoutElement != null)
        {
            layoutElement.ignoreLayout = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Snap to grid during drag
        transform.position = new Vector3(
            Mathf.Round(eventData.position.x / gridSize) * gridSize,
            Mathf.Round(eventData.position.y / gridSize) * gridSize,
            transform.position.z
        );
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Enable layout element to restore original position
        var layoutElement = GetComponent<LayoutElement>();
        if (layoutElement != null)
        {
            layoutElement.ignoreLayout = false;
        }
    }
}
