using UnityEngine;

public class KillZoneFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(-5f, 0, -10f);

    [Header("Kill Zone Bounds")]
    [SerializeField] private float minY = 0f;
    [SerializeField] private float maxY = 0f;

    private Transform player;

    void Start()
    {
        // Auto-find Player in scene
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            player = playerController.transform;
        }
        else
        {
            Debug.LogError("[KillZoneFollow] Player not found in scene!");
        }
    }

    /// <summary>
    /// Starts (or restarts) the death shake. Hooked to PlayerController.OnPlayerDeath.
    /// </summary>

    void LateUpdate()
    {
        if (player == null) return;

        // Target position = player position + offset
        Vector3 targetPosition = player.position + offset;

        // Clamp Y position (prevent camera from going too high/low)
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        // Smooth camera movement. Note: once the player dies the game is frozen
        // (Time.timeScale = 0), so Time.deltaTime is 0 and the camera simply holds
        // its last followed position - which is exactly what we want under it.
        Vector3 basePosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        transform.position = basePosition;
    }
}
