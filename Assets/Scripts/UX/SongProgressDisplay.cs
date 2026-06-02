using TMPro;
using UnityEngine;

/// <summary>
/// Displays song progress percentage on Game Over screen (Process section).
/// </summary>
public class SongProgressDisplay : MonoBehaviour
{
    private TextMeshProUGUI progressText;
    
    private void Awake()
    {
        FindProgressText();
    }

    private void FindProgressText()
    {
        // Method 1: Get from this object (this is the CORRECT method if attached to the progress text itself)
        progressText = GetComponent<TextMeshProUGUI>();
        if (progressText != null)
        {
            Debug.Log($"[SongProgressDisplay] ✓ Found on self: {progressText.gameObject.name}, Current text: '{progressText.text}'");
            return;
        }

        Debug.LogWarning($"[SongProgressDisplay] ✗ NOT found on self! Searching elsewhere...");

        // Method 2: Get from direct children
        progressText = GetComponentInChildren<TextMeshProUGUI>(includeInactive: false);
        if (progressText != null)
        {
            Debug.Log($"[SongProgressDisplay] Found in children: {progressText.gameObject.name}");
            return;
        }

        // Method 3: Get from all children including inactive
        progressText = GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
        if (progressText != null)
        {
            Debug.Log($"[SongProgressDisplay] Found in inactive children: {progressText.gameObject.name}");
            return;
        }

        // Method 4: Find by name pattern
        TextMeshProUGUI[] allTMP = GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true);
        if (allTMP.Length > 0)
        {
            progressText = allTMP[0];
            Debug.Log($"[SongProgressDisplay] Found first TMP: {progressText.gameObject.name}");
            return;
        }

        Debug.LogError("[SongProgressDisplay] Could not find TextMeshProUGUI component anywhere!");
    }

    /// <summary>
    /// Call this when game over/level complete happens to update the progress display.
    /// </summary>
    public void UpdateProgressDisplay()
    {
        // Try to find again if lost
        if (progressText == null)
        {
            Debug.LogWarning("[SongProgressDisplay] progressText is null, searching...");
            FindProgressText();
        }

        if (progressText == null)
        {
            Debug.LogError("[SongProgressDisplay] ✗ progressText is still null after search! Component may not be attached to correct object.");
            return;
        }

        // Calculate song progress if not done yet
        ScoreTracker.UpdateSongProgress();

        float progress = ScoreTracker.GetSongProgressPercent();
        string oldText = progressText.text;
        progressText.text = $"{progress:F0}%";

        Debug.Log($"[SongProgressDisplay] ✓ Updated on '{progressText.gameObject.name}' (ID: {progressText.gameObject.GetInstanceID()}): '{oldText}' → '{progressText.text}' ({progress:F1}%)");
    }
}
