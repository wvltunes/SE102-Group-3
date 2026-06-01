using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game-state authority for a gameplay scene.
///
/// Listens to the player-death and level-complete events and drives the matching
/// state transition (pausing the game, stopping audio, and exposing helpers to
/// reload or advance the level). Lives in the scene as a single instance; place
/// one GameManager in every gameplay scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The high-level states the game can be in during a level.
    /// </summary>
    public enum GameState
    {
        Playing,        // Normal gameplay.
        GameOver,       // Player died - awaiting restart.
        LevelComplete   // Player reached the finish - awaiting next level.
    }

    /// <summary>
    /// Singleton access point. Set in <see cref="Awake"/> and cleared in
    /// <see cref="OnDestroy"/> so it never points at a destroyed object.
    /// </summary>
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// The current game state. Readable everywhere, writable only here.
    /// </summary>
    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        // Enforce a single instance per scene.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Time.timeScale persists across scene loads. If the previous scene was
        // paused on game-over and then reloaded, make sure gameplay starts
        // running again here.
        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
    }

    private void OnEnable()
    {
        // Subscribe in OnEnable so subscriptions are always balanced by the
        // matching OnDisable below - this is what prevents leaked listeners.
        PlayerController.OnPlayerDeath += HandlePlayerDeath;
        LevelCompleteZone.OnLevelComplete += HandleLevelComplete;
    }

    private void OnDisable()
    {
        // Always unsubscribe so a destroyed GameManager never keeps a stale
        // listener registered on the static events.
        PlayerController.OnPlayerDeath -= HandlePlayerDeath;
        LevelCompleteZone.OnLevelComplete -= HandleLevelComplete;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // --- Event handlers -----------------------------------------------------

    private void HandlePlayerDeath()
    {
        SetState(GameState.GameOver);
    }

    private void HandleLevelComplete()
    {
        SetState(GameState.LevelComplete);
    }

    // --- State machine ------------------------------------------------------

    /// <summary>
    /// Transitions to <paramref name="newState"/> and runs the side effects for
    /// that state. Ignores no-op transitions to the current state.
    /// </summary>
    private void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        switch (newState)
        {
            case GameState.GameOver:
                EnterGameOver();
                break;

            case GameState.LevelComplete:
                EnterLevelComplete();
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                break;
        }
    }

    private void EnterGameOver()
    {
        Debug.Log("[GameManager] State changed to GameOver.");

        // Freeze all time-based gameplay (movement, spawners, tweens).
        Time.timeScale = 0f;

        // Audio is not affected by timeScale, so pause it explicitly.
        if (AudioManager.instance != null)
        {
            AudioManager.instance.Pause();
        }

        // The scene is now ready to be reloaded via RestartLevel(); a game-over
        // UI can read CurrentState (or listen to PlayerController.OnPlayerDeath)
        // and call that helper when the player chooses to retry.
    }

    private void EnterLevelComplete()
    {
        Debug.Log("[GameManager] State changed to LevelComplete.");

        // Stop gameplay so nothing moves while the "level cleared" UI is shown.
        Time.timeScale = 0f;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.Pause();
        }

        // A level-complete UI can read CurrentState and call LoadNextLevel()
        // (or RestartLevel()) once the player is ready to continue.
    }

    // --- Public scene helpers ----------------------------------------------

    /// <summary>
    /// Reloads the current scene from the start with fade transition.
    /// Uses SceneTransitionManager to handle the fade effect.
    /// </summary>
    public void RestartLevel()
    {
        SceneTransitionManager.ReloadCurrentLevel();
    }

    /// <summary>
    /// Loads the next scene in the Build Settings order with fade transition.
    /// If the current scene is the last one, it reloads the current scene instead
    /// and logs a warning. Uses SceneTransitionManager to handle the fade effect.
    /// </summary>
    public void LoadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneTransitionManager.LoadLevel(nextIndex);
        }
        else
        {
            Debug.LogWarning(
                "[GameManager] No next level found in Build Settings. Reloading the current scene.");
            SceneTransitionManager.ReloadCurrentLevel();
        }
    }
}
