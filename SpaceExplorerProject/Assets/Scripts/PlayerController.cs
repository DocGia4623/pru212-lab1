using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;

    [Header("Laser")]
    public GameObject laserPrefab;
    public Transform laserSpawnPosition;
    public float laserDuration = 0.5f;
    public LayerMask enemyLayer = -1;
    public Transform muzzleSpawnPosition;
    public AudioClip shootSfx; // Thêm AudioClip cho hiệu ứng bắn
    private AudioSource audioSource; // Thêm AudioSource

    [Header("Health")]
    public int maxLives = 3;
    public GameObject heartIconPrefab; // Icon trái tim
    public Transform healthIconsPanel; // Panel chứa icon

    private int currentLives;
    private bool isDead = false;
    private bool isInvincible = false;
    private List<GameObject> heartIcons = new List<GameObject>();

    private void Start()
    {
        currentLives = maxLives;
        CreateHealthIcons();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
            // Only subtract points if score will not go below zero
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

            // 10% chance to gain 1 life (if not at max)
            if (Random.value < 0.7f && currentLives < maxLives)
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
        Debug.Log("Player Destroyed by Enemy");

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

        // Flashing effect while invincible
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float elapsed = 0f;
        float invincibleDuration = 1f;
        float flashInterval = 0.15f;
        while (elapsed < invincibleDuration)
        {
            if (sr != null)
                sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }
        if (sr != null)
            sr.enabled = true;

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
