using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float laneHeight = 2f; // Height between each lane
    [SerializeField] private int maxLanes = 3; // Maximum number of lanes
    [SerializeField] private int minLanes = 0;
    
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Animator animator; // Reference to the Animator component
    private int currentLane = 0; // Current lane index (0 = bottom lane)
    private Vector3 startPosition; // Starting position of the player
    private bool reversedGravity = false;
    
    // Energy system
    private int currentEnergy = 0;
    private int maxEnergy = 4;
    private float energyRecoveryTimer = 0f;
    private BpmSpawner bpmSpawner; // Reference to BPM spawner

    void Start()
    {
        startPosition = transform.position;
        currentEnergy = maxEnergy;
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the player
        animator = GetComponent<Animator>(); // Get the Animator component attached to the player
        rb.gravityScale = 0; // Disable gravity so player doesn't fall
        bpmSpawner = FindObjectOfType<BpmSpawner>(); // Find BPM spawner in the scene
    }
    // Update is called once per frame
    void Update()
    {
        // Handle energy recovery based on BPM
        HandleEnergyRecovery();
        
        HandleJump();
        UpdateAnimation();
    }
    
    private void HandleEnergyRecovery()
    {
        // Calculate seconds per beat based on BPM (no ground condition needed)
        float secondsPerBeat = bpmSpawner != null ? (60.0f / bpmSpawner.bpm) : 0.5f;
        
        energyRecoveryTimer += Time.deltaTime;
        if (energyRecoveryTimer >= secondsPerBeat)
        {
            RecoverEnergy();
            energyRecoveryTimer = 0f;
        }
    }
    

    private void HandleJump(bool consumesEnergy = true)
    {
        if (Input.GetButtonDown("Jump"))
        {
            // Check if has energy first
            if (consumesEnergy)
            {
                if (currentEnergy <= 0)
                {
                    return; // No energy, can't jump
                }
            }
            
            // Move to the next lane (up)
            if (!reversedGravity)
            {
                if (currentLane < maxLanes - 1)
                {
                    currentLane++;
                    UpdateLanePosition();
                    if (consumesEnergy)
                        ConsumeEnergy();
                }
            }
            else
            {
                if (currentLane > minLanes)
                {
                    currentLane--;
                    UpdateLanePosition();
                    if (consumesEnergy)
                        ConsumeEnergy();
                }
            }
            
        }

        // Check for lane switch input (optional: down arrow or S key to move down)
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Check if has energy first
            if (consumesEnergy)
            {
                if (currentEnergy <= 0)
                {
                    return; // No energy, can't move down
                }
            }
            
            if (!reversedGravity)
            {
                if (currentLane > 0)
                {
                    currentLane--;
                    UpdateLanePosition();
                    if (consumesEnergy)
                        ConsumeEnergy();
                }
            }
            else
            {
                if (currentLane < maxLanes)
                {
                    currentLane++;
                    UpdateLanePosition();
                    if (consumesEnergy)
                        ConsumeEnergy();
                }
            }
        }
        
        
    }

    public void JumpPlayer(int  lane) //used by pads and orbs
    {        
        if (!reversedGravity)
        { 
            currentLane = (currentLane + lane) >= maxLanes ? maxLanes : currentLane + lane; 
        }
        else
        {
            currentLane = (currentLane - lane) <= minLanes ? minLanes : currentLane - lane;
        }
        UpdateLanePosition();
        ConsumeEnergy();
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
        if (!reversedGravity)
        {
            if (currentLane > 0)
            {
                currentLane--;
            }
        }
        else
        {
            if (currentLane < maxLanes - 1)
            {
                currentLane++;
            }
        }
        UpdateLanePosition();
    }
    public void ToggleReverseGravity()
    {
        SpriteRenderer rb = GetComponent<SpriteRenderer>();
        this.reversedGravity = !this.reversedGravity;
        rb.flipY = reversedGravity;
    }
    
    // Energy system methods
    public void RecoverEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy++;
        }
    }
    
    public void ConsumeEnergy()
    {
        if (currentEnergy > 0)
        {
            currentEnergy--;
        }
    }
    
    public int GetCurrentEnergy()
    {
        return currentEnergy;
    }
    
    public int GetMaxEnergy()
    {
        return maxEnergy;
    }
    

}
