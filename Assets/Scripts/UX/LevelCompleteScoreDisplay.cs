using TMPro;
using UnityEngine;

/// <summary>
/// Displays score on Level Complete screen.
/// Attach this script to the TextMeshPro component that shows score on the Level Complete canvas.
/// </summary>
public class LevelCompleteScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        if (scoreText == null)
        {
            Debug.LogError("[LevelCompleteScoreDisplay] TextMeshProUGUI component not found!");
        }
    }

    private void Start()
    {
        // Subscribe to game state changes so we update when level completes
        // This will be called from GameManager when state changes to LevelComplete
    }

    /// <summary>
    /// Call this when level completes to update the score display.
    /// </summary>
    public void UpdateScore()
    {
        if (scoreText == null) return;

        // Calculate song progress before displaying
        ScoreTracker.UpdateSongProgress();

        // Get formatted score text
        string scoreDisplay = ScoreTracker.GetScoreDisplayText();
        scoreText.text = scoreDisplay;

        Debug.Log($"[LevelCompleteScoreDisplay] Updated: {scoreDisplay}");
    }
}
