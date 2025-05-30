using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // Add this line

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public float minInstantiateValue;
    public float maxInstantiateValue;
    public float enemyDestroyTime = 10f;
    private float spawnRate = 2f;
    private float spawnRateMultiplier = 1f;
    private float maxMultiplier = 3f;

    [Header("Star Settings")]
    public GameObject starPrefab;
    public float starSpawnInterval = 7f;

    [Header("Particle Effects")]
    public GameObject explosion;
    public GameObject muzzleFlash;

    [Header("UI Panels")]
    public GameObject pauseMenu;
    public TextMeshProUGUI scoreText;

    [Header("Audio")]
    public AudioSource backgroundMusic;

    private int score = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
        scoreText.text = "Score: 0";
        StartCoroutine(SpawnEnemies());
        StartCoroutine(IncreaseSpawnRate());
        StartCoroutine(SpawnStars());

        // Play background music if assigned
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(true);
        }
    }

    IEnumerator SpawnStars()
    {
        while (true)
        {
            Vector3 spawnPos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 6f);
            float randomAngle = Random.Range(-20f, 20f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, 180f + randomAngle);
            GameObject star = Instantiate(starPrefab, spawnPos, rotation);
            Destroy(star, enemyDestroyTime); // Dùng enemyDestroyTime hoặc tạo biến riêng cho star
            yield return new WaitForSeconds(starSpawnInterval);
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            InstantiateEnemy();
            yield return new WaitForSeconds(spawnRate / spawnRateMultiplier);
        }
    }

    IEnumerator IncreaseSpawnRate()
    {
        while (spawnRateMultiplier < maxMultiplier)
        {
            yield return new WaitForSeconds(10f);
            spawnRateMultiplier = Mathf.Min(spawnRateMultiplier + 0.2f, maxMultiplier);
        }
    }

    void InstantiateEnemy()
    {
        Vector3 spawnPos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 6f);
        float randomAngle = Random.Range(-20f, 20f);
        Quaternion rotation = Quaternion.Euler(0f, 0f, 180f + randomAngle);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, rotation);
        Destroy(enemy, enemyDestroyTime);
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    public void PauseGame(bool isPaused)
    {
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        PlayerPrefs.SetInt("LastScore", score); // Lưu điểm hiện tại
        PlayerPrefs.Save();
        Time.timeScale = 1f;

        // Stop background music before changing scene
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }

        SceneManager.LoadScene(2); // EndGame scene
    }

    public void QuitGame()
    {
        // Stop background music if playing
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }
        Application.Quit();
    }
}