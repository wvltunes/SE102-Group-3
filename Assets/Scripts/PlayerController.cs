using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Raised once, the moment the player dies. The GameManager (and any other
    /// interested systems) subscribe to this so the PlayerController never needs
    /// to know who reacts to a death. Static so listeners can subscribe without
    /// holding a reference to the (per-scene) player instance.
    /// </summary>
    public static event System.Action OnPlayerDeath;

    // Guards against the death event being raised more than once
    // (e.g. overlapping hazards in the same frame).
    private bool isDead = false;

    [SerializeField] private float laneHeight = 2f;
    [SerializeField] private int maxLanes = 3;
    [SerializeField] private int minLanes = 0;

    [Header("DOTween Settings")]
    [SerializeField] private float transitionDuration = 0.1f;
    [SerializeField] private DG.Tweening.Ease laneChangeEase = DG.Tweening.Ease.OutQuad;

    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 6f;
    private bool isRunning = true;

    private Rigidbody2D rb;
    private Animator animator;
    private int currentLane = 0;
    private Vector3 startPosition;
    private bool reversedGravity = false;

    // Beat bypass: after jumping, skip 1 beat before falling
    private int beatsToSkip = 0;

    private int currentEnergy = 0;
    private int maxEnergy = 4;
    private float energyRecoveryTimer = 0f;
    private BpmSpawner bpmSpawner;
    private GroundDetector groundDetector;

    void Start()
    {
        startPosition = transform.position;
        currentEnergy = maxEnergy;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0;
        bpmSpawner = FindObjectOfType<BpmSpawner>();
        groundDetector = GetComponentInChildren<GroundDetector>();
    }

    void Update()
    {
        // A dead player no longer reacts to input or recovers energy.
        if (isDead) return;

        // Constant forward movement
        if (isRunning)
        {
            rb.linearVelocity = new Vector2(runSpeed, rb.linearVelocity.y);
        }

        HandleEnergyRecovery();
        HandleJump();
        UpdateAnimation();
    }

    private void HandleEnergyRecovery()
    {
        if (groundDetector != null && groundDetector.IsGrounded())
        {
            float secondsPerBeat = AudioManager.instance != null ? AudioManager.instance.GetSecondsPerBeat() : 0.5f;

            energyRecoveryTimer += Time.deltaTime;
            if (energyRecoveryTimer >= secondsPerBeat)
            {
                RecoverEnergy();
                energyRecoveryTimer = 0f;
            }
        }
        else
        {
            energyRecoveryTimer = 0f;
        }
    }


    private void HandleJump(bool consumesEnergy = true)
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (consumesEnergy && currentEnergy <= 0) return;

            if (!reversedGravity)
            {
                if (currentLane < maxLanes)
                {
                    currentLane++;
                    UpdateLanePosition();
                    beatsToSkip = 1; // Bypass the next beat after jumping
                    if (consumesEnergy) ConsumeEnergy();
                }
            }
            else
            {
                if (currentLane > minLanes)
                {
                    currentLane--;
                    UpdateLanePosition();
                    beatsToSkip = 1; // Bypass the next beat after jumping
                    if (consumesEnergy) ConsumeEnergy();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (consumesEnergy && currentEnergy <= 0) return;

            if (!reversedGravity)
            {
                if (currentLane > 0)
                {
                    currentLane--;
                    UpdateLanePosition();
                    beatsToSkip = 1; // Bypass the next beat after jumping down
                    if (consumesEnergy) ConsumeEnergy();
                }
            }
            else
            {
                if (currentLane < maxLanes)
                {
                    currentLane++;
                    UpdateLanePosition();
                    beatsToSkip = 1; // Bypass the next beat after jumping
                    if (consumesEnergy) ConsumeEnergy();
                }
            }
        }
    }

    public void JumpPlayer(int lane)
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
        beatsToSkip = 1; // Bypass the next beat after pad/orb jump
        ConsumeEnergy();
    }

    public void JumpPlayerToGround()
    {
        currentLane = !reversedGravity ? minLanes : maxLanes;
        UpdateLanePosition();
    }

    private void UpdateLanePosition()
    {
        float targetY = startPosition.y + (currentLane * laneHeight);

        transform.DOKill();

        transform.DOMoveY(targetY, transitionDuration)
                 .SetEase(laneChangeEase);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
    }

    
    private void OnDestroy()
    {
        transform.DOKill();
    }

    /// <summary>
    /// Kills the player and raises <see cref="OnPlayerDeath"/>.
    /// Safe to call multiple times - the event is only raised once.
    /// Hazards (spikes, enemies, blocks, ...) call this on contact instead of
    /// reloading the scene themselves, so the GameManager owns the game-over flow.
    /// </summary>
    public void Die()
    {
        if (isDead) return; // Already dead - ignore any further hits.
        isDead = true;

        // Freeze the player in place by cancelling any in-progress lane tween.
        transform.DOKill();

        // Kill any momentum so a fatal hit (e.g. running into a block face) can never
        // keep shoving the body around in the frame before time is frozen.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (OnPlayerDeath != null)
        {
            // A GameManager (or other system) is listening - let it drive the
            // game-over state, pausing, UI and scene reload.
            OnPlayerDeath.Invoke();
        }
        else
        {
            // Fallback for scenes that don't yet have a GameManager: reload the
            // current scene so death still behaves like before this system existed.
            Debug.LogWarning(
                "[PlayerController] Player died but no OnPlayerDeath listener was found. " +
                "Reloading the current scene as a fallback. Add a GameManager to handle this properly.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void UpdateAnimation()
    {
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsJumping", false);
    }

    /// <summary>
    /// Called by LaneReducerTriggerMovement (beat lines).
    /// After a jump, the first beat is bypassed so the player doesn't
    /// fall immediately. The 2nd beat onward triggers normal falling.
    /// </summary>
    public bool TryReduceLane()
    {
        if (beatsToSkip > 0)
        {
            beatsToSkip--;
            return false; // Beat bypassed, no lane reduction
        }
        
        ReduceLane();
        return true;
    }
    
    public void ReduceLane()
    {
        if (!reversedGravity)
        {
            if (currentLane > 0) currentLane--;
        }
        else
        {
            if (currentLane < maxLanes) currentLane++;
        }
        UpdateLanePosition();
    }

    public void ToggleReverseGravity()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>(); 
        this.reversedGravity = !this.reversedGravity;
        if (sr != null) sr.flipY = reversedGravity;
    }

    public void RecoverEnergy() { if (currentEnergy < maxEnergy) currentEnergy++; }
    public void ConsumeEnergy() { if (currentEnergy > 0) currentEnergy--; }
    public int GetCurrentEnergy() => currentEnergy;
    public int GetMaxEnergy() => maxEnergy;
    public bool isReversedGravity() => reversedGravity;

    public void SetRunning(bool running)
    {
        isRunning = running;
    }
}