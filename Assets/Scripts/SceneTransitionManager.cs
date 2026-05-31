using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// One place that owns "leave this scene and load another one".
///
/// Every entry point first restores <see cref="Time.timeScale"/> to 1. Gameplay
/// scenes pause by setting timeScale to 0 (see <see cref="GameManager"/> on
/// game-over and level-complete), and timeScale persists across scene loads - so
/// without this reset the next scene would load already frozen. Centralising the
/// reset here means no caller has to remember it.
///
/// Implemented as a static helper: it holds no state and needs no GameObject, so
/// it can be called from anywhere (button handlers, controllers) without a scene
/// reference. A UI Button cannot target a static method directly in the Inspector,
/// so wire buttons to a small MonoBehaviour (e.g. <see cref="LevelCompleteScreenController"/>,
/// <see cref="HomeButtonBehaviour"/>) that forwards to these methods.
/// </summary>
public static class SceneTransitionManager
{
    /// <summary>Build-Settings name of the main menu scene.</summary>
    public const string MainMenuScene = "MainMenu";

    /// <summary>
    /// Loads the scene at <paramref name="buildIndex"/> in Build Settings, after
    /// restoring normal time. Out-of-range indices are rejected with a warning so a
    /// bad "next level" calculation can't hard-crash the navigation flow.
    /// </summary>
    public static void LoadLevel(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning(
                $"[SceneTransitionManager] Build index {buildIndex} is out of range " +
                $"(0..{SceneManager.sceneCountInBuildSettings - 1}). Load ignored.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndex);
    }

    /// <summary>
    /// Loads a scene by its name (must be in Build Settings), after restoring normal
    /// time. Prefer this for level select: a level's Build index is not the same as
    /// its level number (e.g. "Level1" is build index 5 in this project), so loading
    /// by name avoids that off-by-N trap entirely.
    /// </summary>
    public static void LoadLevel(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneTransitionManager] Empty scene name. Load ignored.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>Returns to the main menu, after restoring normal time.</summary>
    public static void LoadMainMenu()
    {
        LoadLevel(MainMenuScene);
    }

    /// <summary>
    /// Reloads the scene that is currently active, after restoring normal time. Used
    /// by the game-over "Retry" path as a fallback when no GameManager is present.
    /// </summary>
    public static void ReloadCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
