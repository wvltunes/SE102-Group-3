using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Manages scene transitions with fade in/out effects using DOTween and CanvasGroup.
/// 
/// Fade Transition Flow:
/// 1. Fade-out (0.5s) - black screen appears
/// 2. Scene loads
/// 3. StartupComponent auto-creates fade canvas (if not present)
/// 4. Fade-in (0.5s) - scene becomes visible
///
/// The fade canvas persists across scene loads using DontDestroyOnLoad to ensure
/// smooth transitions. A single StartupComponent in each scene handles the fade-in.
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

    /// <summary>Duration of fade in/out animation in seconds.</summary>
    private const float FadeDuration = 0.5f;

    /// <summary>The fade canvas persists across scene loads.</summary>
    private static CanvasGroup _cachedFadeCanvasGroup;
    private static bool _fadingOut = false;
    private static bool _fadeInTriggered = false;

    /// <summary>
    /// Gets or creates the transition canvas with CanvasGroup for fade effects.
    /// The canvas persists across scene loads using DontDestroyOnLoad.
    /// </summary>
    private static CanvasGroup GetOrCreateFadeCanvasGroup()
    {
        if (_cachedFadeCanvasGroup != null && _cachedFadeCanvasGroup.gameObject != null)
        {
            // Ensure canvas is enabled
            if (!_cachedFadeCanvasGroup.gameObject.activeInHierarchy)
            {
                Debug.Log("[SceneTransitionManager] Re-enabling cached FadeCanvas.");
                _cachedFadeCanvasGroup.gameObject.SetActive(true);
            }
            return _cachedFadeCanvasGroup;
        }

        GameObject fadeCanvas = GameObject.Find("FadeCanvas");

        if (fadeCanvas == null)
        {
            Debug.Log("[SceneTransitionManager] Creating new FadeCanvas.");
            fadeCanvas = new GameObject("FadeCanvas");
            Canvas canvas = fadeCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767; // Max sorting order to ensure it's on top

            CanvasScaler scaler = fadeCanvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // DO NOT add GraphicRaycaster - it blocks all UI interactions even when invisible

            RectTransform rect = fadeCanvas.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Create a black image to cover the screen
            GameObject imageObj = new GameObject("FadeImage");
            imageObj.transform.SetParent(fadeCanvas.transform, false);
            Image image = imageObj.AddComponent<Image>();
            image.color = Color.black;
            image.raycastTarget = false; // Don't block raycasts

            RectTransform imageRect = imageObj.GetComponent<RectTransform>();
            imageRect.anchorMin = Vector2.zero;
            imageRect.anchorMax = Vector2.one;
            imageRect.offsetMin = Vector2.zero;
            imageRect.offsetMax = Vector2.zero;

            // Apply DontDestroyOnLoad BEFORE the scene change happens
            Object.DontDestroyOnLoad(fadeCanvas);

            Debug.Log("[SceneTransitionManager] FadeCanvas created with black Image and DontDestroyOnLoad applied.");
        }
        else
        {
            Debug.Log("[SceneTransitionManager] Found existing FadeCanvas.");
            // Make sure it's marked as DontDestroyOnLoad
            Object.DontDestroyOnLoad(fadeCanvas);
        }

        CanvasGroup canvasGroup = fadeCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = fadeCanvas.AddComponent<CanvasGroup>();
            Debug.Log("[SceneTransitionManager] Added CanvasGroup to FadeCanvas.");
        }

        _cachedFadeCanvasGroup = canvasGroup;

        return canvasGroup;
    }

    /// <summary>
    /// Performs a fade-out (alpha 0 to 1) animation on the transition canvas.
    /// Uses SetUpdate(true) for unscaled time so it works when timeScale = 0.
    /// </summary>
    private static void FadeOut(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) return;

        _fadingOut = true;
        _fadeInTriggered = false; // Reset fade-in flag for next scene
        canvasGroup.alpha = 0f;
        Debug.Log($"[SceneTransitionManager] Starting fade-out. Canvas alpha: {canvasGroup.alpha}");
        canvasGroup.DOFade(1f, FadeDuration).SetUpdate(true);
    }

    /// <summary>
    /// Performs a fade-in (alpha 1 to 0) animation on the transition canvas.
    /// Uses SetUpdate(true) for unscaled time so it works independent of gameplay state.
    /// </summary>
    private static void FadeIn(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) return;

        _fadingOut = false;
        canvasGroup.alpha = 1f;
        Debug.Log($"[SceneTransitionManager] Starting fade-in. Canvas alpha: {canvasGroup.alpha}");
        canvasGroup.DOFade(0f, FadeDuration).SetUpdate(true);
    }

    /// <summary>
    /// Public method for scenes to call fade-in manually.
    /// Called by SceneTransitionStartup when scene loads.
    /// </summary>
    public static void DoFadeIn()
    {
        if (_fadeInTriggered) return; // Already triggered, don't do it again

        _fadeInTriggered = true;

        // Important: Check if the cached canvas is still valid after scene load
        if (_cachedFadeCanvasGroup != null)
        {
            // Try to get the GameObject - if it's destroyed, the reference will be null
            GameObject fadeCanvasObj = _cachedFadeCanvasGroup.gameObject;
            if (fadeCanvasObj == null)
            {
                // Cached reference is stale, clear it
                _cachedFadeCanvasGroup = null;
                Debug.Log("[SceneTransitionManager] Cached fade canvas was destroyed, creating new one.");
            }
        }

        CanvasGroup canvas = GetOrCreateFadeCanvasGroup();
        if (canvas != null && canvas.gameObject != null)
        {
            Debug.Log("[SceneTransitionManager] Starting fade-in animation.");
            FadeIn(canvas);
        }
        else
        {
            Debug.LogError("[SceneTransitionManager] Failed to get fade canvas for fade-in!");
        }
    }

    /// <summary>
    /// Loads the scene at <paramref name="buildIndex"/> in Build Settings, with fade transition.
    /// Restores normal time before loading. Out-of-range indices are rejected with a warning.
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

        CanvasGroup fadeCanvas = GetOrCreateFadeCanvasGroup();
        FadeOut(fadeCanvas);

        // Schedule scene load after fade-out completes using unscaled time
        DOTween.To(
            () => 0f,
            x => { },
            1f,
            FadeDuration
        ).SetUpdate(true).OnComplete(() =>
        {
            SceneManager.LoadScene(buildIndex);
        });
    }

    /// <summary>
    /// Loads a scene by its name (must be in Build Settings), with fade transition.
    /// Restores normal time before loading. Prefer this for level select: a level's 
    /// Build index is not the same as its level number.
    /// </summary>
    public static void LoadLevel(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneTransitionManager] Empty scene name. Load ignored.");
            return;
        }

        Time.timeScale = 1f;

        CanvasGroup fadeCanvas = GetOrCreateFadeCanvasGroup();
        FadeOut(fadeCanvas);

        // Schedule scene load after fade-out completes using unscaled time
        DOTween.To(
            () => 0f,
            x => { },
            1f,
            FadeDuration
        ).SetUpdate(true).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    /// <summary>
    /// Returns to the main menu with fade transition, after restoring normal time.
    /// </summary>
    public static void GoToMainMenu()
    {
        LoadLevel(MainMenuScene);
    }

    /// <summary>
    /// Alias for GoToMainMenu() for backward compatibility.
    /// </summary>
    public static void LoadMainMenu()
    {
        GoToMainMenu();
    }

    /// <summary>
    /// Reloads the scene that is currently active with fade transition, after 
    /// restoring normal time. Used by the game-over "Retry" path.
    /// </summary>
    public static void ReloadCurrentLevel()
    {
        Time.timeScale = 1f;

        CanvasGroup fadeCanvas = GetOrCreateFadeCanvasGroup();
        FadeOut(fadeCanvas);

        // Schedule scene load after fade-out completes using unscaled time
        DOTween.To(
            () => 0f,
            x => { },
            1f,
            FadeDuration
        ).SetUpdate(true).OnComplete(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }
}
