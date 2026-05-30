using UnityEngine;

/// <summary>
/// Shows/hides the game-over UI. The screen is activated automatically when the
/// player dies (via <see cref="PlayerController.OnPlayerDeath"/>) and can also be
/// toggled manually with the Escape key for debugging.
/// </summary>
public class GameOverScreenController : MonoBehaviour
{
    public static bool gameOver = false;
    [SerializeField] private GameObject gameOverScreen;

    private void Awake()
    {
        // Static fields survive scene reloads, so reset the flag here to make
        // sure a freshly loaded scene never starts in a stale "game over" state.
        gameOver = false;
    }

    private void OnEnable()
    {
        // Show the game-over screen the moment the player dies.
        PlayerController.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        // Always balance the subscription to avoid leaking a listener on the
        // static event when this object is disabled or destroyed.
        PlayerController.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void Update()
    {
        // Secondary debug toggle: manually open/close the screen with Escape.
        // Update still runs while the game is paused (Time.timeScale == 0).
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameOver)
            {
                TriggerGameOverScreen();
            }
            else
            {
                CloseGameOverScreen();
            }
        }
    }

    private void HandlePlayerDeath()
    {
        TriggerGameOverScreen();
    }

    private void TriggerGameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
        gameOver = true;
    }

    private void CloseGameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
        gameOver = false;
    }
}
