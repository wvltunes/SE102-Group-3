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
        Playing,
        Paused,
        GameOver,
        LevelComplete
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

    /// <summary>
    /// Total orbs in this level (for score tracking).
    /// Set this in the Inspector or initialize from a level data source.
    /// </summary>
    [SerializeField] private int totalOrbsInLevel = 0;

    /// <summary>
    /// Reference to orbs score display script (from Game Over canvas).
    /// </summary>
    [SerializeField] private OrbsScoreDisplay orbsScoreDisplay;

    /// <summary>
    /// Reference to song progress display script (from Game Over canvas).
    /// </summary>
    [SerializeField] private SongProgressDisplay songProgressDisplay;

    /// <summary>
    /// Reference to orbs score display script (from Level Complete canvas).
    /// </summary>
    [SerializeField] private OrbsScoreDisplay levelCompleteOrbsDisplay;

    /// <summary>
    /// Reference to song progress display script (from Level Complete canvas).
    /// </summary>
    [SerializeField] private SongProgressDisplay levelCompleteSongProgressDisplay;

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

        // Reset UI display references so they get re-discovered from the fresh scene
        orbsScoreDisplay = null;
        songProgressDisplay = null;
        levelCompleteOrbsDisplay = null;
        levelCompleteSongProgressDisplay = null;

        // Initialize score tracker
        ScoreTracker.Initialize(totalOrbsInLevel);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            LevelCompleteZone.DebugTriggerLevelComplete();
        }
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
            case GameState.Playing:
                Time.timeScale = 1f;

                if (AudioManager.instance != null)
                    AudioManager.instance.Resume();

                break;

            case GameState.Paused:
                Time.timeScale = 0f;

                if (AudioManager.instance != null)
                    AudioManager.instance.Pause();

                break;

            case GameState.GameOver:
                EnterGameOver();
                break;

            case GameState.LevelComplete:
                EnterLevelComplete();
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

        // Update score tracker and display
        ScoreTracker.UpdateSongProgress();

        // Find Game Over Canvas and get components from it
        Canvas gameOverCanvas = FindObjectOfType<Canvas>(includeInactive: true);
        if (gameOverCanvas != null && gameOverCanvas.name.Contains("Over"))
        {
            Debug.Log($"[GameManager] Found game over canvas: {gameOverCanvas.name}");
            
            // Find components in this canvas's hierarchy
            orbsScoreDisplay = gameOverCanvas.GetComponentInChildren<OrbsScoreDisplay>(includeInactive: true);
            songProgressDisplay = gameOverCanvas.GetComponentInChildren<SongProgressDisplay>(includeInactive: true);
        }

        // Fallback: search entire scene if not found in canvas
        if (orbsScoreDisplay == null)
        {
            orbsScoreDisplay = FindObjectOfType<OrbsScoreDisplay>(includeInactive: true);
        }
        if (songProgressDisplay == null)
        {
            songProgressDisplay = FindObjectOfType<SongProgressDisplay>(includeInactive: true);
        }

        if (orbsScoreDisplay != null)
        {
            orbsScoreDisplay.UpdateOrbsDisplay();
            Debug.Log($"[GameManager] Updated orbs display: {orbsScoreDisplay.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("[GameManager] OrbsScoreDisplay not found in scene!");
        }

        if (songProgressDisplay != null)
        {
            songProgressDisplay.UpdateProgressDisplay();
            Debug.Log($"[GameManager] Updated progress display: {songProgressDisplay.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("[GameManager] SongProgressDisplay not found in scene!");
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

        // Update score tracker and display
        ScoreTracker.UpdateSongProgress();

        // Find or use cached levelCompleteOrbsDisplay (search from Level Complete Canvas)
        if (levelCompleteOrbsDisplay == null)
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>(includeInactive: true);
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas.gameObject.name.Contains("Level_Complete") || canvas.gameObject.name.Contains("LevelComplete"))
                {
                    levelCompleteOrbsDisplay = canvas.GetComponentInChildren<OrbsScoreDisplay>(includeInactive: true);
                    if (levelCompleteOrbsDisplay != null)
                    {
                        Debug.Log($"[GameManager] Found level complete orbs display from canvas: {levelCompleteOrbsDisplay.gameObject.name}");
                        break;
                    }
                }
            }
            // Fallback: search all if not found
            if (levelCompleteOrbsDisplay == null)
            {
                OrbsScoreDisplay[] allDisplays = FindObjectsOfType<OrbsScoreDisplay>(includeInactive: true);
                if (allDisplays.Length > 1)
                {
                    // Find the one that's NOT on Game Over canvas
                    foreach (OrbsScoreDisplay display in allDisplays)
                    {
                        if (!display.gameObject.name.Contains("Game") || display.gameObject.name.Contains("LevelComplete"))
                        {
                            levelCompleteOrbsDisplay = display;
                            Debug.Log($"[GameManager] Found level complete orbs display (fallback): {levelCompleteOrbsDisplay.gameObject.name}");
                            break;
                        }
                    }
                }
                else if (allDisplays.Length > 0)
                {
                    levelCompleteOrbsDisplay = allDisplays[0];
                }
            }
        }

        if (levelCompleteOrbsDisplay != null)
        {
            levelCompleteOrbsDisplay.UpdateOrbsDisplay();
            Debug.Log("[GameManager] Updated orbs display on level complete.");
        }
        else
        {
            Debug.LogWarning("[GameManager] Level complete orbs display not found in scene!");
        }

        // Find or use cached levelCompleteSongProgressDisplay (search from Level Complete Canvas)
        if (levelCompleteSongProgressDisplay == null)
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>(includeInactive: true);
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas.gameObject.name.Contains("Level_Complete") || canvas.gameObject.name.Contains("LevelComplete"))
                {
                    levelCompleteSongProgressDisplay = canvas.GetComponentInChildren<SongProgressDisplay>(includeInactive: true);
                    if (levelCompleteSongProgressDisplay != null)
                    {
                        Debug.Log($"[GameManager] Found level complete progress display from canvas: {levelCompleteSongProgressDisplay.gameObject.name}");
                        break;
                    }
                }
            }
            // Fallback: search all if not found
            if (levelCompleteSongProgressDisplay == null)
            {
                SongProgressDisplay[] allDisplays = FindObjectsOfType<SongProgressDisplay>(includeInactive: true);
                if (allDisplays.Length > 1)
                {
                    // Find the one that's NOT on Game Over canvas
                    foreach (SongProgressDisplay display in allDisplays)
                    {
                        if (!display.gameObject.name.Contains("Game") || display.gameObject.name.Contains("LevelComplete"))
                        {
                            levelCompleteSongProgressDisplay = display;
                            Debug.Log($"[GameManager] Found level complete progress display (fallback): {levelCompleteSongProgressDisplay.gameObject.name}");
                            break;
                        }
                    }
                }
                else if (allDisplays.Length > 0)
                {
                    levelCompleteSongProgressDisplay = allDisplays[0];
                }
            }
        }

        if (levelCompleteSongProgressDisplay != null)
        {
            levelCompleteSongProgressDisplay.UpdateProgressDisplay();
            Debug.Log("[GameManager] Updated progress display on level complete.");
        }
        else
        {
            Debug.LogWarning("[GameManager] Level complete progress display not found in scene!");
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
    public void PauseGame()
    {
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        SetState(GameState.Playing);
    }
}
