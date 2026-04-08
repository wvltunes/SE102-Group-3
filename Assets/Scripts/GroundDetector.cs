using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    private PlayerController playerController;
    private bool isGroundedLocally = false;
    
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
    
    public bool IsGrounded()
    {
        return isGroundedLocally;
    }
}
