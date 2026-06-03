using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Drives the "level cleared" UI for a gameplay scene.
///
/// Listens for <see cref="LevelCompleteZone.OnLevelComplete"/> (the same event the
/// <see cref="GameManager"/> uses to pause the game) and, when it fires, reveals the
/// level-complete canvas, records unlock progress, and plays a small pop animation.
/// Two public methods are meant to be wired to the on-screen buttons.
///
/// Place this on the root of the LevelCompleteScreen prefab (the Canvas) and point
/// <see cref="completeScreen"/> at the panel that should appear.
/// </summary>
public class LevelCompleteScreenController : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("The panel/root object to reveal when the level is completed. Leave it " +
             "inactive in the scene so it only shows on completion.")]
    [SerializeField] private GameObject completeScreen;

    [Tooltip("Optional: the object the pop animation scales. Defaults to " +
             "completeScreen if left empty (assign the inner panel to avoid scaling " +
             "any full-screen dim background).")]
    [SerializeField] private RectTransform popTarget;

    [Header("Navigation")]
    [Tooltip("Scene loaded by the Menu button. 'GameSelection' drops the player back " +
             "on the level-select screen (so the newly unlocked level is visible); " +
             "set it to 'MainMenu' to return all the way home instead.")]
    [SerializeField] private string menuSceneName = "GameSelection";

    [Header("Juice")]
    [Tooltip("Duration of the scale-in pop, in unscaled seconds (runs while paused). " +
             "Set to 0 to disable the animation.")]
    [SerializeField] private float popDuration = 0.35f;
    [SerializeField] private float popOvershoot = 1.1f;

    // Guards against the screen being shown twice (e.g. a double trigger).
    private bool shown = false;

    private void OnEnable()
    {
        LevelCompleteZone.OnLevelComplete += HandleLevelComplete;
    }

    private void OnDisable()
    {
        LevelCompleteZone.OnLevelComplete -= HandleLevelComplete;
    }

    private void Start()
    {
        // Make sure the screen starts hidden even if the prefab was saved visible.
        if (completeScreen != null)
        {
            completeScreen.SetActive(false);
        }
    }

    private void HandleLevelComplete()
    {
        if (shown) return;
        shown = true;

        // Persist progress: clearing Level N unlocks Level N+1 on the select screen.
        int currentLevel = ParseLevelNumber(SceneManager.GetActiveScene().name);
        if (currentLevel > 0)
        {
            LevelProgress.UnlockUpTo(currentLevel + 1);
        }

        if (completeScreen != null)
        {
            completeScreen.SetActive(true);
        }

        // GameManager has already set Time.timeScale = 0, so the pop runs on unscaled
        // time. Start from zero scale to avoid a one-frame flash at full size.
        RectTransform target = popTarget != null
            ? popTarget
            : (completeScreen != null ? completeScreen.transform as RectTransform : null);
        if (target != null && popDuration > 0f)
        {
            target.localScale = Vector3.zero;
            StartCoroutine(PopIn(target));
        }
    }

    // --- Button handlers (wire these to the UI buttons' OnClick) ---------------

    /// <summary>Test method to check if button is wired correctly.</summary>
    public void TestButtonClick()
    {
        Debug.Log("[LevelCompleteScreenController] TEST: Button was clicked!");
    }

    /// <summary>Menu button: leave to the main menu / level-select scene.</summary>
    public void OnMenuButton()
    {
        SceneTransitionManager.LoadLevel(menuSceneName);
    }

    /// <summary>
    /// Next-level button: load the next scene in Build Settings order. Levels are
    /// laid out sequentially, so the next scene is simply buildIndex + 1. If this was
    /// the last level, fall back to the menu instead of loading nothing.
    /// </summary>
    public void OnNextLevelButton()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        Debug.Log($"[LevelCompleteScreenController] Current scene index: {currentIndex}, Next index: {nextIndex}, Total scenes: {SceneManager.sceneCountInBuildSettings}");
        
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"[LevelCompleteScreenController] Loading next level at index {nextIndex}");
            SceneTransitionManager.LoadLevel(nextIndex);
        }
        else
        {
            Debug.Log("[LevelCompleteScreenController] No next level - returning to menu.");
            SceneTransitionManager.LoadLevel(menuSceneName);
        }
    }

    // --- Helpers ---------------------------------------------------------------

    private IEnumerator PopIn(RectTransform target)
    {
        float t = 0f;
        Vector3 peak = Vector3.one * popOvershoot;

        // Scale 0 -> overshoot (first 60%) -> 1 (last 40%) for a springy "pop".
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / popDuration);
            target.localScale = p < 0.6f
                ? Vector3.LerpUnclamped(Vector3.zero, peak, p / 0.6f)
                : Vector3.LerpUnclamped(peak, Vector3.one, (p - 0.6f) / 0.4f);
            yield return null;
        }
        target.localScale = Vector3.one;
    }

    /// <summary>
    /// Pulls the trailing number out of a scene name ("Level3" -> 3). Returns 0 if the
    /// name has no trailing digits (e.g. a test scene), which simply disables the
    /// unlock step rather than guessing.
    /// </summary>
    private static int ParseLevelNumber(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return 0;
        Match m = Regex.Match(sceneName, @"(\d+)$");
        return m.Success && int.TryParse(m.Value, out int n) ? n : 0;
    }
}
