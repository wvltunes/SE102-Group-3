using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    private PlayerController playerController;
    private bool isGroundedLocally = false;
    private bool isOnBlock = false; // Set by BlockBehaviour when player touches top/bottom of a block
    
    [SerializeField] private float raycastDistance = 2f; // Distance to raycast downward - INCREASED
    [SerializeField] private bool showDebugRay = true; // Show raycast in editor
    
    private int updateCounter = 0; // Track how many times Update runs

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        raycastDistance = 3f; // Force set raycast distance to be large enough
    }
    
    private void Update()
    {
        // Raycast to detect ground instead of trigger collider
        DetectGroundWithRaycast();
    }
    
    private void DetectGroundWithRaycast()
    {
        // Cast raycast ignoring player's own colliders
        Vector3 rayStartPos;
        if (!playerController.isReversedGravity())
        {
            rayStartPos = transform.position - Vector3.up * 0.2f; // Start raycast from slightly below
        }
        else
        {
            rayStartPos = transform.position + Vector3.up * 0.2f; // Start raycast from slightly above
        }
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayStartPos, Vector2.down, raycastDistance);
        
        isGroundedLocally = false;
        
        // Check all hits
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                isGroundedLocally = true;
                break;
            }
        }
        

    }
    
    /// <summary>
    /// Called by BlockBehaviour to set whether the player is standing on a block (top or bottom).
    /// When on a block, the player is considered grounded.
    /// </summary>
    public void SetBlockGround(bool onBlock)
    {
        isOnBlock = onBlock;
    }
    
    public bool IsGrounded()
    {
        // Always recalculate ground detection when called (to fix execution order issue)
        DetectGroundWithRaycast();
        return isGroundedLocally || isOnBlock;
    }
}
