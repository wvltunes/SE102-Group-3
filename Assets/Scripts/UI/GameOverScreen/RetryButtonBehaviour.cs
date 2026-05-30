using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButtonBehaviour : MonoBehaviour
{
    public void RetryButtonOnClick()
    {
        if (GameManager.Instance != null)
        {
            // Preferred path: let the GameManager reset time and reload the level.
            GameManager.Instance.RestartLevel();
        }
        else
        {
            // Fallback when no GameManager is present: restore time (the game may
            // be paused) and reload the current scene manually.
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
