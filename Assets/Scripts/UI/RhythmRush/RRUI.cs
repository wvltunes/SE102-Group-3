using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RhythmRush.UI
{
    public enum RRBtn { Yellow, Pink, Cyan, Purple, Green, Ghost }

    /// <summary>
    /// UGUI construction helpers: RectTransform anchoring, TMP text, images, and the
    /// signature RhythmRush widgets — chunky capsule buttons (thick ink outline + hard
    /// violet drop-shadow + lift/sink press juice) and dark neon panels.
    /// Pure code; nothing here needs a prefab or a scene reference.
    /// </summary>
    public static class RRUI
    {
        // ============================================================ RECT / NODE HELPERS

        public static RectTransform Node(string name, RectTransform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.localScale = Vector3.one;
            return rt;
        }

        public static Image Img(string name, RectTransform parent, Sprite sprite, Color color, bool sliced = false, bool raycast = false)
        {
            var rt = Node(name, parent);
            var img = rt.gameObject.AddComponent<Image>();
            img.sprite = sprite;
            img.color = color;
            img.type = sliced ? Image.Type.Sliced : Image.Type.Simple;
            img.raycastTarget = raycast;
            return img;
        }

        public static TextMeshProUGUI Text(string name, RectTransform parent, string content,
            TMP_FontAsset font, float size, Color color, TextAlignmentOptions align = TextAlignmentOptions.Center)
        {
            var rt = Node(name, parent);
            var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
            t.font = font;
            t.fontSize = size;
            t.color = color;
            t.text = content;
            t.alignment = align;
            t.raycastTarget = false;
            t.richText = true;
            return t;
        }

        /// <summary>Apply a TMP ink outline + optional neon glow (the brand's "sticker" headline look).</summary>
        public static void Neon(TextMeshProUGUI t, Color outline, float outlineWidth, Color? glow = null, float glowOuter = 0.45f)
        {
            try
            {
                t.outlineColor = outline;
                t.outlineWidth = outlineWidth;
                if (glow.HasValue)
                {
                    Material m = t.fontMaterial; // instanced
                    m.EnableKeyword(ShaderUtilities.Keyword_Glow);
                    m.SetColor(ShaderUtilities.ID_GlowColor, glow.Value);
                    m.SetFloat(ShaderUtilities.ID_GlowOuter, glowOuter);
                    m.SetFloat(ShaderUtilities.ID_GlowInner, 0.1f);
                    m.SetFloat(ShaderUtilities.ID_GlowPower, 1f);
                }
            }
            catch { /* shader variant unavailable — outline/glow are decorative, ignore */ }
        }

        // ============================================================ ANCHORING EXTENSIONS

        public static RectTransform Stretch(this RectTransform rt, float l = 0, float t = 0, float r = 0, float b = 0)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(l, b); rt.offsetMax = new Vector2(-r, -t);
            return rt;
        }

        /// <summary>CSS-style placement: x from left, y from top, with a fixed size.</summary>
        public static RectTransform TopLeft(this RectTransform rt, float x, float y, float w, float h)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1); rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(w, h); rt.anchoredPosition = new Vector2(x, -y);
            return rt;
        }

        public static RectTransform TopRight(this RectTransform rt, float xFromRight, float yFromTop, float w, float h)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(1, 1); rt.pivot = new Vector2(1, 1);
            rt.sizeDelta = new Vector2(w, h); rt.anchoredPosition = new Vector2(-xFromRight, -yFromTop);
            return rt;
        }

        public static RectTransform TopCenter(this RectTransform rt, float yFromTop, float w, float h)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1); rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(w, h); rt.anchoredPosition = new Vector2(0, -yFromTop);
            return rt;
        }

        public static RectTransform BottomLeft(this RectTransform rt, float x, float yFromBottom, float w, float h)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0, 0); rt.pivot = new Vector2(0, 0);
            rt.sizeDelta = new Vector2(w, h); rt.anchoredPosition = new Vector2(x, yFromBottom);
            return rt;
        }

        public static RectTransform BottomStretch(this RectTransform rt, float height)
        {
            rt.anchorMin = new Vector2(0, 0); rt.anchorMax = new Vector2(1, 0); rt.pivot = new Vector2(0.5f, 0);
            rt.offsetMin = new Vector2(0, 0); rt.offsetMax = new Vector2(0, 0);
            rt.sizeDelta = new Vector2(0, height); rt.anchoredPosition = new Vector2(0, 0);
            return rt;
        }

        /// <summary>Centre in parent with a fixed size, offset (dx right, dy up).</summary>
        public static RectTransform Mid(this RectTransform rt, float w, float h, float dx = 0, float dy = 0)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(w, h); rt.anchoredPosition = new Vector2(dx, dy);
            return rt;
        }

        public static RectTransform Size(this RectTransform rt, float w, float h) { rt.sizeDelta = new Vector2(w, h); return rt; }

        // ---- position-only helpers (keep the element's existing size) ----
        public static RectTransform AtTopCenter(this RectTransform rt, float yFromTop)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1); rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -yFromTop); return rt;
        }

        public static RectTransform AtMid(this RectTransform rt, float dx = 0, float dy = 0)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(dx, dy); return rt;
        }

        public static RectTransform AtLeftCenter(this RectTransform rt, float xFromLeft)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0, 0.5f); rt.pivot = new Vector2(0, 0.5f);
            rt.anchoredPosition = new Vector2(xFromLeft, 0); return rt;
        }

        public static RectTransform AtRightCenter(this RectTransform rt, float xFromRight)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(1, 0.5f); rt.pivot = new Vector2(1, 0.5f);
            rt.anchoredPosition = new Vector2(-xFromRight, 0); return rt;
        }

        // ============================================================ CAPSULE BUTTON

        public sealed class Cap
        {
            public RectTransform root;
            public Button button;
            public TextMeshProUGUI label;
        }

        public static Cap Capsule(RectTransform parent, string text, RRBtn variant, bool small = false)
        {
            float fontSize = small ? 18f : 26f;
            float padX     = small ? 20f : 34f;
            float height   = small ? 40f : 54f;
            int radius     = Mathf.Min(RRTheme.RLg, Mathf.FloorToInt(height / 2f));
            float border   = small ? 2.5f : 3f;

            Color fill, txt, outline = RRTheme.Ink;
            bool ghost = variant == RRBtn.Ghost;
            switch (variant)
            {
                case RRBtn.Pink:   fill = RRTheme.Pink;   txt = RRTheme.Fg1;   break;
                case RRBtn.Cyan:   fill = RRTheme.Cyan;   txt = RRTheme.FgInk; break;
                case RRBtn.Purple: fill = RRTheme.Purple; txt = RRTheme.Fg1;   break;
                case RRBtn.Green:  fill = RRTheme.Green;  txt = RRTheme.FgInk; break;
                case RRBtn.Ghost:  fill = new Color(12 / 255f, 15 / 255f, 28 / 255f, 1f); txt = RRTheme.Cyan; outline = RRTheme.Cyan; break;
                default:           fill = RRTheme.Yellow; txt = RRTheme.FgInk; break;
            }

            var root = Node("Btn:" + text, parent);
            var hit = root.gameObject.AddComponent<Image>();          // transparent raycast / click target
            hit.color = new Color(0, 0, 0, 0);
            hit.raycastTarget = true;

            var btn = root.gameObject.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;
            btn.targetGraphic = hit;

            var rounded = RRSprites.Rounded(radius);
            Vector2 drop = small ? RRTheme.DropSm : RRTheme.DropMd;
            if (ghost) drop = new Vector2(drop.x + 3f, drop.y + 4f); // a touch larger so it peeks past the even glow

            // shadow (hard violet offset, no blur) — drawn first (bottom layer)
            var shadow = Img("shadow", root, rounded, RRTheme.Shadow, sliced: true);
            shadow.rectTransform.Stretch();
            shadow.rectTransform.anchoredPosition = new Vector2(drop.x, -drop.y);

            // ghost: even soft neon glow, drawn over the shadow so the halo is symmetric all around
            if (ghost)
            {
                const int feather = 8;
                var glow = Img("glow", root, RRSprites.RoundedSoft(radius, feather), RRTheme.A(RRTheme.Cyan, 0.5f), sliced: true);
                glow.rectTransform.Stretch(-feather, -feather, -feather, -feather);
            }

            // body = outline + fill + label, moves as one on hover/press
            var body = Node("body", root); body.Stretch();
            Img("outline", body, rounded, outline, sliced: true).rectTransform.Stretch();
            Img("fill", body, rounded, fill, sliced: true).rectTransform.Stretch(border, border, border, border);

            var label = Text("label", body, text, RRFonts.Display, fontSize, txt);
            label.rectTransform.Stretch();
            label.alignment = TextAlignmentOptions.Center;
            label.margin = new Vector4(0, 0, 0, small ? 3 : 5); // Bangers sits low; nudge baseline up
            RRUI.Neon(label, RRTheme.A(RRTheme.Ink, 0.35f), 0.12f, ghost ? RRTheme.Cyan : (Color?)null, 0.35f);

            // auto-size to text (CSS inline-flex)
            float textW = label.GetPreferredValues(text).x;
            float w = Mathf.Ceil(textW + padX * 2f);
            root.sizeDelta = new Vector2(w, height);

            var fx = root.gameObject.AddComponent<RRButtonFX>();
            fx.Init(body, shadow.rectTransform, drop);

            return new Cap { root = root, button = btn, label = label };
        }

        /// <summary>Resize every capsule to the widest one's width (labels stay centered). Use for uniform button stacks.</summary>
        public static void Equalize(params Cap[] caps)
        {
            float max = 0f;
            foreach (var c in caps) max = Mathf.Max(max, c.root.sizeDelta.x);
            foreach (var c in caps) c.root.sizeDelta = new Vector2(max, c.root.sizeDelta.y);
        }

        // ============================================================ ICON BUTTON (ghost, square)

        public static Button IconButton(RectTransform parent, Sprite icon, float size = 52f)
        {
            int radius = Mathf.Min(RRTheme.RLg, Mathf.FloorToInt(size / 2f));
            var root = Node("IconBtn", parent);
            root.Size(size, size);
            var hit = root.gameObject.AddComponent<Image>(); hit.color = new Color(0, 0, 0, 0); hit.raycastTarget = true;
            var btn = root.gameObject.AddComponent<Button>(); btn.transition = Selectable.Transition.None; btn.targetGraphic = hit;

            var rounded = RRSprites.Rounded(radius);
            var shadow = Img("shadow", root, rounded, RRTheme.Shadow, sliced: true);
            shadow.rectTransform.Stretch(); shadow.rectTransform.anchoredPosition = new Vector2(RRTheme.DropSm.x, -RRTheme.DropSm.y);

            var body = Node("body", root); body.Stretch();
            Img("outline", body, rounded, RRTheme.Cyan, sliced: true).rectTransform.Stretch();
            Img("fill", body, rounded, new Color(20 / 255f, 24 / 255f, 42 / 255f, 0.85f), sliced: true).rectTransform.Stretch(2.5f, 2.5f, 2.5f, 2.5f);

            if (icon != null)
            {
                var img = Img("icon", body, icon, RRTheme.Fg1);
                Vector2 sz = icon.rect.size; float k = 22f / Mathf.Max(1f, sz.y);
                img.rectTransform.Mid(sz.x * k, 22f);
                img.preserveAspect = true;
            }

            var fx = root.gameObject.AddComponent<RRButtonFX>();
            fx.Init(body, shadow.rectTransform, RRTheme.DropSm);
            return btn;
        }

        // ============================================================ NEON PANEL

        /// <summary>Dark gradient panel with thick ink/neon border, hard drop-shadow and an inner purple hairline.</summary>
        public static RectTransform Panel(RectTransform parent, float w, float h, bool neon = true)
        {
            int W = Mathf.RoundToInt(w), H = Mathf.RoundToInt(h);
            var root = Node("Panel", parent);
            root.Mid(W, H);

            var shadow = Img("shadow", root, RRSprites.RoundedFill(W, H, RRTheme.RLg, RRTheme.Shadow, RRTheme.Shadow), Color.white);
            shadow.rectTransform.Stretch();
            shadow.rectTransform.anchoredPosition = new Vector2(RRTheme.DropMd.x, -RRTheme.DropMd.y);

            if (neon)
            {
                var glow = Img("glow", root, RRSprites.RoundedFill(W, H, RRTheme.RLg, RRTheme.Purple, RRTheme.Purple), RRTheme.A(RRTheme.Purple, 0.30f));
                glow.rectTransform.Stretch(-10, -10, -10, -10);
            }

            // gradient fill
            Img("fill", root, RRSprites.RoundedFill(W, H, RRTheme.RLg, RRTheme.A(RRTheme.PanelTop, 0.96f), RRTheme.A(RRTheme.PanelBot, 0.98f)), Color.white)
                .rectTransform.Stretch();
            // outer border
            Img("border", root, RRSprites.RoundedRing(W, H, RRTheme.RLg, 3, neon ? RRTheme.Purple : RRTheme.Ink), Color.white)
                .rectTransform.Stretch();
            // inner purple hairline
            Img("inner", root, RRSprites.RoundedRing(W - 8, H - 8, RRTheme.RLg - 4, 2, RRTheme.A(RRTheme.Purple, 0.35f)), Color.white)
                .rectTransform.Stretch(4, 4, 4, 4);

            // content holder (children added by callers)
            var content = Node("content", root); content.Stretch();
            return content;
        }
    }
}
