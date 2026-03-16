using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f; // Force applied for jumping
    [SerializeField] private Transform groundCheck; // Position to check if the player is grounded
    [SerializeField] private LayerMask groundPlayer; // Layer mask to identify what is considered ground
    private bool isGrounded;
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Animator animator; // Reference to the Animator component

    void Start()
    {

    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the player
        animator = GetComponent<Animator>(); // Get the Animator component attached to the player
    }
    // Update is called once per frame
    void Update()
    {
        HandleJump();
        UpdateAnimation();
    }
    

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Apply jump force to the Rigidbody2D 
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundPlayer); // Check if the player is grounded by checking for overlap with the ground layer
    }

    private void UpdateAnimation()
    {
        bool IsRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f; // Check if the player is running based on horizontal velocity
        animator.SetBool("IsJumping", !isGrounded); // Set the "isJumping" parameter in the Animator based on whether the player is grounded
    }
}
