using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    private PlayerController playerController;
    private bool isGroundedLocally = false;
    private bool isOnBlock = false; // Set by BlockBehaviour when player touches top/bottom of a block
    
    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGroundedLocally = true;
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGroundedLocally = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGroundedLocally = false;
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
        return isGroundedLocally || isOnBlock;
    }
}
