using TMPro;
using UnityEngine;

/// <summary>
/// Displays orbs count on Game Over screen (Score section).
/// </summary>
public class OrbsScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI orbsText;

    private void Awake()
    {
        FindOrbsText();
    }

    private void FindOrbsText()
    {
        // Method 1: Get from this object
        orbsText = GetComponent<TextMeshProUGUI>();
        if (orbsText != null)
        {
            Debug.Log($"[OrbsScoreDisplay] Found on self: {orbsText.gameObject.name}");
            return;
        }

        // Method 2: Get from direct children
        orbsText = GetComponentInChildren<TextMeshProUGUI>(includeInactive: false);
        if (orbsText != null)
        {
            Debug.Log($"[OrbsScoreDisplay] Found in children: {orbsText.gameObject.name}");
            return;
        }

        // Method 3: Get from all children including inactive
        orbsText = GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
        if (orbsText != null)
        {
            Debug.Log($"[OrbsScoreDisplay] Found in inactive children: {orbsText.gameObject.name}");
            return;
        }

        // Method 4: Find by name pattern
        TextMeshProUGUI[] allTMP = GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true);
        if (allTMP.Length > 0)
        {
            orbsText = allTMP[0];
            Debug.Log($"[OrbsScoreDisplay] Found first TMP: {orbsText.gameObject.name}");
            return;
        }

        Debug.LogError("[OrbsScoreDisplay] Could not find TextMeshProUGUI component anywhere!");
    }

    /// <summary>
    /// Call this when game over happens to update the orbs display.
    /// </summary>
    public void UpdateOrbsDisplay()
    {
        // Try to find again if lost
        if (orbsText == null)
        {
            FindOrbsText();
        }

        if (orbsText == null)
        {
            Debug.LogError("[OrbsScoreDisplay] orbsText is still null after search!");
            return;
        }

        int collected = ScoreTracker.GetOrbsCollected();
        
        orbsText.text = $"{collected}";

        Debug.Log($"[OrbsScoreDisplay] Updated: {collected} on {orbsText.gameObject.name}");
    }
}
