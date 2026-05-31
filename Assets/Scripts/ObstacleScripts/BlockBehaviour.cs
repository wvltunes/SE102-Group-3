using UnityEngine;

/// <summary>
/// Block object that acts as a solid obstacle.
/// When the player touches the top or bottom of the block, it is treated as ground.
/// The block uses BoxCollider2D for hitbox-based collision detection.
/// Attach this script to a Block prefab with a BoxCollider2D (NOT trigger) and a Rigidbody2D (kinematic).
/// </summary>
public class BlockBehaviour : MonoBehaviour
{
    [Header("Ground Detection Settings")]
    [Tooltip("How far from the top/bottom edge of the block to consider as ground contact")]
    [SerializeField] private float groundContactThreshold = 0.3f;

    [Header("Death Settings")]
    [Tooltip("Kill the player when they run head-on into a side (face) of the block")]
    [SerializeField] private bool killOnSideCollision = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("BlockBehaviour requires a BoxCollider2D component!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerContact(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerContact(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Notify GroundDetector that player left the block
            GroundDetector groundDetector = collision.gameObject.GetComponentInChildren<GroundDetector>();
            if (groundDetector != null)
            {
                groundDetector.SetBlockGround(false);
            }
        }
    }

    private void HandlePlayerContact(Collision2D collision)
    {
        if (boxCollider == null) return;

        // Check each contact point to determine if player is on top or bottom
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // contact.normal points FROM the block TOWARD the player
            // If normal.y > 0 => player is on TOP of the block (ground from above)
            // If normal.y < 0 => player is on BOTTOM of the block (ground from below / ceiling)
            if (Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x))
            {
                // Vertical contact (top or bottom) => treat as ground
                GroundDetector groundDetector = collision.gameObject.GetComponentInChildren<GroundDetector>();
                if (groundDetector != null)
                {
                    groundDetector.SetBlockGround(true);
                }
                return;
            }
            // Horizontal contact (sides) => NOT ground, acts as wall
        }

        // No vertical contact was found above — every contact point is dominated
        // by its horizontal normal, meaning the player ran head-on into a side
        // face of the block instead of landing on top / under it. Trigger death.
        if (killOnSideCollision)
        {
            // Route the kill through the player so the GameManager handles the
            // game-over flow centrally instead of reloading the scene here.
            PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.Die();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        // Draw the block bounds
        Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.3f);
        Gizmos.DrawCube(transform.position + (Vector3)col.offset, col.size * transform.localScale);
        
        // Draw ground contact zones (top and bottom)
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Vector3 size = col.size * transform.localScale;
        
        // Top zone
        Vector3 topCenter = transform.position + (Vector3)col.offset + Vector3.up * (size.y / 2f);
        Gizmos.DrawWireCube(topCenter, new Vector3(size.x, groundContactThreshold, 0));
        
        // Bottom zone
        Vector3 bottomCenter = transform.position + (Vector3)col.offset + Vector3.down * (size.y / 2f);
        Gizmos.DrawWireCube(bottomCenter, new Vector3(size.x, groundContactThreshold, 0));
    }
}
