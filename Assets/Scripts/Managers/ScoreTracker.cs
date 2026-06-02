using UnityEngine;

/// <summary>
/// Static class to track player score during gameplay.
/// Tracks:
/// - Total orbs collected in the level
/// - Song progress percentage when player dies/completes level
/// </summary>
public static class ScoreTracker
{
    private static int orbsCollected = 0;
    private static int totalOrbsInLevel = 0;
    private static float songProgressPercent = 0f;

    /// <summary>
    /// Initialize score tracker at level start.
    /// Call this from level initialization to set total orbs.
    /// </summary>
    public static void Initialize(int totalOrbs)
    {
        orbsCollected = 0;
        totalOrbsInLevel = totalOrbs;
        songProgressPercent = 0f;
        Debug.Log($"[ScoreTracker] Initialized with {totalOrbs} total orbs in level.");
    }

    /// <summary>
    /// Call this when player collects an orb.
    /// </summary>
    public static void AddOrb()
    {
        orbsCollected++;
        Debug.Log($"[ScoreTracker] Orb collected! Total: {orbsCollected}/{totalOrbsInLevel}");
    }

    /// <summary>
    /// Calculate and update song progress percentage.
    /// Call this when level ends (game over or level complete).
    /// </summary>
    public static void UpdateSongProgress()
    {
        if (AudioManager.instance == null)
        {
            Debug.LogWarning("[ScoreTracker] AudioManager not found, cannot calculate song progress.");
            songProgressPercent = 0f;
            return;
        }

        float currentTime = AudioManager.instance.GetCurrentTimeInSong();
        float totalDuration = AudioManager.instance.GetSongDuration();

        if (totalDuration > 0f)
        {
            songProgressPercent = (currentTime / totalDuration) * 100f;
            songProgressPercent = Mathf.Clamp(songProgressPercent, 0f, 100f);
            Debug.Log($"[ScoreTracker] Song progress: {songProgressPercent:F1}% ({currentTime:F2}s / {totalDuration:F2}s)");
        }
        else
        {
            songProgressPercent = 0f;
        }
    }

    /// <summary>
    /// Get formatted score display string.
    /// Format: "Orbs: 5/10 | Progress: 45%"
    /// </summary>
    public static string GetScoreDisplayText()
    {
        return $"Orbs: {orbsCollected}/{totalOrbsInLevel} | Progress: {songProgressPercent:F0}%";
    }

    /// <summary>
    /// Get orbs collected count.
    /// </summary>
    public static int GetOrbsCollected()
    {
        return orbsCollected;
    }

    /// <summary>
    /// Get total orbs in level.
    /// </summary>
    public static int GetTotalOrbs()
    {
        return totalOrbsInLevel;
    }

    /// <summary>
    /// Get song progress percentage (0-100).
    /// </summary>
    public static float GetSongProgressPercent()
    {
        return songProgressPercent;
    }

    /// <summary>
    /// Reset score tracker (for new level or testing).
    /// </summary>
    public static void Reset()
    {
        orbsCollected = 0;
        totalOrbsInLevel = 0;
        songProgressPercent = 0f;
        Debug.Log("[ScoreTracker] Reset.");
    }
}
