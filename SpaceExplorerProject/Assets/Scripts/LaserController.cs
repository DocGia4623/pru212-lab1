using UnityEngine;

public class LaserController : MonoBehaviour
{
    public float laserLength = 20f;
    private LineRenderer lineRenderer;

    void Start()
    {
        // Thiết lập LineRenderer cho laser
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Cấu hình LineRenderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red; // Fixed: Use startColor
        lineRenderer.endColor = Color.red;   // Fixed: Use endColor
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;

        // Thiết lập vị trí laser
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (transform.up * laserLength);

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Tạo hiệu ứng phát sáng (optional)
        lineRenderer.sortingOrder = 1;
    }
}
