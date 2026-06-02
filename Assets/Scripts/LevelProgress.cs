using UnityEngine;

/// <summary>
/// Single source of truth for level-unlock progress, persisted in PlayerPrefs.
///
/// Progress is stored under one integer key, <see cref="UnlockedLevelKey"/>, whose
/// value is the NUMBER of unlocked levels (a count, not an index). This matches the
/// convention already used by DiscMenuController, where a 0-based slot <c>i</c> is
/// unlocked when <c>i &lt; UnlockedLevel</c>:
///
///   UnlockedLevel = 1  ->  Level 1 unlocked, Levels 2..5 locked  (fresh save)
///   UnlockedLevel = 3  ->  Levels 1..3 unlocked, Levels 4..5 locked
///
/// Keeping the read/write/unlock rules here means the level-select screen and the
/// level-complete screen can never drift apart on what "unlocked" means.
/// </summary>
public static class LevelProgress
{
    /// <summary>PlayerPrefs key holding the count of unlocked levels.</summary>
    public const string UnlockedLevelKey = "UnlockedLevel";

    /// <summary>
    /// How many levels are unlocked. Defaults to 1 (only Level 1) on a fresh save,
    /// matching <c>PlayerPrefs.GetInt("UnlockedLevel", 1)</c> used elsewhere.
    /// </summary>
    public static int UnlockedCount => PlayerPrefs.GetInt(UnlockedLevelKey, 1);

    /// <summary>
    /// True if the given 1-based level number (Level 1 = 1, Level 2 = 2, ...) is
    /// currently unlocked.
    /// </summary>
    public static bool IsUnlocked(int levelNumber)
    {
        return levelNumber <= UnlockedCount;
    }

    /// <summary>
    /// Ensures every level up to and including <paramref name="levelNumber"/> is
    /// unlocked. Only ever raises the stored count - it never re-locks progress - and
    /// persists immediately so the unlock survives the scene change that usually
    /// follows. Returns true if anything new was unlocked.
    /// </summary>
    public static bool UnlockUpTo(int levelNumber)
    {
        if (levelNumber <= UnlockedCount) return false;

        PlayerPrefs.SetInt(UnlockedLevelKey, levelNumber);
        PlayerPrefs.Save();
        Debug.Log($"[LevelProgress] Unlocked levels up to {levelNumber}.");
        return true;
    }

    /// <summary>
    /// Wipes all unlock progress back to the fresh-save default (only Level 1). Handy
    /// for a "reset progress" button or for testing the locked state.
    /// </summary>
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(UnlockedLevelKey);
        PlayerPrefs.Save();
    }
}
