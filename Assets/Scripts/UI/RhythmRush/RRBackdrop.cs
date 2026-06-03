using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RhythmRush.UI
{
    /// <summary>
    /// The in-game neon stage that sits behind the Pause / Game Over / Level Clear
    /// overlays: radial purple void, perspective grid floor, treeline silhouette,
    /// floating orbs, the running character, and the live HUD (score chip, segmented
    /// energy bar, centered song-progress bar, combo call-out).
    /// </summary>
    public static class RRBackdrop
    {
        public struct Hud
        {
            public string score, song;
            public float energy, pct;
            public int combo;
            public string judge;
        }

        public static void Build(RectTransform stage, Hud hud)
        {
            // ---- void gradient ----
            RRUI.Img("bg", stage, RRSprites.GameplayBg(), Color.white).rectTransform.Stretch();

            // ---- perspective grid floor (bottom 46%) ----
            var grid = RRUI.Img("grid", stage, RRSprites.PerspectiveGrid(), RRTheme.A(Color.white, 0.7f));
            grid.rectTransform.BottomStretch(720f * 0.46f);

            // ---- treeline silhouette (bottom 60%) ----
            var tree = RRSprites.Art("treeline.png");
            if (tree != null)
            {
                var t = RRUI.Img("treeline", stage, tree, RRTheme.A(Color.white, 0.85f));
                t.rectTransform.BottomStretch(720f * 0.60f);
            }

            // ---- floating orbs ----
            string[] orbs = { "orb-pink.png", "orb-blue.png", "orb-yellow.png", "orb-orange.png" };
            float[,] pos = { { 0.46f, 0.38f }, { 0.60f, 0.55f }, { 0.72f, 0.30f }, { 0.84f, 0.62f } }; // left%, top%
            for (int i = 0; i < orbs.Length; i++)
            {
                var sp = RRSprites.Art(orbs[i]); if (sp == null) continue;
                Vector2 sz = RRSprites.ArtSize(orbs[i]); float h = 54f, w = sz.x / Mathf.Max(1f, sz.y) * h;
                var o = RRUI.Img("orb" + i, stage, sp, Color.white);
                o.preserveAspect = true;
                o.rectTransform.TopLeft(pos[i, 0] * 1280f, pos[i, 1] * 720f, w, h);
                o.gameObject.AddComponent<RRBob>().Configure(8f, 1.6f, i * 1.3f);
            }

            // ---- running character (Aria) ----
            var runnerSp = RRSprites.Art("aria.png");
            if (runnerSp != null)
            {
                Vector2 sz = RRSprites.ArtSize("aria.png"); float h = 180f, w = sz.x / Mathf.Max(1f, sz.y) * h;
                var r = RRUI.Img("runner", stage, runnerSp, Color.white);
                r.preserveAspect = true;
                r.rectTransform.BottomLeft(200f, 130f, w, h);
                r.gameObject.AddComponent<RRBob>().Configure(10f, 4f, 0f);
            }

            // ---- HUD: score chip + energy bar (top-left) ----
            var hudLeft = RRUI.Node("hud-left", stage); hudLeft.TopLeft(32, 24, 360, 110);
            var chip = RRWidgets.ScoreChip(hudLeft, "Score", hud.score, RRTheme.Yellow);
            chip.TopLeft(0, 0, chip.sizeDelta.x, chip.sizeDelta.y);
            var energy = RRWidgets.EnergyBar(hudLeft, hud.energy);
            energy.TopLeft(0, 62, energy.sizeDelta.x, energy.sizeDelta.y);

            // ---- HUD: pause button + key hint (top-right) ----
            var pauseBtn = RRUI.IconButton(stage, null, 52f);
            ((RectTransform)pauseBtn.transform).TopRight(32, 24, 52, 52);
            var ii = RRUI.Text("II", (RectTransform)pauseBtn.transform.Find("body"), "II", RRFonts.HudX, 18, RRTheme.Cyan);
            ii.rectTransform.Stretch();
            RRWidgets.KeyHint(stage).TopRight(32, 86, 200, 16);

            // ---- HUD: centered song-progress ----
            var pw = RRUI.Node("progress-wrap", stage); pw.TopCenter(26, 420, 50);
            RRWidgets.ProgressBar(pw, hud.pct, 420f).TopCenter(0, 420, 18);
            var sn = RRUI.Text("song", pw, hud.song, RRFonts.Hud, 13, RRTheme.Cyan, TextAlignmentOptions.Left);
            sn.rectTransform.TopLeft(0, 24, 300, 16);
            var pc = RRUI.Text("pct", pw, Mathf.RoundToInt(hud.pct) + "%", RRFonts.Hud, 13, RRTheme.Pink, TextAlignmentOptions.Right);
            pc.rectTransform.TopRight(0, 24, 120, 16);

            // ---- combo call-out ----
            if (hud.combo > 0)
            {
                var combo = RRUI.Node("combo", stage); combo.Mid(440, 200, 0, 360f - 0.42f * 720f);
                var judge = RRUI.Text("judge", combo, hud.judge, RRFonts.Display, 46, RRTheme.Cyan);
                judge.rectTransform.Mid(440, 56, 0, 64);
                RRUI.Neon(judge, RRTheme.Ink, 0.15f, RRTheme.Cyan, 0.4f);
                var cval = RRUI.Text("cval", combo, hud.combo.ToString(), RRFonts.Display, 88, RRTheme.Pink);
                cval.rectTransform.Mid(440, 100, 0, -8);
                RRUI.Neon(cval, RRTheme.Ink, 0.16f, RRTheme.Pink, 0.4f);
                var clbl = RRUI.Text("clbl", combo, "COMBO", RRFonts.UiBold, 14, RRTheme.Fg1);
                clbl.characterSpacing = 12; clbl.rectTransform.Mid(440, 20, 0, -70);
            }
        }
    }
}
