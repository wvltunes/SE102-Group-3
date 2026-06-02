using UnityEngine;

/// <summary>
/// Persistent holder for the player's chosen character.
///
/// Survives scene loads via <see cref="DontDestroyOnLoad"/> and also remembers the
/// choice across app restarts by mirroring the selected index into PlayerPrefs under
/// <see cref="SelectedCharacterKey"/> ("SelectedCharacter"). This lets a gameplay
/// scene recover the selection even on a cold start that skipped the character-select
/// screen (e.g. launching a level directly from the Editor).
///
/// Visuals (sprite/animators) are pushed in from the selection screen at runtime.
/// An optional <see cref="roster"/> of <see cref="CharacterUIData"/> ScriptableObjects
/// provides per-character STATS (speed/energy) and a fallback sprite so the choice can
/// be resolved purely from the saved index when no live selection exists.
/// </summary>
public class CharacterManager : MonoBehaviour
{
    /// <summary>PlayerPrefs key holding the 0-based index of the chosen character.</summary>
    public const string SelectedCharacterKey = "SelectedCharacter";

    public static CharacterManager instance;

    [Header("Roster (optional) — one CharacterUIData per selectable character")]
    [Tooltip("Lets gameplay resolve the saved index to a character's stats (and a " +
             "fallback sprite) even when the selection screen was skipped. Leave empty " +
             "to rely solely on the runtime selection pushed from the select screen.")]
    public CharacterUIData[] roster;

    [Header("Runtime selection")]
    public Sprite selectedSprite;
    public RuntimeAnimatorController selectedAnimator;      // animator trong game
    public RuntimeAnimatorController selectedMenuAnimator;  // animator ở main menu
    public int selectedIndex = -1;

    // Per-character stats normalized to 0..1 (0 = min, 1 = max). A negative value
    // means "not set", in which case gameplay keeps its own default speed/energy.
    public float selectedSpeed01 = -1f;
    public float selectedEnergy01 = -1f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Restore a previously saved choice so a fresh launch straight into a level
        // still shows the right character. Only fills in what the selection screen has
        // not already set this session.
        if (selectedIndex < 0)
        {
            int saved = PlayerPrefs.GetInt(SelectedCharacterKey, -1);
            if (saved >= 0)
            {
                selectedIndex = saved;
                ApplyFromRoster(saved, includeSprite: true);
            }
        }
    }

    /// <summary>
    /// Records the chosen character (visuals) and persists the index to PlayerPrefs.
    /// Stats are pulled from the roster entry when one is assigned for this index.
    /// </summary>
    public void SetSelection(int index, Sprite sprite,
                             RuntimeAnimatorController gameAnimator,
                             RuntimeAnimatorController menuAnimator,
                             float speed01 = -1f, float energy01 = -1f)
    {
        selectedIndex = index;
        if (sprite != null) selectedSprite = sprite;
        if (gameAnimator != null) selectedAnimator = gameAnimator;
        if (menuAnimator != null) selectedMenuAnimator = menuAnimator;

        if (speed01 >= 0f) selectedSpeed01 = speed01;
        if (energy01 >= 0f) selectedEnergy01 = energy01;

        // Roster stats (if any) take precedence over loosely-typed UI values, but
        // never clobber the in-game sprite that was just set from the select screen.
        ApplyFromRoster(index, includeSprite: false);

        Save();
    }

    /// <summary>Persists just the current selected index to PlayerPrefs.</summary>
    public void Save()
    {
        PlayerPrefs.SetInt(SelectedCharacterKey, selectedIndex);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Fills stats (and optionally the sprite) from the roster entry for
    /// <paramref name="index"/>. No-op when the roster is missing or the index is out
    /// of range. Animators are not part of <see cref="CharacterUIData"/>, so they are
    /// left untouched.
    /// </summary>
    public void ApplyFromRoster(int index, bool includeSprite)
    {
        if (roster == null || index < 0 || index >= roster.Length) return;
        CharacterUIData data = roster[index];
        if (data == null) return;

        if (includeSprite && data.characterRender != null)
            selectedSprite = data.characterRender;

        selectedSpeed01 = Mathf.Clamp01(data.speed / 100f);
        selectedEnergy01 = Mathf.Clamp01(data.energy / 100f);
    }

    /// <summary>Returns the roster entry for the current selection, or null.</summary>
    public CharacterUIData GetSelectedData()
    {
        if (roster == null || selectedIndex < 0 || selectedIndex >= roster.Length) return null;
        return roster[selectedIndex];
    }
}
