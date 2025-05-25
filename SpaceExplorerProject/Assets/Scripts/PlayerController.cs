using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;

    [Header("Missile")]

    public GameObject missile;
    public Transform missileSpawnPosition;
    public float destroyTime = 5f;
    public Transform muzzleSpawnPosition;

    private void Update()
    {
        PlayerMovement();
        PlayerShoot();
    }
    void PlayerMovement()
    {
        float xPos = Input.GetAxis("Horizontal");
        float yPos = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(xPos, yPos, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    void PlayerShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnMissle();
            SpawnMuzzleFlash();
        }

    }
    void SpawnMissle()
    {
        // Assuming you have a method to handle shooting
        GameObject gm = Instantiate(missile, missileSpawnPosition);
        gm.transform.SetParent(null);
        Destroy(gm, destroyTime);
    }
    void SpawnMuzzleFlash()
    {
        GameObject muzzle = Instantiate(GameManager.instance.muzzleFlash, muzzleSpawnPosition);
        muzzle.transform.SetParent(null);
        Destroy(muzzle, destroyTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject gm = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(gm, 2f);
            Destroy(this.gameObject);
            Debug.Log("Player Destroyed by Enemy");
        }
    }
}
