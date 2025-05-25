using UnityEngine;

public class MissileController : MonoBehaviour
{
    public float missleSpeed = 25f;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * missleSpeed * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject gm = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(gm, 2f);
            Destroy(collision.gameObject);
            Destroy(gameObject);                 
        }
    }
}
