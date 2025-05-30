using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public GameObject background1; // Assign first background image
    public GameObject background2; // Assign second background image
    public float scrollSpeed = 2f;

    private float imageHeight;

    void Start()
    {
        if (background1 == null || background2 == null)
        {
            Debug.LogError("Assign both background images in the Inspector.");
            enabled = false;
            return;
        }

        // Get the height of the background image (assumes both are the same size)
        SpriteRenderer sr = background1.GetComponent<SpriteRenderer>();
        if (sr != null)
            imageHeight = sr.bounds.size.y;
        else
            imageHeight = background1.GetComponent<RectTransform>().rect.height;
    }

    void Update()
    {
        // Move both backgrounds downward
        background1.transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);
        background2.transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        // If a background moves completely off screen, move it to the top
        if (background1.transform.position.y <= -imageHeight)
        {
            background1.transform.position = new Vector3(
                background1.transform.position.x,
                background2.transform.position.y + imageHeight,
                background1.transform.position.z
            );
        }
        if (background2.transform.position.y <= -imageHeight)
        {
            background2.transform.position = new Vector3(
                background2.transform.position.x,
                background1.transform.position.y + imageHeight,
                background2.transform.position.z
            );
        }
    }
}