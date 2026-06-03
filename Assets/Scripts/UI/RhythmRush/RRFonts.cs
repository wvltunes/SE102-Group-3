using UnityEngine;
using TMPro;

namespace RhythmRush.UI
{
    /// <summary>
    /// Loads the brand fonts (Bangers / Orbitron / Chakra Petch) from
    /// Resources/RhythmRush/Fonts and turns them into dynamic TMP font assets
    /// at runtime — no editor Font Asset Creator step required.
    ///
    ///   Display (Bangers)      — graffiti headers, combo, titles
    ///   Hud / HudX (Orbitron)  — numeric HUD / scoreboard data (700 / 800)
    ///   Ui / UiBold (Chakra)   — UI labels and body (500 / 700)
    /// </summary>
    public static class RRFonts
    {
        const string Root = "RhythmRush/Fonts/";

        static TMP_FontAsset _display, _hud, _hudX, _ui, _uiBold;

        public static TMP_FontAsset Display { get { if (_display == null) _display = Load("Bangers-Regular");        return _display; } }
        public static TMP_FontAsset Hud     { get { if (_hud     == null) _hud     = Load("Orbitron-Bold");          return _hud;     } }
        public static TMP_FontAsset HudX    { get { if (_hudX    == null) _hudX    = Load("Orbitron-ExtraBold");     return _hudX;    } }
        public static TMP_FontAsset Ui      { get { if (_ui      == null) _ui      = Load("ChakraPetch-Medium");     return _ui;      } }
        public static TMP_FontAsset UiBold  { get { if (_uiBold  == null) _uiBold  = Load("ChakraPetch-Bold");       return _uiBold;  } }

        static TMP_FontAsset Load(string fileName)
        {
            Font font = Resources.Load<Font>(Root + fileName);
            if (font == null)
            {
                Debug.LogWarning($"[RhythmRush] Font '{fileName}' not found under Resources/{Root}. " +
                                 "Falling back to the default TMP font.");
                return TMP_Settings.defaultFontAsset;
            }

            TMP_FontAsset asset = TMP_FontAsset.CreateFontAsset(font);
            if (asset == null) return TMP_Settings.defaultFontAsset;
            asset.name = fileName;
            return asset;
        }
    }
}
