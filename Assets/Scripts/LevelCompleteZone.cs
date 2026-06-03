using UnityEngine;
using UnityEngine;

/// <summary>
/// Trigger volume placed at the finish line of a level. When the player enters
/// it, the <see cref="OnLevelComplete"/> event is raised so the GameManager can
/// transition the game into the LevelComplete state.
///
/// Setup: attach to a GameObject that has a Collider2D with "Is Trigger" enabled,
/// positioned where the level should be considered cleared.
///
/// Position can be set dynamically via LevelSequencer or manually adjusted here.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LevelCompleteZone : MonoBehaviour
{
    /// <summary>
    /// Raised once, the moment the player reaches the end of the level. Static so
    /// the GameManager can subscribe without holding a reference to this instance.
    /// </summary>
    public static event System.Action OnLevelComplete;

    /// <summary>
    /// Debug helper used to simulate level completion from keyboard shortcuts.
    /// </summary>
    public static void DebugTriggerLevelComplete()
    {
        OnLevelComplete?.Invoke();
    }

    [Tooltip("Tag used to identify the player object that completes the level.")]
    [SerializeField] private string playerTag = "Player";

    [Header("Position Settings")]
    [Tooltip("X position of the completion zone (world space)")]
    [SerializeField] private float positionX = 0f;

    [Tooltip("Y position of the completion zone (world space)")]
    [SerializeField] private float positionY = 0f;

    [Tooltip("Z position of the completion zone (world space)")]
    [SerializeField] private float positionZ = 0f;

    [Header("Collider Settings")]
    [Tooltip("Width of the trigger collider")]
    [SerializeField] private float colliderWidth = 2f;

    [Tooltip("Height of the trigger collider")]
    [SerializeField] private float colliderHeight = 8f;

    // Ensures the event is only raised once, even if the player has multiple
    // colliders or briefly re-enters the zone.
    private bool triggered = false;

    private void OnEnable()
    {
        // Apply position settings from inspector on enable
        ApplyPositionSettings();
    }

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
            // Victory stinger from the dedicated SFX source so it is heard even
            // though the GameManager pauses the music for the level-complete state.
            SfxManager.Instance?.PlayVictory();
            OnLevelComplete?.Invoke();
        }
    }

    /// <summary>
    /// Sets the position of this completion zone. Called by LevelSequencer.
    /// </summary>
    public void SetZonePosition(Vector3 newPosition)
    {
        positionX = newPosition.x;
        positionY = newPosition.y;
        positionZ = newPosition.z;
        ApplyPositionSettings();
        Debug.Log($"[LevelCompleteZone] Position set to ({positionX}, {positionY}, {positionZ})");
    }

    /// <summary>
    /// Sets the collider size. Called by LevelSequencer.
    /// </summary>
    public void SetColliderSize(float width, float height)
    {
        colliderWidth = width;
        colliderHeight = height;
        ApplyColliderSettings();
        Debug.Log($"[LevelCompleteZone] Collider size set to ({colliderWidth}, {colliderHeight})");
    }

    /// <summary>
    /// Applies position settings from serialized fields to transform
    /// </summary>
    private void ApplyPositionSettings()
    {
        transform.position = new Vector3(positionX, positionY, positionZ);
    }

    /// <summary>
    /// Applies collider settings from serialized fields
    /// </summary>
    private void ApplyColliderSettings()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = new Vector2(colliderWidth, colliderHeight);
        }
    }

    /// <summary>
    /// Resets the triggered state (for level restart)
    /// </summary>
    public void ResetZone()
    {
        triggered = false;
        Debug.Log("[LevelCompleteZone] Zone reset for new level attempt");
    }
}
