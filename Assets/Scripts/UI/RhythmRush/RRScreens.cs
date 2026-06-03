using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RhythmRush.UI
{
    /// <summary>
    /// The four screens from ui_kits/game/Screens.html: Pause Menu · Game Over ·
    /// Level Clear · Settings.
    ///
    /// Two flavours of each result screen:
    ///   • <c>Pause/GameOver/LevelClear/Settings</c> — full screens over the shared neon
    ///     gameplay backdrop (used by the standalone <see cref="RRScreenGallery"/>).
    ///   • <c>*Overlay</c> — just the dim + modal, for layering over the *live* game
    ///     (used by <see cref="RRGameScreenBinder"/>).
    /// </summary>
    public static class RRScreens
    {
        // sample data (from screens-deck.jsx) for the standalone gallery
        const string Score = "1,482,600";
        const string Song  = "CIRCUIT BREAK";
        const int MaxCombo = 312;

        static Action Log(string what) => () => Debug.Log("[RhythmRush] " + what);

        static void Wire(Button b, Action action, string fallbackLabel)
        {
            Action a = action ?? Log(fallbackLabel);
            b.onClick.AddListener(() => a());
        }

        // ---------- shared scaffolding ----------
        static RectTransform ScreenRoot(RectTransform parent, string name)
        {
            var root = RRUI.Node(name, parent); root.Stretch();
            return root;
        }

        static void Dim(RectTransform parent)
        {
            var dim = RRUI.Img("overlay-dim", parent, null, new Color(6 / 255f, 7 / 255f, 14 / 255f, 0.78f), raycast: true);
            dim.rectTransform.Stretch();
        }

        static void Title(RectTransform panel, string text, Color color, float yFromTop, bool glow)
        {
            var t = RRUI.Text("title", panel, text, RRFonts.Display, 64, color);
            t.rectTransform.Size(520, 78).AtTopCenter(yFromTop);
            RRUI.Neon(t, RRTheme.Ink, 0.17f, glow ? color : (Color?)null, 0.5f);
        }

        /// <summary>Panel width needed to hold a centered button row at full size (≥ <paramref name="min"/>).</summary>
        static float PanelWidthFor(float min, params RRUI.Cap[] caps)
        {
            const float gap = 14f, sidePad = 44f;
            float total = 0; foreach (var c in caps) total += c.root.sizeDelta.x; total += gap * (caps.Length - 1);
            return Mathf.Max(min, total + sidePad * 2f);
        }

        /// <summary>Lay a row of capsule buttons out horizontally, centered, gap 14, at a Y from the panel top.</summary>
        static void ButtonRow(RectTransform panel, float yFromTop, params RRUI.Cap[] caps)
        {
            const float gap = 14f;
            float total = 0; foreach (var c in caps) total += c.root.sizeDelta.x; total += gap * (caps.Length - 1);
            var row = RRUI.Node("row", panel); row.Size(total, 56).AtTopCenter(yFromTop);
            float x = -total / 2f;
            foreach (var c in caps)
            {
                float w = c.root.sizeDelta.x;
                c.root.SetParent(row, false);
                c.root.AtMid(x + w / 2f, 0);
                x += w + gap;
            }
        }

        // ============================================================ PAUSE

        public static RectTransform Pause(RectTransform parent, Action onResume = null, Action onRetry = null, Action onQuit = null)
        {
            var root = ScreenRoot(parent, "Screen:Pause");
            RRBackdrop.Build(root, new RRBackdrop.Hud { score = Score, song = Song, energy = 0.72f, pct = 62, combo = 248, judge = "PERFECT!" });
            PauseOverlay(root, onResume, onRetry, onQuit);
            return root;
        }

        /// <summary>Dim + PAUSED modal (with a TUTORIAL sub-panel) — for layering over the live game.</summary>
        public static void PauseOverlay(RectTransform parent, Action onResume, Action onRetry, Action onQuit)
        {
            Dim(parent);

            // --- PAUSED modal ---
            var pause = RRUI.Panel(parent, 560, 400);
            Title(pause, "PAUSED", RRTheme.Cyan, 30, glow: true);

            var resume = RRUI.Capsule(pause, "RESUME", RRBtn.Cyan);
            var retry  = RRUI.Capsule(pause, "RETRY", RRBtn.Pink);
            var tut    = RRUI.Capsule(pause, "TUTORIAL", RRBtn.Purple);
            var quit   = RRUI.Capsule(pause, "QUIT TO MENU", RRBtn.Ghost);
            RRUI.Equalize(resume, retry, tut, quit);   // all four the same size
            resume.root.AtTopCenter(110);
            retry.root.AtTopCenter(178);
            tut.root.AtTopCenter(246);
            quit.root.AtTopCenter(314);

            // --- HOW TO PLAY modal (hidden until TUTORIAL is pressed) ---
            var (tutContent, back) = BuildTutorial(parent);
            GameObject pausePanel = pause.parent.gameObject;
            GameObject tutPanel = tutContent.parent.gameObject;
            tutPanel.SetActive(false);

            Wire(resume.button, onResume, "Resume");
            Wire(retry.button, onRetry, "Retry");
            Wire(quit.button, onQuit, "Quit to menu");
            tut.button.onClick.AddListener(() => { pausePanel.SetActive(false); tutPanel.SetActive(true); });
            back.onClick.AddListener(() => { tutPanel.SetActive(false); pausePanel.SetActive(true); });
        }

        /// <summary>The "HOW TO PLAY" panel reached from the pause menu. Returns its content node + BACK button.</summary>
        static (RectTransform content, UnityEngine.UI.Button back) BuildTutorial(RectTransform parent)
        {
            var content = RRUI.Panel(parent, 620, 444);

            var t = RRUI.Text("title", content, "HOW TO PLAY", RRFonts.Display, 52, RRTheme.Cyan);
            t.rectTransform.Size(540, 64).AtTopCenter(26);
            RRUI.Neon(t, RRTheme.Ink, 0.16f, RRTheme.Cyan, 0.45f);

            var tag = RRUI.Text("tag", content, "Catch the flow, ride the beat.", RRFonts.Ui, 15, RRTheme.Fg2);
            tag.rectTransform.Size(540, 22).AtTopCenter(96);

            KbdRow(content, "SPACE", "Jump up a lane", 140);
            KbdRow(content, "S", "Drop down a lane", 188);
            KbdRow(content, "ESC", "Pause / resume", 236);

            var tip = RRUI.Text("tip", content,
                "Hit orbs & pads on the beat. Miss and your ENERGY drains — stay grounded to recover. Clear the track to earn your STAR RANK.",
                RRFonts.Ui, 13, RRTheme.Fg3);
            tip.rectTransform.Size(520, 52).AtTopCenter(296);

            var back = RRUI.Capsule(content, "BACK", RRBtn.Cyan, small: true);
            back.root.AtTopCenter(372);
            return (content, back.button);
        }

        /// <summary>A keyboard-key chip + description row for the tutorial panel.</summary>
        static void KbdRow(RectTransform parent, string key, string desc, float yFromTop)
        {
            const float rowW = 460f, chipW = 96f, chipH = 34f;
            var row = RRUI.Node("trow:" + key, parent); row.Size(rowW, 40).AtTopCenter(yFromTop);

            var chip = RRUI.Node("kbd", row); chip.Size(chipW, chipH).AtLeftCenter(0);
            RRUI.Img("b", chip, RRSprites.Rounded(8), RRTheme.Ink, sliced: true).rectTransform.Stretch();
            RRUI.Img("f", chip, RRSprites.Rounded(6), RRTheme.A(Color.white, 0.08f), sliced: true).rectTransform.Stretch(2, 2, 2, 2);
            RRUI.Text("k", chip, key, RRFonts.Hud, 14, RRTheme.Cyan).rectTransform.Stretch();

            var d = RRUI.Text("d", row, desc.ToUpperInvariant(), RRFonts.UiBold, 14, RRTheme.Fg1, TextAlignmentOptions.Left);
            d.characterSpacing = 1; d.rectTransform.Size(rowW - chipW - 18, 40).AtLeftCenter(chipW + 18);
        }

        // ============================================================ GAME OVER

        public static RectTransform GameOver(RectTransform parent, Action onRetry = null, Action onQuit = null)
        {
            var root = ScreenRoot(parent, "Screen:GameOver");
            RRBackdrop.Build(root, new RRBackdrop.Hud { score = Score, song = Song, energy = 0f, pct = 47, combo = 248, judge = "PERFECT!" });
            GameOverOverlay(root, new[]
            {
                new RRWidgets.Result("Score", Score, RRTheme.Yellow),
                new RRWidgets.Result("Max Combo", MaxCombo.ToString(), RRTheme.Pink),
                new RRWidgets.Result("Cleared", "47%", RRTheme.Cyan),
            }, onRetry, onQuit);
            return root;
        }

        public static void GameOverOverlay(RectTransform parent, RRWidgets.Result[] results, Action onRetry, Action onQuit)
        {
            Dim(parent);

            // Build the buttons first (same size), then size the panel to fit them.
            var retry = RRUI.Capsule(parent, "RETRY", RRBtn.Pink);
            var quit  = RRUI.Capsule(parent, "QUIT", RRBtn.Ghost);
            RRUI.Equalize(retry, quit);

            var panel = RRUI.Panel(parent, PanelWidthFor(560, retry, quit), 362);
            Title(panel, "GAME OVER", RRTheme.Pink, 30, glow: true);

            var body = RRUI.Text("body", panel, "The beat got away from you.", RRFonts.Ui, 16, RRTheme.Fg2);
            body.rectTransform.Size(480, 22).AtTopCenter(112);

            RRWidgets.ResultGrid(panel, results).AtTopCenter(154);
            ButtonRow(panel, 252, retry, quit);

            Wire(retry.button, onRetry, "Retry");
            Wire(quit.button, onQuit, "Quit");
        }

        // ============================================================ LEVEL CLEAR

        public static RectTransform LevelClear(RectTransform parent, Action onNext = null, Action onRetry = null, Action onQuit = null)
        {
            var root = ScreenRoot(parent, "Screen:LevelClear");
            RRBackdrop.Build(root, new RRBackdrop.Hud { score = Score, song = Song, energy = 0.72f, pct = 100, combo = 248, judge = "PERFECT!" });
            LevelClearOverlay(root, 3, new[]
            {
                new RRWidgets.Result("Score", Score, RRTheme.Yellow),
                new RRWidgets.Result("Max Combo", MaxCombo.ToString(), RRTheme.Pink),
                new RRWidgets.Result("Accuracy", "98%", RRTheme.Cyan),
            }, onNext, onRetry, onQuit);
            return root;
        }

        public static void LevelClearOverlay(RectTransform parent, int stars, RRWidgets.Result[] results, Action onNext, Action onRetry, Action onMenu)
        {
            Dim(parent);

            // Build the buttons first (same size), then size the panel to fit them.
            var cont  = RRUI.Capsule(parent, "CONTINUE", RRBtn.Green);
            var retry = RRUI.Capsule(parent, "RETRY", RRBtn.Pink);
            var menu  = RRUI.Capsule(parent, "MENU", RRBtn.Ghost);
            RRUI.Equalize(cont, retry, menu);

            var panel = RRUI.Panel(parent, PanelWidthFor(560, cont, retry, menu), 400);
            Title(panel, "LEVEL CLEAR!", RRTheme.Yellow, 28, glow: true);

            RRWidgets.Stars(panel, stars, 3).AtTopCenter(110);
            RRWidgets.ResultGrid(panel, results).AtTopCenter(196);
            ButtonRow(panel, 300, cont, retry, menu);

            Wire(cont.button, onNext, "Continue");
            Wire(retry.button, onRetry, "Retry");
            Wire(menu.button, onMenu, "Menu");
        }

        // ============================================================ SETTINGS

        public static RectTransform Settings(RectTransform parent, Action onSave = null)
        {
            var root = ScreenRoot(parent, "Screen:Settings");
            RRUI.Img("bg", root, RRSprites.MenuBg(), Color.white).rectTransform.Stretch();

            var back = RRUI.IconButton(root, RRSprites.Art("back-arrow.png"), 52f);
            ((RectTransform)back.transform).TopLeft(36, 26, 52, 52);
            var optTitle = RRUI.Text("OPTIONS", root, "OPTIONS", RRFonts.Display, 38, RRTheme.Fg1, TextAlignmentOptions.Left);
            optTitle.rectTransform.TopLeft(106, 30, 400, 48);
            RRUI.Neon(optTitle, RRTheme.Ink, 0.13f, RRTheme.Purple, 0.4f);

            var panel = RRUI.Panel(root, 540, 380);
            var list = RRUI.Node("set-list", panel); list.Size(420, 260).AtMid(0, 14);

            float y = 0; const float rowH = 36f, step = 56f;
            RowSlider(list, "Music", 80, ref y, rowH, step);
            RowSlider(list, "SFX", 70, ref y, rowH, step);
            RowSegmented(list, "Note Speed", new[] { "SLOW", "MID", "FAST" }, 1, ref y, rowH, step);
            RowToggle(list, "Fullscreen", true, ref y, rowH, step);
            RowToggle(list, "Hit Flash", true, ref y, rowH, step);

            var save = RRUI.Capsule(panel, "SAVE & CLOSE", RRBtn.Cyan, small: true);
            save.root.AtMid(0, -150);
            Wire(save.button, onSave, "Save & close");
            return root;
        }

        static RectTransform Row(RectTransform list, string label, ref float y, float rowH, float step)
        {
            var row = RRUI.Node("row:" + label, list); row.Size(420, rowH).AtTopCenter(y);
            var lab = RRUI.Text("label", row, label.ToUpperInvariant(), RRFonts.UiBold, 15, RRTheme.Fg1, TextAlignmentOptions.Left);
            lab.characterSpacing = 4; lab.rectTransform.Size(220, rowH).AtLeftCenter(0);
            y += step;
            return row;
        }

        static void RowSlider(RectTransform list, string label, float value, ref float y, float rowH, float step)
        {
            var row = Row(list, label, ref y, rowH, step);
            RRWidgets.Slider(row, value).GetComponent<RectTransform>().AtRightCenter(0);
        }

        static void RowSegmented(RectTransform list, string label, string[] opts, int sel, ref float y, float rowH, float step)
        {
            var row = Row(list, label, ref y, rowH, step);
            RRWidgets.Segmented(row, opts, sel).GetComponent<RectTransform>().AtRightCenter(0);
        }

        static void RowToggle(RectTransform list, string label, bool on, ref float y, float rowH, float step)
        {
            var row = Row(list, label, ref y, rowH, step);
            RRWidgets.Toggle(row, on).GetComponent<RectTransform>().AtRightCenter(0);
        }
    }
}
