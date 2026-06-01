using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(5f, 0, -10f);

    [Header("Camera Bounds")]
    [SerializeField] private float minY = 0f;
    [SerializeField] private float maxY = 0f;

    [Header("Death Shake")]
    [Tooltip("How long the camera shakes after the player dies (seconds).")]
    [SerializeField] private float shakeDuration = 0.4f;
    [Tooltip("How far the camera is jolted at the start of the shake (world units).")]
    [SerializeField] private float shakeMagnitude = 0.35f;

    private Transform player;

    // Counts down while a shake is active. Driven by unscaled time so the shake
    // still plays after the GameManager freezes the game (Time.timeScale = 0) on death.
    private float shakeTimer = 0f;

    void Start()
    {
        // Auto-find Player in scene
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            player = playerController.transform;
        }
        else
        {
            Debug.LogError("[CameraFollow] Player not found in scene!");
        }
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerDeath += TriggerDeathShake;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDeath -= TriggerDeathShake;
    }

    /// <summary>
    /// Starts (or restarts) the death shake. Hooked to PlayerController.OnPlayerDeath.
    /// </summary>
    private void TriggerDeathShake()
    {
        shakeTimer = shakeDuration;
    }

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

        // Apply the death shake on top of the followed position.
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.unscaledDeltaTime;

            // Fade the jolt out over the shake's lifetime so it eases to a stop.
            float strength = shakeMagnitude * Mathf.Clamp01(shakeTimer / shakeDuration);
            Vector2 jitter = Random.insideUnitCircle * strength;
            basePosition += new Vector3(jitter.x, jitter.y, 0f);
        }

        transform.position = basePosition;
    }
}
