using UnityEngine;

public class BeatLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Only recover energy if player is grounded (touching ground/platform/block)
                GroundDetector groundDetector = collision.GetComponentInChildren<GroundDetector>();
                if (groundDetector != null && groundDetector.IsGrounded())
                {
                    playerController.RecoverEnergy();
                }
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Only recover energy if player is grounded (touching ground/platform/block)
                GroundDetector groundDetector = collision.GetComponentInChildren<GroundDetector>();
                if (groundDetector != null && groundDetector.IsGrounded())
                {
                    playerController.RecoverEnergy();
                }
            }
        }
    }
}
