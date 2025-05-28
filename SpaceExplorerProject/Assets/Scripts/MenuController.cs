using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
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
}