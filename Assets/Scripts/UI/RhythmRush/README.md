# RhythmRush — Screens (UGUI / Canvas recreation)

A pixel-faithful Unity UGUI recreation of `ui_kits/game/Screens.html` from the
**RhythmRush Design System** (Claude Design handoff bundle). The source deck contained
four screens, all rebuilt here:

1. **Pause Menu** — `PAUSED` + RESUME / RETRY / QUIT TO MENU
2. **Game Over** — `GAME OVER` + result grid + RETRY / QUIT
3. **Level Clear** — `LEVEL CLEAR!` + ★★★ rank + result grid + CONTINUE / RETRY / MENU
4. **Settings** — sliders, toggles, segmented note-speed, SAVE & CLOSE

> Main Menu, Character Select, Track Select and the Gameplay HUD were trimmed from the
> deck by the designer during the chat, so they are intentionally **not** screens here.
> (Pause / Game Over / Level Clear still render *over* the gameplay backdrop, exactly as
> the deck mounted them.)

## In the real game (automatic)

`RRGameScreenBinder` + `RRGameScreenBootstrap` make the neon screens **replace** the
project's cartoon ones during actual play. The bootstrap auto-installs into **every
scene that has a `GameManager`** (LevelBase, Level1-5, MechanicTest…) — no per-scene
wiring. In Play mode:

- **Esc** → neon **PAUSED** (Resume / Retry / Quit to menu)
- player death → neon **GAME OVER** (Retry / Quit)
- level complete → neon **LEVEL CLEAR** (Continue / Retry / Menu)

Buttons call the real actions (`GameManager.ResumeGame/RestartLevel/LoadNextLevel`,
`SceneTransitionManager`), level-unlock progress is preserved, and the old
`Game_Pause_Menu_Canvas` / `Game_Over_Menu_Canvas` / `Game_Level_Complete_Canvas` are
hidden at runtime.

> The old cartoon canvas may still be visible in the **edit-mode** Game view (it's saved
> active in the scene) — that's only at edit time; the binder hides it on Play.

To turn the takeover off and fall back to the original screens, set
`RhythmRush.UI.RRGameScreenBootstrap.Enabled = false` (or delete `RRGameScreenBinder.cs`).

## Standalone preview gallery

- **Menu:** `Tools ▸ RhythmRush ▸ Open Screen Gallery (new scene)` then press **Play**, or
  `Create Screen Gallery (current scene)`.
- **Navigate:** `←` / `→` (or `A` / `D`), or number keys `1`–`4`. Includes the Settings
  screen and the full neon gameplay backdrop.

Everything is built **in code at runtime** — no prefab or scene wiring required.

## Files

| File | Role |
|---|---|
| `RRTheme.cs` | Color / radius / spacing / shadow tokens — a port of `colors_and_type.css` |
| `RRFonts.cs` | Loads Bangers / Orbitron / Chakra Petch as **dynamic TMP font assets** at runtime |
| `RRSprites.cs` | Procedural sprites: rounded capsules, parallelogram energy segments, circles, neon-gradient backgrounds, perspective grid, scanlines + PNG→Sprite loader |
| `RRUI.cs` | UGUI builder: anchoring helpers, TMP text + neon styling, capsule buttons, icon button, neon panels |
| `RRWidgets.cs` | Energy bar, song-progress bar, score chip, star rank, result grid, slider, toggle, segmented control |
| `RRButtonFX.cs` / `RRToggleFX.cs` / `RRSegmentedFX.cs` | Hover/press juice + interactive state (unscaled-time, so they live on a paused screen) |
| `RRBackdrop.cs` | Shared in-game neon stage behind Pause / Game Over / Level Clear |
| `RRScreens.cs` | The four screens (gallery) + `*Overlay` builders (dim+modal) for live-game use |
| `RRScreenGallery.cs` | Canvas + 1280×720 clipped stage + screen switcher (standalone preview) |
| `RRGameScreenBinder.cs` | Drives the overlays in real gameplay + auto-install bootstrap |
| `Editor/RRGalleryMenu.cs` | `Tools ▸ RhythmRush` menu items |

## Design fidelity

- **Look:** chunky capsule buttons with a thick near-black outline (`#14101E`) and a hard
  violet offset drop-shadow (`#2E2A6B`, no blur); buttons lift on hover and sink into the
  shadow on press. Dark gradient panels with a glowing purple border + inner hairline.
  Parallelogram (−14°) energy segments. Neon text outline + glow on headers. A permanent
  scanline + corner-vignette CRT overlay. Colors and sizes come straight from the CSS.
- **Art:** the running character (Aria), orbs, star and treeline are the real PNGs the
  design system extracted from this build (under `Resources/RhythmRush/Art`).

## Wiring into the real game

Each screen builder accepts callbacks — e.g.
`RRScreens.Pause(parent, onResume: ResumeGame, onRetry: RestartLevel, onQuit: GoToMenu)` —
so you can hand them your existing `GameManager` / `PauseManager` / `SceneTransitionManager`
methods instead of the default debug logs. The fonts and widgets can also be reused on
their own.

## Caveats / things to confirm

- **Fonts** are the real Google Fonts the brief asked for (Bangers, Orbitron, Chakra
  Petch), loaded as dynamic TMP assets — the design system flagged Chakra Petch as a
  *substitute* for the unknown UI font; swap it if you have the original.
- The **perspective grid floor** is an approximation drawn procedurally (UGUI can't do a
  true 3D-projected mesh cheaply); the CSS used a `perspective()` transform.
- In-game results use the deck's 3-column layout (Score / Max Combo / Cleared|Accuracy).
  **Score** (orbs collected) and **Cleared** (song %) are real; **Max Combo** and
  **Accuracy** are sample stand-ins (`SampleMaxCombo` / `SampleAccuracy` in
  `RRGameScreenBinder`) until the game tracks them — swap them for real fields then.
  The **gallery** shows the deck's mock values throughout.
- The neon pause menu has **Resume / Retry / Tutorial / Quit to menu**. TUTORIAL opens a
  neon "HOW TO PLAY" sub-panel (controls: Space = jump up, S = drop down, Esc = pause)
  with a BACK button — replacing the project's old cartoon tutorial panel.
