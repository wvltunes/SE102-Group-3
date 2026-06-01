using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    private PlayerController playerController;
    private bool isOnBlock = false; // Set by BlockBehaviour when player touches top/bottom of a block

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
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
        // Use the player's logical lane, not a world-position raycast. A raycast lags
        // behind the lane-change tween (~0.1s), so right after a jump it would still
        // report "grounded" and the beat line crossing that same beat would refund the
        // jump cost. The lane flips instantly on jump, so this can't be fooled by the tween.
        bool onGroundLane = playerController != null && playerController.IsOnGroundLane();
        return onGroundLane || isOnBlock;
    }
}
