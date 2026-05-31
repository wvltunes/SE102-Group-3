using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonBehaviour : MonoBehaviour
{
    [Tooltip("Name of the main menu scene to return to (must be in Build Settings).")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public void HomeButtonOnClick()
    {
        // The game is frozen on game-over, so restore normal time before leaving,
        // otherwise the main menu would load while still paused.
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
