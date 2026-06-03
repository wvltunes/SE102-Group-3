using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RhythmRush.UI
{
    /// <summary>
    /// HUD + results widgets: segmented energy bar, song-progress bar, score chip,
    /// star rank, results grid, and the Settings controls (slider / toggle / segmented).
    /// </summary>
    public static class RRWidgets
    {
        // ---------- Segmented energy bar (skewed parallelograms) ----------
        public static RectTransform EnergyBar(RectTransform parent, float value, int segments = 14)
        {
            var seg = RRSprites.Parallelogram(18, 26, 14f);
            float segW = seg.rect.width, segH = seg.rect.height;
            const float pad = 6f, gap = 3f;
            float innerW = segments * segW + (segments - 1) * gap;
            float barW = innerW + pad * 2f, barH = segH + pad * 2f;

            int on = Mathf.RoundToInt(value * segments);
            bool low = on <= segments * 0.28f && on > 0;

            var root = RRUI.Node("EnergyBar", parent); root.Size(barW, barH);
            RRUI.Img("border", root, RRSprites.Rounded(RRTheme.RMd), RRTheme.Ink, sliced: true).rectTransform.Stretch();
            RRUI.Img("fill", root, RRSprites.Rounded(RRTheme.RMd - 2), RRTheme.A(RRTheme.Void, 0.7f), sliced: true).rectTransform.Stretch(3, 3, 3, 3);

            for (int i = 0; i < segments; i++)
            {
                Color c = i >= on ? new Color(1, 1, 1, 0.08f) : (low ? RRTheme.Pink : RRTheme.Cyan);
                var s = RRUI.Img("seg" + i, root, seg, c);
                s.rectTransform.anchorMin = s.rectTransform.anchorMax = new Vector2(0, 0.5f);
                s.rectTransform.pivot = new Vector2(0, 0.5f);
                s.rectTransform.sizeDelta = new Vector2(segW, segH);
                s.rectTransform.anchoredPosition = new Vector2(pad + i * (segW + gap), 0);
            }
            return root;
        }

        // ---------- Song progress bar (track + gradient fill + head) ----------
        public static RectTransform ProgressBar(RectTransform parent, float pct, float width)
        {
            float h = 18f; int r = 9;
            var root = RRUI.Node("Progress", parent); root.Size(width, h);
            RRUI.Img("border", root, RRSprites.Rounded(r), RRTheme.Ink, sliced: true).rectTransform.Stretch();
            RRUI.Img("track", root, RRSprites.Rounded(6), RRTheme.A(RRTheme.Void, 0.85f), sliced: true).rectTransform.Stretch(3, 3, 3, 3);

            float p = Mathf.Clamp01(pct / 100f);
            var fill = RRUI.Img("fill", root, RRSprites.RoundedFillH(420, 14, 7, RRTheme.Purple, RRTheme.Pink), Color.white);
            var frt = fill.rectTransform;
            frt.anchorMin = new Vector2(0, 0); frt.anchorMax = new Vector2(p, 1); frt.pivot = new Vector2(0, 0.5f);
            frt.offsetMin = new Vector2(2, 2); frt.offsetMax = new Vector2(-2, -2);

            var head = RRUI.Img("head", root, RRSprites.Circle(14), Color.white);
            head.rectTransform.anchorMin = head.rectTransform.anchorMax = new Vector2(p, 0.5f);
            head.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            head.rectTransform.sizeDelta = new Vector2(14, 14);
            head.rectTransform.anchoredPosition = Vector2.zero;
            return root;
        }

        // ---------- Score chip ----------
        public static RectTransform ScoreChip(RectTransform parent, string label, string value, Color valueColor)
        {
            var root = RRUI.Node("Chip", parent);
            RRUI.Img("border", root, RRSprites.Rounded(RRTheme.RMd), RRTheme.Ink, sliced: true).rectTransform.Stretch();
            RRUI.Img("fill", root, RRSprites.Rounded(RRTheme.RMd - 2), RRTheme.A(RRTheme.Deep, 0.78f), sliced: true).rectTransform.Stretch(2.5f, 2.5f, 2.5f, 2.5f);

            var k = RRUI.Text("k", root, label.ToUpperInvariant(), RRFonts.UiBold, 10, RRTheme.Fg3);
            k.characterSpacing = 8; k.rectTransform.TopCenter(7, 200, 12);
            var v = RRUI.Text("v", root, value, RRFonts.HudX, 28, valueColor);
            v.rectTransform.Mid(260, 32, 0, -6);

            float w = Mathf.Max(96f, v.GetPreferredValues(value).x + 36f);
            root.Size(w, 52);
            return root;
        }

        // ---------- Star rank ----------
        public static RectTransform Stars(RectTransform parent, int count, int total = 3)
        {
            var star = RRSprites.Art("star.png");
            Vector2 sz = RRSprites.ArtSize("star.png");
            float h = 64f, w = sz.x / Mathf.Max(1f, sz.y) * h, gap = 10f;
            float totalW = total * w + (total - 1) * gap;

            var root = RRUI.Node("Stars", parent); root.Size(totalW, h);
            for (int i = 0; i < total; i++)
            {
                bool filled = i < count;
                var img = RRUI.Img("star" + i, root, star, filled ? Color.white : new Color(0.28f, 0.28f, 0.32f, 1f));
                img.preserveAspect = true;
                img.rectTransform.anchorMin = img.rectTransform.anchorMax = new Vector2(0, 0.5f);
                img.rectTransform.pivot = new Vector2(0, 0.5f);
                img.rectTransform.sizeDelta = new Vector2(w, h);
                img.rectTransform.anchoredPosition = new Vector2(i * (w + gap), 0);
            }
            return root;
        }

        // ---------- Results grid (label over value, columns spaced) ----------
        public struct Result { public string label; public string value; public Color color; public Result(string l, string v, Color c) { label = l; value = v; color = c; } }

        public static RectTransform ResultGrid(RectTransform parent, params Result[] items)
        {
            const float gap = 30f, labelSize = 11f, valueSize = 30f;
            var root = RRUI.Node("ResultGrid", parent);
            if (items == null || items.Length == 0) { root.Size(0, 56); return root; }

            float[] colW = new float[items.Length];
            float total = 0f;
            var probe = RRUI.Text("probe", root, "", RRFonts.HudX, valueSize, Color.white);
            for (int i = 0; i < items.Length; i++)
            {
                float vW = probe.GetPreferredValues(items[i].value).x;
                probe.font = RRFonts.UiBold; probe.fontSize = labelSize;
                float lW = probe.GetPreferredValues(items[i].label.ToUpperInvariant()).x + 4f;
                probe.font = RRFonts.HudX; probe.fontSize = valueSize;
                colW[i] = Mathf.Max(vW, lW);
                total += colW[i];
            }
            Object.Destroy(probe.gameObject);
            total += gap * (items.Length - 1);
            root.Size(total, 56);

            float x = -total / 2f;
            for (int i = 0; i < items.Length; i++)
            {
                float cx = x + colW[i] / 2f;
                var lab = RRUI.Text("k" + i, root, items[i].label.ToUpperInvariant(), RRFonts.UiBold, labelSize, RRTheme.Fg3);
                lab.characterSpacing = 6; lab.rectTransform.Mid(colW[i] + 8, 14, cx, 16);
                var val = RRUI.Text("v" + i, root, items[i].value, RRFonts.HudX, valueSize, items[i].color);
                val.rectTransform.Mid(colW[i] + 8, 32, cx, -8);
                x += colW[i] + gap;
            }
            return root;
        }

        // ---------- Settings slider ----------
        public static Slider Slider(RectTransform parent, float value)
        {
            const float w = 230f, h = 12f; const int handle = 24;
            var root = RRUI.Node("Slider", parent); root.Size(w, h);
            var bg = RRUI.Img("bg", root, RRSprites.Rounded(6), RRTheme.A(Color.white, 0.10f), sliced: true, raycast: true);
            bg.rectTransform.Stretch();
            RRUI.Img("border", root, RRSprites.RoundedRing(Mathf.RoundToInt(w), Mathf.RoundToInt(h), 6, 2, RRTheme.Ink), Color.white).rectTransform.Stretch();

            var slider = root.gameObject.AddComponent<Slider>();
            slider.minValue = 0; slider.maxValue = 100; slider.wholeNumbers = false;
            slider.transition = Selectable.Transition.None; slider.targetGraphic = bg;

            var area = RRUI.Node("Handle Slide Area", root);
            area.anchorMin = new Vector2(0, 0); area.anchorMax = new Vector2(1, 1);
            area.offsetMin = new Vector2(handle / 2f, 0); area.offsetMax = new Vector2(-handle / 2f, 0);

            var hImg = RRUI.Img("Handle", area, RRSprites.Circle(handle), RRTheme.Cyan, raycast: true);
            var hrt = hImg.rectTransform;
            hrt.anchorMin = hrt.anchorMax = new Vector2(0, 0.5f); hrt.pivot = new Vector2(0.5f, 0.5f);
            hrt.sizeDelta = new Vector2(handle, handle);
            // ink border ring drawn directly on the handle
            RRUI.Img("ring", hrt, RRSprites.RoundedRing(handle, handle, handle / 2, 3, RRTheme.Ink), Color.white).rectTransform.Stretch();

            slider.handleRect = hrt;
            slider.SetValueWithoutNotify(value);
            return slider;
        }

        // ---------- Settings toggle ----------
        public static RRToggleFX Toggle(RectTransform parent, bool on)
        {
            const float w = 64f, h = 32f; int knob = 22;
            var root = RRUI.Node("Toggle", parent); root.Size(w, h);
            var bg = RRUI.Img("bg", root, RRSprites.Rounded(16), on ? RRTheme.Cyan : RRTheme.A(Color.white, 0.08f), sliced: true, raycast: true);
            bg.rectTransform.Stretch();
            RRUI.Img("border", root, RRSprites.RoundedRing(Mathf.RoundToInt(w), Mathf.RoundToInt(h), 16, 3, RRTheme.Ink), Color.white).rectTransform.Stretch();

            var kn = RRUI.Img("knob", root, RRSprites.Circle(knob), Color.white);
            kn.rectTransform.anchorMin = kn.rectTransform.anchorMax = new Vector2(0, 0.5f);
            kn.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            kn.rectTransform.sizeDelta = new Vector2(knob, knob);
            RRUI.Img("knob-ring", kn.rectTransform, RRSprites.RoundedRing(knob, knob, knob / 2, 2, RRTheme.Ink), Color.white).rectTransform.Stretch();

            float offX = 2 + knob / 2f, onX = w - 2 - knob / 2f;
            var fx = root.gameObject.AddComponent<RRToggleFX>();
            fx.Init(bg, kn.rectTransform, offX, onX, RRTheme.A(Color.white, 0.08f), RRTheme.Cyan, on);
            return fx;
        }

        // ---------- Settings segmented control ----------
        public static RRSegmentedFX Segmented(RectTransform parent, string[] options, int selected)
        {
            const float h = 36f, btnPad = 16f, fontSize = 13f;
            var probe = RRUI.Text("probe", parent, "", RRFonts.UiBold, fontSize, Color.white);
            float[] bw = new float[options.Length];
            float total = 0;
            for (int i = 0; i < options.Length; i++) { bw[i] = probe.GetPreferredValues(options[i]).x + btnPad * 2f; total += bw[i]; }
            Object.Destroy(probe.gameObject);

            var root = RRUI.Node("Segmented", parent); root.Size(total + 6, h);
            RRUI.Img("border", root, RRSprites.Rounded(RRTheme.RMd), RRTheme.Ink, sliced: true).rectTransform.Stretch();
            var inner = RRUI.Node("inner", root); inner.Stretch(3, 3, 3, 3);

            var fx = root.gameObject.AddComponent<RRSegmentedFX>();
            float x = 0;
            for (int i = 0; i < options.Length; i++)
            {
                bool sel = i == selected;
                var cell = RRUI.Node("opt" + i, inner);
                cell.anchorMin = cell.anchorMax = new Vector2(0, 0.5f); cell.pivot = new Vector2(0, 0.5f);
                cell.sizeDelta = new Vector2(bw[i], h - 6); cell.anchoredPosition = new Vector2(x, 0);

                var bg = RRUI.Img("bg", cell, RRSprites.Rounded(2), sel ? RRTheme.Purple : RRTheme.A(Color.white, 0.05f), sliced: true, raycast: true);
                bg.rectTransform.Stretch();
                var lab = RRUI.Text("lbl", cell, options[i], RRFonts.UiBold, fontSize, sel ? RRTheme.Fg1 : RRTheme.Fg2);
                lab.characterSpacing = 3; lab.rectTransform.Stretch();

                var btn = cell.gameObject.AddComponent<Button>(); btn.transition = Selectable.Transition.None; btn.targetGraphic = bg;
                fx.Register(i, bg, lab, btn);
                x += bw[i];
            }
            fx.Select(selected, notify: false);
            return fx;
        }

        // ---------- Key-hint pill row (e.g. SPACE jump · ESC pause) ----------
        public static RectTransform KeyHint(RectTransform parent)
        {
            var root = RRUI.Node("KeyHint", parent);
            var t = RRUI.Text("t", root, "<b>SPACE</b> jump   <b>ESC</b> pause", RRFonts.Hud, 11, RRTheme.Fg2, TextAlignmentOptions.Right);
            t.rectTransform.Stretch();
            root.Size(180, 16);
            return root;
        }
    }
}
