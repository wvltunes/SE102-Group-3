using UnityEngine;

/// <summary>
/// Per-character GAME assets (in-level sprite + animator controllers), indexed to
/// match the character-select order. Lives in a Resources folder so any scene -
/// including a level launched directly in the Editor, or one reached without passing
/// through the character-select screen - can resolve the saved character index
/// (<see cref="CharacterManager.SelectedCharacterKey"/>) back into the right visuals.
///
/// This closes the gap that PlayerPrefs alone cannot: an index survives a cold start,
/// but the chosen sprite/animator (asset references) do not. <see cref="CharacterManager"/>
/// loads this roster from Resources and uses it to restore the look on any entry path.
///
/// Keep <see cref="entries"/> in the SAME order as the champions array on the
/// character-select screen so index N here is the same character as slot N there.
/// </summary>
[CreateAssetMenu(menuName = "Character UI/Game Roster")]
public class CharacterGameRoster : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        [Tooltip("In-level sprite for this character (fallback when no animator).")]
        public Sprite gameSprite;

        [Tooltip("Animator controller used while playing a level.")]
        public RuntimeAnimatorController gameAnimator;

        [Tooltip("Animator controller used on the main-menu character preview.")]
        public RuntimeAnimatorController menuAnimator;
    }

    [Tooltip("One entry per character, in character-select order (index 0 = first slot).")]
    public Entry[] entries;

    /// <summary>Returns the entry for <paramref name="index"/>, or null if out of range.</summary>
    public Entry Get(int index)
    {
        if (entries == null || index < 0 || index >= entries.Length) return null;
        return entries[index];
    }
}
