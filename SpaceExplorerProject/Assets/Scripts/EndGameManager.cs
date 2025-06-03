using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;

    public LeaderboardEntry(string name, int playerScore)
    {
        playerName = name;
        score = playerScore;
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class EndGameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;
    public GameObject nameInputPanel;
    public GameObject leaderboardPanel;

    [Header("Game Over UI")]
    public Text currentScoreText;
    public Text gameOverMessageText;

    [Header("Name Input UI")]
    public InputField playerNameInput;
    public Button submitNameButton;
    public Button skipButton;
    public Text nameInputPromptText;

    [Header("Leaderboard UI")]
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;
    public Text[] leaderboardTexts;

    [Header("Navigation Buttons")]
    public Button playAgainButton;
    public Button exitButton;
    public Button viewLeaderboardButton;
    public Button backFromLeaderboardButton;

    private int currentScore;
    private LeaderboardData leaderboard;
    private string leaderboardFilePath;
    private bool isNewHighScore = false;

    void Start()
    {
        InitializeLeaderboard();
        SetupUI();
        CheckForHighScore();
    }

    void InitializeLeaderboard()
    {
        leaderboardFilePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        LoadLeaderboard();
        currentScore = PlayerPrefs.GetInt("LastScore", 0);
    }

    void SetupUI()
    {
        RegisterButtonListeners();

        nameInputPanel?.SetActive(false);
        leaderboardPanel?.SetActive(false);
        gameOverPanel?.SetActive(true);

        if (currentScoreText != null)
            currentScoreText.text = "Score: " + currentScore.ToString();
    }

    void RegisterButtonListeners()
    {
        submitNameButton?.onClick.AddListener(SubmitPlayerName);
        skipButton?.onClick.AddListener(SkipNameEntry);
        playAgainButton?.onClick.AddListener(PlayAgain);
        exitButton?.onClick.AddListener(Exit);
        viewLeaderboardButton?.onClick.AddListener(ShowLeaderboard);
        backFromLeaderboardButton?.onClick.AddListener(HideLeaderboard);
    }

    void CheckForHighScore()
    {
        isNewHighScore = IsTopFiveScore(currentScore);

        if (isNewHighScore)
        {
            gameOverMessageText?.SetText("New High Score!");
            nameInputPromptText?.SetText(
                $"Congratulations! You made it to the top 5 with {currentScore} points!\nEnter your name:");
            Invoke(nameof(ShowNameInput), 2f);
        }
        else
        {
            gameOverMessageText?.SetText("Game Over");
        }
    }

    bool IsTopFiveScore(int score)
    {
        if (leaderboard.entries.Count < 5) return true;
        return score > leaderboard.entries.OrderByDescending(e => e.score).ElementAt(4).score;
    }

    void ShowNameInput()
    {
        gameOverPanel?.SetActive(false);
        nameInputPanel?.SetActive(true);

        if (playerNameInput != null)
        {
            playerNameInput.text = "";
            playerNameInput.Select();
            playerNameInput.ActivateInputField();
        }
    }

    public void SubmitPlayerName()
    {
        // Prevent double submission by disabling the button immediately
        submitNameButton.interactable = false;

        string playerName = GetValidatedPlayerName();

        // Only add to leaderboard if not already added
        if (isNewHighScore)
        {
            AddToLeaderboard(playerName, currentScore);
            SaveLeaderboard();
            isNewHighScore = false; // Prevent further submissions for this score
        }

        nameInputPanel?.SetActive(false);
        ShowLeaderboard();
    }

    string GetValidatedPlayerName()
    {
        string playerName = playerNameInput?.text.Trim() ?? "Anonymous";

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Anonymous";
        }

        return playerName.Length > 15 ? playerName.Substring(0, 15) : playerName;
    }

    public void SkipNameEntry()
    {
        // Prevent double submission by disabling the button immediately
        skipButton.interactable = false;

        // Only add to leaderboard if not already added
        if (isNewHighScore)
        {
            AddToLeaderboard("Anonymous", currentScore);
            SaveLeaderboard();
            isNewHighScore = false; // Prevent further submissions for this score
        }

        nameInputPanel?.SetActive(false);
        gameOverPanel?.SetActive(true);
    }

    void AddToLeaderboard(string playerName, int score)
    {
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, score);
        leaderboard.entries.Add(newEntry);

        leaderboard.entries = leaderboard.entries
            .OrderByDescending(e => e.score)
            .Take(5)
            .ToList();
    }

    void LoadLeaderboard()
    {
        if (File.Exists(leaderboardFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(leaderboardFilePath);
                leaderboard = JsonUtility.FromJson<LeaderboardData>(jsonData) ?? new LeaderboardData();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading leaderboard: " + e.Message);
                leaderboard = new LeaderboardData();
            }
        }
        else
        {
            leaderboard = new LeaderboardData();
        }
    }

    void SaveLeaderboard()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(leaderboard, true);
            File.WriteAllText(leaderboardFilePath, jsonData);
            Debug.Log("Leaderboard saved to: " + leaderboardFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving leaderboard: " + e.Message);
        }
    }

    public void ShowLeaderboard()
    {
        gameOverPanel?.SetActive(false);
        leaderboardPanel?.SetActive(true);
        UpdateLeaderboardDisplay();
    }

    public void HideLeaderboard()
    {
        leaderboardPanel?.SetActive(false);
        gameOverPanel?.SetActive(true);
    }

    void UpdateLeaderboardDisplay()
    {
        var sortedEntries = leaderboard.entries.OrderByDescending(e => e.score).ToList();

        if (leaderboardTexts != null && leaderboardTexts.Length >= 5)
        {
            for (int i = 0; i < 5; i++)
            {
                leaderboardTexts[i]?.SetText(i < sortedEntries.Count
                    ? $"{i + 1}. {sortedEntries[i].playerName} - {sortedEntries[i].score}"
                    : $"{i + 1}. --- - ---");
            }
        }
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}

public static class TextExtension
{
    public static void SetText(this Text textComponent, string value)
    {
        if (textComponent != null)
        {
            textComponent.text = value;
        }
    }
}