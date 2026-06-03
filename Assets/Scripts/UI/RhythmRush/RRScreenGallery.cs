using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace RhythmRush.UI
{
    /// <summary>
    /// Drop this on an empty GameObject in any scene and press Play to view the four
    /// recreated screens from ui_kits/game/Screens.html — Pause Menu, Game Over,
    /// Level Clear, Settings — on a fixed 1280×720 neon stage.
    ///
    /// Navigate with ← / → (or number keys 1–4). Character Select / the gameplay HUD
    /// were trimmed from the source deck by the designer, so they are intentionally absent.
    /// </summary>
    [AddComponentMenu("RhythmRush/Screen Gallery")]
    public sealed class RRScreenGallery : MonoBehaviour
    {
        public enum Screen { Pause, GameOver, LevelClear, Settings }

        [Tooltip("Which screen to show first.")]
        public Screen startScreen = Screen.Pause;

        readonly List<RectTransform> _screens = new List<RectTransform>();
        readonly string[] _names = { "PAUSE MENU", "GAME OVER", "LEVEL CLEAR", "SETTINGS" };
        int _index;
        TextMeshProUGUI _nav;

        void Start()
        {
            EnsureEventSystem();

            // ---- canvas (scales the 1280×720 design space to the window) ----
            var canvasGo = new GameObject("RhythmRush Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            scaler.referencePixelsPerUnit = 100f;
            var canvasRt = (RectTransform)canvasGo.transform;

            // ---- letterbox backdrop ----
            RRUI.Img("letterbox", canvasRt, null, RRTheme.Hex("#05060C")).rectTransform.Stretch();

            // ---- fixed 16:9 stage, clipped ----
            var stage = RRUI.Node("Stage", canvasRt); stage.Mid(1280, 720);
            RRUI.Img("stage-bg", stage, null, RRTheme.Void).rectTransform.Stretch();
            stage.gameObject.AddComponent<RectMask2D>();

            // ---- screens ----
            _screens.Add(RRScreens.Pause(stage));
            _screens.Add(RRScreens.GameOver(stage));
            _screens.Add(RRScreens.LevelClear(stage));
            _screens.Add(RRScreens.Settings(stage));

            // ---- always-on CRT treatment over the whole stage ----
            BuildVignette(stage);
            BuildScanlines(stage);

            // ---- nav hint (in the letterbox, outside the clipped stage) ----
            _nav = RRUI.Text("nav", canvasRt, "", RRFonts.Hud, 13, RRTheme.Fg2);
            _nav.rectTransform.anchorMin = _nav.rectTransform.anchorMax = new Vector2(0.5f, 0);
            _nav.rectTransform.pivot = new Vector2(0.5f, 0);
            _nav.rectTransform.sizeDelta = new Vector2(700, 24);
            _nav.rectTransform.anchoredPosition = new Vector2(0, 10);

            Show((int)startScreen);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) Show(_index + 1);
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) Show(_index - 1);
            else if (Input.GetKeyDown(KeyCode.Alpha1)) Show(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) Show(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) Show(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) Show(3);
        }

        void Show(int index)
        {
            _index = (index % _screens.Count + _screens.Count) % _screens.Count;
            for (int i = 0; i < _screens.Count; i++) _screens[i].gameObject.SetActive(i == _index);
            if (_nav != null)
                _nav.text = $"<color=#7B5CFF>◀</color>  {_index + 1}/{_screens.Count}  <b>{_names[_index]}</b>  <color=#7B5CFF>▶</color>   ·   ← / →  or  1–4";
        }

        void BuildVignette(RectTransform stage)
        {
            var sprite = RRSprites.RadialBg(256, 144, 0.5f, 0.5f, 0.78f, 0.78f, new[]
            {
                new RRSprites.Stop(0.00f, new Color(0, 0, 0, 0)),
                new RRSprites.Stop(0.55f, new Color(0, 0, 0, 0)),
                new RRSprites.Stop(1.00f, new Color(0, 0, 0, 0.7f)),
            });
            RRUI.Img("vignette", stage, sprite, Color.white).rectTransform.Stretch();
        }

        void BuildScanlines(RectTransform stage)
        {
            var img = RRUI.Img("scanlines", stage, RRSprites.Scanlines(), new Color(1, 1, 1, 1));
            img.type = Image.Type.Tiled;
            img.rectTransform.Stretch();
        }

        static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            go.transform.SetParent(null);
        }
    }
}
