using UnityEngine;

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
            // Fallback when no GameManager is present: SceneTransitionManager restores
            // time (the game may be paused) and reloads the current scene.
            SceneTransitionManager.ReloadCurrentLevel();
        }
    }
}
