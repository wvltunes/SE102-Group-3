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
    [Tooltip("Kill the player the instant they run head-on into the LEFT face of the block")]
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

        // Classify the hit by how the two colliders OVERLAP, not by the contact normal
        // (corner noise) nor by centre position. The block is short (1 unit) and the
        // player is taller and stands on the same ground line, so the player's CENTRE is
        // already above the short block's top even when ramming its left face head-on -
        // which is why a centre-based check wrongly read it as "riding on top" and let
        // the solid collider just shove the player back instead of killing.
        Bounds blockBounds = boxCollider.bounds;
        Bounds playerBounds = collision.collider.bounds;

        // Vertical overlap between the player and the block colliders.
        //   Standing on top / under  -> the player only touches an edge, overlap ~= 0.
        //   Ramming a side face       -> the player's body spans the block, overlap is large.
        float verticalOverlap =
            Mathf.Min(playerBounds.max.y, blockBounds.max.y)
            - Mathf.Max(playerBounds.min.y, blockBounds.min.y);

        // Little or no vertical overlap => the player is resting on the top or bottom
        // face => treat as ground (for energy recovery / grounded checks).
        bool ridingOnBlock = verticalOverlap <= groundContactThreshold;

        GroundDetector groundDetector = collision.gameObject.GetComponentInChildren<GroundDetector>();
        if (groundDetector != null)
        {
            groundDetector.SetBlockGround(ridingOnBlock);
        }

        if (!killOnSideCollision) return;

        // Head-on hit on the LEFT face: the player's body overlaps the block's height
        // (not riding it) AND sits on the block's left half. The block only ever travels
        // leftward, so this is the face the player runs into. Hitting it kills instantly -
        // the GameManager freezes time on death, so there is no pushback once Die() fires.
        bool hitLeftFace = !ridingOnBlock && playerBounds.center.x < blockBounds.center.x;
        if (hitLeftFace)
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

        // Deadly LEFT face (only border that triggers death). Drawn in red.
        if (killOnSideCollision)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.6f);
            Vector3 leftCenter = transform.position + (Vector3)col.offset + Vector3.left * (size.x / 2f);
            Gizmos.DrawWireCube(leftCenter, new Vector3(groundContactThreshold, size.y, 0));
        }
    }
}
