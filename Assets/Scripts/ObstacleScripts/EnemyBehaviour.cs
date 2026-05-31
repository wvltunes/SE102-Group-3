using UnityEngine;

/// <summary>
/// Enemy object that kills the player on hitbox contact.
/// Uses a trigger collider for detection - any contact with the player results in death.
/// Similar to SpikeBehaviour but designed for enemy-type objects.
/// </summary>
public class EnemyBehaviour : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("If true, the enemy will reload the scene on contact (death)")]
    [SerializeField] private bool killsPlayer = true;
    
    [Header("Optional Animation")]
    [Tooltip("Speed of idle bobbing animation (0 = no bobbing)")]
    [SerializeField] private float bobSpeed = 0f;
    [Tooltip("Amplitude of idle bobbing animation")]
    [SerializeField] private float bobAmplitude = 0.1f;
    
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        // Optional idle bobbing animation
        if (bobSpeed > 0f)
        {
            Vector3 pos = transform.localPosition;
            pos.y = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
            transform.localPosition = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && killsPlayer)
        {
            // Let the player raise its death event so the GameManager owns the
            // game-over flow (same kill, handled centrally instead of here).
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.Die();
            }
        }
    }
}
