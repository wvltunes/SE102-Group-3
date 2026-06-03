using UnityEngine;

namespace RhythmRush.UI
{
    /// <summary>
    /// RhythmRush design tokens — a 1:1 port of colors_and_type.css.
    /// Graffiti street-art × neon sci-fi. Full-saturation neon on near-black.
    /// All screens pull their colors, radii, spacing and shadow offsets from here.
    /// </summary>
    public static class RRTheme
    {
        // ---------- CORE NEON PALETTE ----------
        public static readonly Color Pink   = Hex("#FF3C6E"); // primary / danger / combo
        public static readonly Color Cyan   = Hex("#00FFB3"); // success / energy
        public static readonly Color Purple = Hex("#7B5CFF"); // secondary / magic
        public static readonly Color Yellow = Hex("#FFE14D"); // PLAY / score / clear
        public static readonly Color Green  = Hex("#6FE03C"); // continue / go
        public static readonly Color Orange = Hex("#FF8A3C"); // warnings / medals

        // ---------- NEON GLOW VARIANTS ----------
        public static readonly Color CyanGlow   = Hex("#2BFFC6");
        public static readonly Color PurpleGlow = Hex("#9B82FF");

        // ---------- INK & OUTLINES ----------
        public static readonly Color Ink     = Hex("#14101E"); // thick cartoon outline
        public static readonly Color InkSoft = Hex("#221B33"); // soft outline / dividers
        public static readonly Color Shadow  = Hex("#2E2A6B"); // violet hard drop-shadow

        // ---------- BACKGROUNDS ----------
        public static readonly Color Void    = Hex("#0A0A12"); // deepest stage bg
        public static readonly Color Deep     = Hex("#0E0F1A"); // panel base
        public static readonly Color Night    = Hex("#14182A"); // raised surface
        public static readonly Color PanelTop = Hex("#1C213A"); // card gradient top
        public static readonly Color PanelBot = Hex("#0D0F1C"); // card gradient bottom

        // ---------- TEXT COLORS ----------
        public static readonly Color Fg1   = Hex("#FFFFFF"); // primary on dark
        public static readonly Color Fg2   = Hex("#B7BECF"); // secondary / muted
        public static readonly Color Fg3   = Hex("#6E768C"); // tertiary / disabled
        public static readonly Color FgInk = Hex("#14101E"); // dark text on bright fills

        // ---------- RADII (px) ----------
        public const int RSm   = 8;
        public const int RMd   = 14;
        public const int RLg    = 22;  // chunky capsule buttons / panels
        public const int RThumb = 18;  // thumbnails

        // ---------- OUTLINE WEIGHTS (px) ----------
        public const float Outline     = 3f;
        public const float OutlineBold = 5f;

        // ---------- HARD DROP-SHADOW OFFSETS (x right, y down — no blur) ----------
        public static readonly Vector2 DropSm = new Vector2(4f, 4f);
        public static readonly Vector2 DropMd = new Vector2(6f, 7f);

        // ---------- SPACING SCALE (8px base) ----------
        public const float S1 = 4f, S2 = 8f, S3 = 12f, S4 = 16f, S5 = 24f, S6 = 32f, S7 = 48f, S8 = 64f;

        // ---------- helpers ----------
        public static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color c);
            return c;
        }

        /// <summary>Return a copy of <paramref name="c"/> with alpha <paramref name="a"/> (0..1).</summary>
        public static Color A(Color c, float a) { c.a = a; return c; }
    }
}
