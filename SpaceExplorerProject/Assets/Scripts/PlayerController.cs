using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;

    // Laser
    public GameObject laserPrefab;
    public Transform laserSpawnPosition;
    public float laserDuration = 0.5f;
    public LayerMask enemyLayer = -1;
    public Transform muzzleSpawnPosition;
    public AudioClip shootSfx;
    private AudioSource audioSource;

    // Health
    public int maxLives = 3;
    public GameObject heartIconPrefab;
    public Transform healthIconsPanel;

    // Smoke trail
    public ParticleSystem smokeTrail;

    private int currentLives;
    private bool isDead = false;
    private bool isInvincible = false;
    private List<GameObject> heartIcons = new List<GameObject>();

    private Camera mainCam;

    private void Start()
    {
        currentLives = maxLives;
        CreateHealthIcons();

        // Ensure there is an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        mainCam = Camera.main;
    }

    private void Update()
    {
        if (!isDead)
        {
            PlayerMovement();
            PlayerShoot();
        }
    }

    void PlayerMovement()
    {
        float xPos = Input.GetAxis("Horizontal");
        float yPos = Input.GetAxis("Vertical"); 
        Vector3 movement = new Vector3(xPos, yPos, 0) * speed * Time.deltaTime;
        transform.Translate(movement);

        // Clamp player position to camera bounds
        if (mainCam != null)
        {
            Vector3 pos = transform.position;
            Vector3 min = mainCam.ViewportToWorldPoint(new Vector3(0, 0, mainCam.nearClipPlane));
            Vector3 max = mainCam.ViewportToWorldPoint(new Vector3(1, 1, mainCam.nearClipPlane));

            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.y = Mathf.Clamp(pos.y, min.y, max.y);
            transform.position = pos;
        }

        // Play or stop smoke trail based on movement
        if (smokeTrail != null)
        {
            if (movement != Vector3.zero && !smokeTrail.isPlaying)
            {
                smokeTrail.Play();
            }
            else if (movement == Vector3.zero && smokeTrail.isPlaying)
            {
                smokeTrail.Stop();
            }
        }
    }

    void PlayerShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootLaser();
            SpawnMuzzleFlash();
            PlayShootSfx();
        }
    }

    void PlayShootSfx()
    {
        if (shootSfx != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSfx);
        }
    }

    void ShootLaser()
    {
        GameObject laser = Instantiate(laserPrefab, laserSpawnPosition.position, laserSpawnPosition.rotation);
        laser.transform.SetParent(null);
        Destroy(laser, laserDuration);

        RaycastHit2D[] hits = Physics2D.RaycastAll(laserSpawnPosition.position, Vector2.up, 20f, enemyLayer);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                GameObject explosion = Instantiate(GameManager.instance.explosion, hit.point, Quaternion.identity);
                Destroy(explosion, 2f);
                Destroy(hit.collider.gameObject);
                GameManager.instance.AddScore(1);
            }
        }
    }

    void SpawnMuzzleFlash()
    {
        GameObject muzzle = Instantiate(GameManager.instance.muzzleFlash, muzzleSpawnPosition);
        muzzle.transform.SetParent(null);
        Destroy(muzzle, 0.2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isDead && !isInvincible)
        {
            // Reduce score using reflection
            if (GameManager.instance != null)
            {
                int currentScore = typeof(GameManager)
                    .GetField("score", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(GameManager.instance) as int? ?? 0;

                int newScore = currentScore - 5;
                GameManager.instance.AddScore(newScore < 0 ? -currentScore : -5);
            }

            DieAndRespawn();
        }

        if (collision.gameObject.CompareTag("Star"))
        {
            GameManager.instance.AddScore(10);

            // 25% chance to gain a heart
            if (Random.value < 0.25f && currentLives < maxLives)
            {
                currentLives++;
                UpdateHealthIcons();
            }

            Destroy(collision.gameObject);
        }
    }

    void DieAndRespawn()
    {
        isDead = true;
        currentLives--;
        UpdateHealthIcons();

        GameObject explosion = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
        Destroy(explosion, 2f);
        gameObject.SetActive(false);

        if (currentLives > 0)
        {
            Invoke(nameof(Respawn), 3f);
        }
        else
        {
            GameManager.instance.GameOver();
        }
    }

    void Respawn()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
        isDead = false;
        StartCoroutine(InvincibilityCoroutine());
    }

    System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float elapsed = 0f;
        float invincibleDuration = 1f;
        float flashInterval = 0.15f;

        while (elapsed < invincibleDuration)
        {
            if (sr != null)
            {
                sr.enabled = !sr.enabled;
            }
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        if (sr != null)
        {
            sr.enabled = true;
        }

        isInvincible = false;
    }

    void CreateHealthIcons()
    {
        for (int i = 0; i < maxLives; i++)
        {
            GameObject icon = Instantiate(heartIconPrefab, healthIconsPanel);
            heartIcons.Add(icon);
        }
    }

    void UpdateHealthIcons()
    {
        for (int i = 0; i < heartIcons.Count; i++)
        {
            heartIcons[i].SetActive(i < currentLives);
        }
    }
}
