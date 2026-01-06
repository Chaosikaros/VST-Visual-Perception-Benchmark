using UnityEngine;

public class ScreenSpaceSize : MonoBehaviour
{
    public GameObject target;
    public void getRect()
    {
        Camera camera = Camera.main;
        Renderer renderer = target.GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;

        Vector3 minPoint = camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z));
        Vector3 maxPoint = camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.max.z));

        Rect rect = new Rect(minPoint.x, minPoint.y, maxPoint.x - minPoint.x, maxPoint.y - minPoint.y);
        Debug.Log("Screen Space Size: " + rect.width + "x" + rect.height);
    }
    void Update()
    {
        Camera camera = Camera.main;
        Renderer renderer = target.GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;

        Vector3 minPoint = camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z));
        Vector3 maxPoint = camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.max.z));

        Rect rect = new Rect(minPoint.x, minPoint.y, maxPoint.x - minPoint.x, maxPoint.y - minPoint.y);
        Debug.Log("Screen Space Size: " + rect.width + "x" + rect.height);
    }
}
