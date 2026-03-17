using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float laneHeight = 2f; // Height between each lane
    [SerializeField] private int maxLanes = 3; // Maximum number of lanes
    
    private bool isGrounded;
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Animator animator; // Reference to the Animator component
    private int currentLane = 0; // Current lane index (0 = bottom lane)
    private Vector3 startPosition; // Starting position of the player

    void Start()
    {
        startPosition = transform.position;
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the player
        animator = GetComponent<Animator>(); // Get the Animator component attached to the player
        rb.gravityScale = 0; // Disable gravity so player doesn't fall
    }
    // Update is called once per frame
    void Update()
    {
        HandleJump();
        UpdateAnimation();
    }
    

    private void HandleJump()
    {
        isGrounded = true; // Always consider player grounded since there's no gravity
        
        if (Input.GetButtonDown("Jump"))
        {
            // Move to the next lane (up)
            if (currentLane < maxLanes - 1)
            {
                currentLane++;
                UpdateLanePosition();
            }
        }
        
        // Check for lane switch input (optional: down arrow or S key to move down)
        if (Input.GetKeyDown(KeyCode.S) && currentLane > 0)
        {
            currentLane--;
            UpdateLanePosition();
        }
    }
    
    private void UpdateLanePosition()
    {
        // Update player Y position based on current lane
        Vector3 newPosition = transform.position;
        newPosition.y = startPosition.y + (currentLane * laneHeight);
        transform.position = newPosition;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Stop vertical movement
    }

    private void UpdateAnimation()
    {
        bool IsRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f; // Check if the player is running based on horizontal velocity
        animator.SetBool("IsJumping", false); // Player is always grounded in lane system
    }

    public void ReduceLane()
    {
        // Lower the player by one lane
        if (currentLane > 0)
        {
            currentLane--;
            UpdateLanePosition();
        }
    }
}
