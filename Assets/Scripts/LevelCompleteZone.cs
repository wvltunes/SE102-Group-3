using UnityEngine;

/// <summary>
/// Trigger volume placed at the finish line of a level. When the player enters
/// it, the <see cref="OnLevelComplete"/> event is raised so the GameManager can
/// transition the game into the LevelComplete state.
///
/// Setup: attach to a GameObject that has a Collider2D with "Is Trigger" enabled,
/// positioned where the level should be considered cleared.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LevelCompleteZone : MonoBehaviour
{
    /// <summary>
    /// Raised once, the moment the player reaches the end of the level. Static so
    /// the GameManager can subscribe without holding a reference to this instance.
    /// </summary>
    public static event System.Action OnLevelComplete;

    [Tooltip("Tag used to identify the player object that completes the level.")]
    [SerializeField] private string playerTag = "Player";

    // Ensures the event is only raised once, even if the player has multiple
    // colliders or briefly re-enters the zone.
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        // Skip triggering in MechanicTest scene (test/development only)
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MechanicTest")
        {
            return;
        }

        if (other.CompareTag(playerTag))
        {
            triggered = true;
            OnLevelComplete?.Invoke();
        }
    }
}
