using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject enemyPrefab;
    public float minInstantiateValue;
    public float maxInstantiateValue;
    public float enemyDestroyTime = 10f;

    [Header("Particle Effects")]
    public GameObject explosion;
    public GameObject muzzleFlash;

    [Header("Panels")]
    public GameObject StartMenu;
    public GameObject pauseMenu;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartMenu.SetActive(true);
        pauseMenu.SetActive(false);
        Time.timeScale = 0f; // Pause the game at the start
        InvokeRepeating("InstantiateEnemy", 1f, 2f);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !StartMenu.activeSelf)
        {
            PauseGame(true);

        }
    }
    void InstantiateEnemy()
    {
        Vector3 enemypos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 6f);
        GameObject enemy = Instantiate(enemyPrefab, enemypos, Quaternion.Euler(0f, 0f, 180f));
        Destroy(enemy, enemyDestroyTime);
    }
    public void StartGameButton()
    {
        StartMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    public void PauseGame(bool isPaused)
    {
        if (isPaused)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
