using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject tutorialCanvas; // Assign your Tutorial Canvas in the Inspector
    public GameObject mainMenuCanvas; // Assign your Main Menu Canvas in the Inspector

    public void StartGame()
    {
        // Load the game scene
        SceneManager.LoadScene(1); // Replace "GameScene" with the name of your game scene
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }

    public void OpenTutorial()
    {
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
        }
        if (tutorialCanvas != null)
        {
            tutorialCanvas.SetActive(true);
        }
    }

    public void CloseTutorial()
    {
        if (tutorialCanvas != null)
        {
            tutorialCanvas.SetActive(false);
        }
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
        }
    }
}