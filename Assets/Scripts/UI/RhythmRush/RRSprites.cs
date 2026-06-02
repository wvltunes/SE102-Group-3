using System.Collections.Generic;
using UnityEngine;

namespace RhythmRush.UI
{
    /// <summary>
    /// Runtime sprite factory. Everything the kit draws — rounded capsules,
    /// parallelogram energy segments, circles, neon-gradient backgrounds and the
    /// perspective grid floor — is generated procedurally here and cached, so the
    /// screens need no imported sprite atlas. Art PNGs (runner, orbs, star) are
    /// pulled from Resources/RhythmRush/Art and wrapped as sprites.
    /// </summary>
    public static class RRSprites
    {
        static readonly Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

        static readonly Color32 White = new Color32(255, 255, 255, 255);
        static readonly Color32 Clear = new Color32(255, 255, 255, 0);

        // ============================================================ ROUNDED RECTS

        /// <summary>9-sliced white rounded rect (tint via Image.color). Stretches to any size ≥ 2*radius.</summary>
        public static Sprite Rounded(int radius)
        {
            radius = Mathf.Max(1, radius);
            string key = "round_" + radius;
            if (_cache.TryGetValue(key, out var s)) return s;

            int size = radius * 2 + 4;       // a few center px so 9-slice has a middle band
            var tex = NewTex(size, size);
            var px = new Color32[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    px[y * size + x] = Fade(White, RoundedCoverage(x + 0.5f, y + 0.5f, size, size, radius));
            tex.SetPixels32(px); tex.Apply(false, false);

            var sp = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
            sp.name = key;
            _cache[key] = sp;
            return sp;
        }

        /// <summary>Exact-size rounded rect filled with a vertical gradient (top → bottom).</summary>
        public static Sprite RoundedFill(int w, int h, int radius, Color top, Color bottom)
        {
            w = Mathf.Max(2, w); h = Mathf.Max(2, h); radius = Mathf.Clamp(radius, 0, Mathf.Min(w, h) / 2);
            string key = $"fill_{w}_{h}_{radius}_{Hx(top)}_{Hx(bottom)}";
            if (_cache.TryGetValue(key, out var s)) return s;

            var tex = NewTex(w, h);
            var px = new Color32[w * h];
            for (int y = 0; y < h; y++)
            {
                float t = h <= 1 ? 0f : (float)y / (h - 1);     // y=0 bottom
                Color rgb = Color.Lerp(bottom, top, t);
                for (int x = 0; x < w; x++)
                {
                    float cov = RoundedCoverage(x + 0.5f, y + 0.5f, w, h, radius);
                    Color c = rgb; c.a *= cov;
                    px[y * w + x] = c;
                }
            }
            tex.SetPixels32(px); tex.Apply(false, false);
            var sp = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            sp.name = key; _cache[key] = sp; return sp;
        }

        /// <summary>Exact-size rounded rect filled with a horizontal gradient (left → right) — e.g. the song-progress fill.</summary>
        public static Sprite RoundedFillH(int w, int h, int radius, Color left, Color right)
        {
            w = Mathf.Max(2, w); h = Mathf.Max(2, h); radius = Mathf.Clamp(radius, 0, Mathf.Min(w, h) / 2);
            string key = $"fillh_{w}_{h}_{radius}_{Hx(left)}_{Hx(right)}";
            if (_cache.TryGetValue(key, out var s)) return s;

            var tex = NewTex(w, h);
            var px = new Color32[w * h];
            for (int x = 0; x < w; x++)
            {
                float t = w <= 1 ? 0f : (float)x / (w - 1);
                Color rgb = Color.Lerp(left, right, t);
                for (int y = 0; y < h; y++)
                {
                    float cov = RoundedCoverage(x + 0.5f, y + 0.5f, w, h, radius);
                    Color c = rgb; c.a *= cov;
                    px[y * w + x] = c;
                }
            }
            tex.SetPixels32(px); tex.Apply(false, false);
            var sp = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            sp.name = key; _cache[key] = sp; return sp;
        }

        /// <summary>Exact-size rounded outline ring (hollow centre) — used for panel borders / inner hairlines.</summary>
        public static Sprite RoundedRing(int w, int h, int radius, int thickness, Color color)
        {
            w = Mathf.Max(2, w); h = Mathf.Max(2, h); thickness = Mathf.Max(1, thickness);
            radius = Mathf.Clamp(radius, 0, Mathf.Min(w, h) / 2);
            string key = $"ring_{w}_{h}_{radius}_{thickness}_{Hx(color)}";
            if (_cache.TryGetValue(key, out var s)) return s;

            int innerR = Mathf.Max(0, radius - thickness);
            var tex = NewTex(w, h);
            var px = new Color32[w * h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float outer = RoundedCoverage(x + 0.5f, y + 0.5f, w, h, radius);
                    float inner = RoundedCoverage(x + 0.5f - thickness, y + 0.5f - thickness, w - thickness * 2, h - thickness * 2, innerR);
                    float a = Mathf.Clamp01(outer - inner);
                    Color c = color; c.a *= a;
                    px[y * w + x] = c;
                }
            tex.SetPixels32(px); tex.Apply(false, false);
            var sp = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            sp.name = key; _cache[key] = sp; return sp;
        }

        /// <summary>9-sliced rounded rect with a soft feathered edge — a real neon glow (tint via Image.color).
        /// Place behind an element, stretched out by <paramref name="feather"/> px on each side.</summary>
        public static Sprite RoundedSoft(int radius, int feather)
        {
            radius = Mathf.Max(1, radius); feather = Mathf.Max(1, feather);
            string key = $"soft_{radius}_{feather}";
            if (_cache.TryGetValue(key, out var s)) return s;

            int ext = radius + feather;
            int size = ext * 2 + 4;
            float c = size / 2f, half = (size - 2f * feather) / 2f; // inner rounded-rect half-extent
            var tex = NewTex(size, size);
            var px = new Color32[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(Mathf.Abs(x + 0.5f - c) - (half - radius), 0f);
                    float dy = Mathf.Max(Mathf.Abs(y + 0.5f - c) - (half - radius), 0f);
                    float sd = Mathf.Sqrt(dx * dx + dy * dy) - radius; // signed dist to rounded edge
                    float t = Mathf.Clamp01(1f - sd / feather);
                    px[y * size + x] = Fade(White, t * t * (3f - 2f * t));  // smoothstep falloff
                }
            tex.SetPixels32(px); tex.Apply(false, false);
            var sp = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect, new Vector4(ext, ext, ext, ext));
            sp.name = key; _cache[key] = sp; return sp;
        }

        // ============================================================ CIRCLE / PARALLELOGRAM

        /// <summary>Anti-aliased filled white circle (tint via Image.color).</summary>
        public static Sprite Circle(int diameter)
        {
            diameter = Mathf.Max(2, diameter);
            string key = "circle_" + diameter;
            if (_cache.TryGetValue(key, out var s)) return s;

            var tex = NewTex(diameter, diameter);
            var px = new Color32[diameter * diameter];
            float r = diameter / 2f, cx = r, cy = r;
            for (int y = 0; y < diameter; y++)
                for (int x = 0; x < diameter; x++)
                {
                    float d = Mathf.Sqrt((x + 0.5f - cx) * (x + 0.5f - cx) + (y + 0.5f - cy) * (y + 0.5f - cy));
                    px[y * diameter + x] = Fade(White, Mathf.Clamp01(r - d));   // 1px AA
                }
            tex.SetPixels32(px); tex.Apply(false, false);
            var sp = Sprite.Create(tex, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f), 100f);
            sp.name = key; _cache[key] = sp; return sp;
        }

        /// <summary>Skewed parallelogram (the −14° energy segment). Leans like "/". Tint via Image.color.</summary>
        public static Sprite Parallelogram(int w, int h, float skewDeg)
        {
            w = Mathf.Max(2, w); h = Mathf.Max(2, h);
            string key = $"para_{w}_{h}_{skewDeg}";
            if (_cache.TryGetValue(key, out var s)) return s;

            float off = Mathf.Tan(skewDeg * Mathf.Deg2Rad) * h;
            int total = w + Mathf.CeilToInt(Mathf.Abs(off)) + 1;
            var tex = NewTex(total, h);
            var px = new Color32[total * h];
            for (int y = 0; y < h; y++)
            {
                float shift = off * (h <= 1 ? 0 : (float)y / (h - 1));   // bottom 0 → top off
                for (int x = 0; x < total; x++)
                {
                    float left = shift, right = shift + w;
                    float cov = Mathf.Clamp01(Mathf.Min(x + 0.5f - left, right - (x + 0.5f)));
                    px[y * total + x] = Fade(White, cov);
                }
            }
            tex.SetPixels32(px); tex.Apply(false, false);
            var sp = Sprite.Create(tex, new Rect(0, 0, total, h), new Vector2(0.5f, 0.5f), 100f);
            sp.name = key; _cache[key] = sp; return sp;
        }

        // ============================================================ BACKGROUNDS

        public struct Stop { public float pos; public Color color; public Stop(float p, Color c) { pos = p; color = c; } }

        /// <summary>Radial gradient background (matches the CSS radial vignettes).</summary>
        public static Sprite RadialBg(int w, int h, float cx, float cy, float rx, float ry, Stop[] stops)
        {
            string key = $"radial_{w}_{h}_{cx}_{cy}_{rx}_{ry}_{stops.Length}_{Hx(stops[0].color)}_{Hx(stops[stops.Length - 1].color)}";
            if (_cache.TryGetValue(key, out var s)) return s;

            var tex = NewTex(w, h);
            var px = new Color32[w * h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float u = (x + 0.5f) / w, v = (y + 0.5f) / h;
                    float dx = (u - cx) / rx, dy = (v - cy) / ry;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);             // 0 centre → 1 edge
                    px[y * w + x] = Sample(stops, d);
                }
            tex.SetPixels32(px); tex.Apply(false, false);
            var sp = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
            sp.name = key; _cache[key] = sp; return sp;
        }

        /// <summary>The in-game neon stage backdrop (radial purple void).</summary>
        public static Sprite GameplayBg()
        {
            return RadialBg(256, 144, 0.5f, 0.92f, 1.1f, 1.0f, new[]
            {
                new Stop(0.00f, RRTheme.Hex("#3A2660")),
                new Stop(0.38f, RRTheme.Hex("#1A1430")),
                new Stop(0.78f, RRTheme.Hex("#0A0A12")),
                new Stop(1.00f, RRTheme.Hex("#0A0A12")),
            });
        }

        /// <summary>The deep-blue radial vignette behind menus / settings.</summary>
        public static Sprite MenuBg()
        {
            return RadialBg(256, 144, 0.5f, 0.55f, 1.1f, 1.0f, new[]
            {
                new Stop(0.00f, RRTheme.Hex("#243A63")),
                new Stop(0.45f, RRTheme.Hex("#131B34")),
                new Stop(1.00f, RRTheme.Hex("#0A0A12")),
            });
        }

        /// <summary>Perspective neon grid floor (cyan verticals converge to a vanishing point, purple horizontals recede).</summary>
        public static Sprite PerspectiveGrid(int w = 384, int h = 192)
        {
            string key = $"grid_{w}_{h}";
            if (_cache.TryGetValue(key, out var s)) return s;

            var buf = new Color[w * h];                  // start transparent
            float vpx = w / 2f, vpy = h;                 // vanishing point: top centre (far)
            Color cyan = RRTheme.A(RRTheme.Cyan, 0.55f);
            Color purp = RRTheme.A(RRTheme.Purple, 0.5f);

            // vertical lines fanning out from the vanishing point
            for (int i = -10; i <= 10; i++)
            {
                float xNear = vpx + i * (w * 0.13f);
                DrawLine(buf, w, h, xNear, 0, vpx, vpy - 1, cyan, 2);
            }
            // horizontal lines at perspective-compressed heights
            for (int k = 1; k <= 14; k++)
            {
                float y = h * (1f - 1f / (1f + k * 0.32f));
                DrawHLine(buf, w, h, y, purp, 2);
            }

            var tex = NewTex(w, h);
            var px = new Color32[w * h];
            for (int y = 0; y < h; y++)
            {
                float fade = 1f - (float)y / h;          // bright near (bottom), fades to horizon
                fade = Mathf.Clamp01(fade * 1.15f);
                for (int x = 0; x < w; x++)
                {
                    Color c = buf[y * w + x];
                    c.a *= fade;
                    px[y * w + x] = c;
                }
            }
            tex.SetPixels32(px); tex.Apply(false, false);
            var sp = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
            sp.name = key; _cache[key] = sp; return sp;
        }

        /// <summary>Repeating scanline tile (CRT overlay). Sized large (64×256) so a tiled Image stays well under the UGUI mesh limit.</summary>
        public static Sprite Scanlines()
        {
            string key = "scan";
            if (_cache.TryGetValue(key, out var s)) return s;
            int w = 64, h = 256;
            var tex = NewTex(w, h);
            var px = new Color32[w * h];
            for (int y = 0; y < h; y++)
            {
                byte a = (byte)((y % 4 == 0) ? 16 : 0);   // a faint line every 4px
                for (int x = 0; x < w; x++) px[y * w + x] = new Color32(255, 255, 255, a);
            }
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.SetPixels32(px); tex.Apply(false, false);
            var sp = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
            sp.name = key; _cache[key] = sp; return sp;
        }

        // ============================================================ ART (PNG → Sprite)

        public static Sprite Art(string fileName)
        {
            string key = "art_" + fileName;
            if (_cache.TryGetValue(key, out var s)) return s;
            var tex = LoadArtTex(fileName);
            if (tex == null) { Debug.LogWarning($"[RhythmRush] Art '{fileName}' missing under Resources/RhythmRush/Art."); return null; }
            var sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            sp.name = key; _cache[key] = sp; return sp;
        }

        /// <summary>Native pixel size of an art texture (for keeping aspect ratio).</summary>
        public static Vector2 ArtSize(string fileName)
        {
            var tex = LoadArtTex(fileName);
            return tex == null ? Vector2.one : new Vector2(tex.width, tex.height);
        }

        // Resources.Load wants the path without an extension.
        static Texture2D LoadArtTex(string fileName)
            => Resources.Load<Texture2D>("RhythmRush/Art/" + System.IO.Path.GetFileNameWithoutExtension(fileName));

        // ============================================================ internals

        static Texture2D NewTex(int w, int h)
        {
            var t = new Texture2D(w, h, TextureFormat.RGBA32, false, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave
            };
            return t;
        }

        static Color32 Fade(Color32 c, float a) { c.a = (byte)Mathf.Clamp(Mathf.RoundToInt(a * 255f), 0, 255); return c; }

        /// <summary>Anti-aliased coverage (0..1) of pixel centre (px,py) inside a w×h rounded rect with corner radius r.</summary>
        static float RoundedCoverage(float px, float py, float w, float h, float r)
        {
            if (w <= 0 || h <= 0) return 0f;
            r = Mathf.Clamp(r, 0, Mathf.Min(w, h) / 2f);
            float dx = Mathf.Max(Mathf.Abs(px - w / 2f) - (w / 2f - r), 0f);
            float dy = Mathf.Max(Mathf.Abs(py - h / 2f) - (h / 2f - r), 0f);
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            return Mathf.Clamp01(r - dist + 0.5f);   // 1px feather at the corner arc
        }

        static Color Sample(Stop[] stops, float t)
        {
            t = Mathf.Clamp01(t);
            if (t <= stops[0].pos) return stops[0].color;
            for (int i = 1; i < stops.Length; i++)
            {
                if (t <= stops[i].pos)
                {
                    float span = Mathf.Max(1e-5f, stops[i].pos - stops[i - 1].pos);
                    return Color.Lerp(stops[i - 1].color, stops[i].color, (t - stops[i - 1].pos) / span);
                }
            }
            return stops[stops.Length - 1].color;
        }

        static void DrawLine(Color[] buf, int w, int h, float x0, float y0, float x1, float y1, Color col, int thick)
        {
            float dx = x1 - x0, dy = y1 - y0;
            int steps = Mathf.CeilToInt(Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy))) + 1;
            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                Plot(buf, w, h, Mathf.RoundToInt(x0 + dx * t), Mathf.RoundToInt(y0 + dy * t), col, thick);
            }
        }

        static void DrawHLine(Color[] buf, int w, int h, float y, Color col, int thick)
        {
            int yi = Mathf.RoundToInt(y);
            for (int x = 0; x < w; x++) Plot(buf, w, h, x, yi, col, thick);
        }

        static void Plot(Color[] buf, int w, int h, int x, int y, Color col, int thick)
        {
            for (int oy = 0; oy < thick; oy++)
                for (int ox = 0; ox < thick; ox++)
                {
                    int xx = x + ox, yy = y + oy;
                    if (xx < 0 || yy < 0 || xx >= w || yy >= h) continue;
                    Color cur = buf[yy * w + xx];
                    if (col.a > cur.a) buf[yy * w + xx] = col;   // brightest wins
                }
        }

        static string Hx(Color c) => ((int)(c.r * 255) << 16 | (int)(c.g * 255) << 8 | (int)(c.b * 255)).ToString("X6") + ((int)(c.a * 255)).ToString("X2");
    }
}
