using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace RhythmRush.UI
{
    /// <summary>
    /// Drives the neon Pause / Game Over / Level Clear overlays in a live gameplay
    /// scene. It mirrors <see cref="GameManager"/>'s state, hides the project's legacy
    /// cartoon canvases, and wires the buttons to the real game actions
    /// (resume / restart / next level / menu) — so the design-kit screens replace the
    /// old ones during actual play.
    ///
    /// Spawned automatically by <see cref="RRGameScreenBootstrap"/> in any scene that
    /// has a GameManager; you never have to add it by hand.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RRGameScreenBinder : MonoBehaviour
    {
        RectTransform _layer;
        GameObject _current;
        GameManager.GameState _shown;

        void Start()
        {
            DisableLegacyUI();
            BuildCanvas();
            _shown = GameManager.Instance != null ? GameManager.Instance.CurrentState : GameManager.GameState.Playing;
            Rebuild(_shown);
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            // Pause toggle (the legacy PauseManager is disabled, so we own Esc now).
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (gm.CurrentState == GameManager.GameState.Playing) gm.PauseGame();
                else if (gm.CurrentState == GameManager.GameState.Paused) gm.ResumeGame();
            }

            if (gm.CurrentState != _shown)
            {
                _shown = gm.CurrentState;
                Rebuild(_shown);
            }
        }

        // ---------------------------------------------------------------- overlays

        void Rebuild(GameManager.GameState state)
        {
            if (_current != null) { Destroy(_current); _current = null; }

            switch (state)
            {
                case GameManager.GameState.Paused:
                    RRScreens.PauseOverlay(NewRoot("Pause"), Resume, Retry, QuitToMenu);
                    PopModal();
                    break;

                case GameManager.GameState.GameOver:
                    RRScreens.GameOverOverlay(NewRoot("GameOver"), GameOverResults(), Retry, QuitToMenu);
                    PopModal();
                    break;

                case GameManager.GameState.LevelComplete:
                    UnlockNextLevel();
                    RRScreens.LevelClearOverlay(NewRoot("LevelClear"), ComputeStars(), ClearResults(), Continue, Retry, QuitToSelect);
                    PopModal();
                    break;
            }
        }

        RectTransform NewRoot(string name)
        {
            var root = RRUI.Node("RR:" + name, _layer); root.Stretch();
            _current = root.gameObject;
            return root;
        }

        void PopModal()
        {
            if (_current == null) return;
            var panel = _current.transform.Find("Panel") as RectTransform;
            if (panel != null) StartCoroutine(PopIn(panel));
        }

        static IEnumerator PopIn(RectTransform target)
        {
            const float dur = 0.26f, overshoot = 1.08f;
            float t = 0f;
            target.localScale = Vector3.zero;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                float p = Mathf.Clamp01(t / dur);
                target.localScale = p < 0.6f
                    ? Vector3.LerpUnclamped(Vector3.zero, Vector3.one * overshoot, p / 0.6f)
                    : Vector3.LerpUnclamped(Vector3.one * overshoot, Vector3.one, (p - 0.6f) / 0.4f);
                yield return null;
            }
            target.localScale = Vector3.one;
        }

        // ---------------------------------------------------------------- button actions

        void Resume()       { var gm = GameManager.Instance; if (gm != null) gm.ResumeGame(); }
        void Retry()        { var gm = GameManager.Instance; if (gm != null) gm.RestartLevel(); else SceneTransitionManager.ReloadCurrentLevel(); }
        void Continue()     { var gm = GameManager.Instance; if (gm != null) gm.LoadNextLevel(); }
        void QuitToMenu()   { SceneTransitionManager.GoToMainMenu(); }
        void QuitToSelect() { SceneTransitionManager.LoadLevel("GameSelection"); }

        // ---------------------------------------------------------------- values

        // Sample stand-ins for metrics the game doesn't track yet (combo / accuracy).
        const string SampleMaxCombo = "312";
        const string SampleAccuracy = "98%";

        static string RealScore()
        {
            // No point-score system yet → use orbs collected as the real score.
            return ScoreTracker.GetOrbsCollected().ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
        }

        static RRWidgets.Result[] GameOverResults()
        {
            ScoreTracker.UpdateSongProgress();
            int pct = Mathf.RoundToInt(ScoreTracker.GetSongProgressPercent());
            return new[]
            {
                new RRWidgets.Result("Score", RealScore(), RRTheme.Yellow),     // real
                new RRWidgets.Result("Max Combo", SampleMaxCombo, RRTheme.Pink), // sample (not tracked)
                new RRWidgets.Result("Cleared", pct + "%", RRTheme.Cyan),        // real
            };
        }

        static RRWidgets.Result[] ClearResults()
        {
            ScoreTracker.UpdateSongProgress();
            return new[]
            {
                new RRWidgets.Result("Score", RealScore(), RRTheme.Yellow),       // real
                new RRWidgets.Result("Max Combo", SampleMaxCombo, RRTheme.Pink),  // sample (not tracked)
                new RRWidgets.Result("Accuracy", SampleAccuracy, RRTheme.Cyan),   // sample (not tracked)
            };
        }

        static int ComputeStars()
        {
            int total = ScoreTracker.GetTotalOrbs();
            if (total <= 0) return 3;
            float ratio = (float)ScoreTracker.GetOrbsCollected() / total;
            return ratio >= 0.9f ? 3 : ratio >= 0.6f ? 2 : 1;
        }

        // Preserve the legacy LevelCompleteScreenController's unlock side-effect.
        static void UnlockNextLevel()
        {
            var m = Regex.Match(SceneManager.GetActiveScene().name, @"(\d+)$");
            if (m.Success && int.TryParse(m.Value, out int n) && n > 0)
                LevelProgress.UnlockUpTo(n + 1);
        }

        // ---------------------------------------------------------------- setup

        void BuildCanvas()
        {
            EnsureEventSystem();
            var go = new GameObject("RhythmRush Screen Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.transform.SetParent(transform, false);
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;                       // above the game HUD, below the fade canvas (32767)
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            scaler.referencePixelsPerUnit = 100f;
            _layer = (RectTransform)go.transform;
        }

        static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        /// <summary>Hide the project's cartoon Pause / Game Over / Level Complete canvases and stop their controllers.</summary>
        static void DisableLegacyUI()
        {
            foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                string n = c.gameObject.name.ToLowerInvariant();
                bool legacy = (n.Contains("pause") && n.Contains("menu"))
                           || (n.Contains("over") && n.Contains("menu"))
                           || n.Contains("complete");
                if (legacy) c.gameObject.SetActive(false);
            }
            DisableAll<PauseManager>();
            DisableAll<GameOverScreenController>();
            DisableAll<LevelCompleteScreenController>();
        }

        static void DisableAll<T>() where T : Behaviour
        {
            foreach (var b in Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                b.enabled = false;
        }
    }

    /// <summary>
    /// Auto-installs <see cref="RRGameScreenBinder"/> into every gameplay scene (any
    /// scene containing a GameManager) at runtime — no per-scene wiring. Set
    /// <see cref="Enabled"/> = false (or delete this file) to fall back to the project's
    /// original cartoon screens.
    /// </summary>
    public static class RRGameScreenBootstrap
    {
        public static bool Enabled = true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            SceneManager.sceneLoaded += (scene, mode) => TrySpawn();
            TrySpawn(); // the first scene is already loaded when this runs
        }

        static void TrySpawn()
        {
            if (!Enabled) return;
            if (Object.FindFirstObjectByType<GameManager>() == null) return;        // not a gameplay scene
            if (Object.FindFirstObjectByType<RRGameScreenBinder>() != null) return; // already installed
            new GameObject("RhythmRush Screens (auto)").AddComponent<RRGameScreenBinder>();
        }
    }
}
