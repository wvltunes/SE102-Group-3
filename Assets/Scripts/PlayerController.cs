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

    [Tooltip("Falling back down to the ground is quicker and accelerates (gravity-like) so it doesn't feel floaty.")]
    [SerializeField] private float fallDuration = 0.02f;
    [SerializeField] private DG.Tweening.Ease fallEase = DG.Tweening.Ease.InQuad;

    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private int beatsToSkipAfterJump = 2; // Number of beats to skip after a jump before allowing lane reduction
    private bool isRunning = true;
    private bool isJumping = false;

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

        // Debug: Press 'D' to trigger death
        if (Input.GetKeyDown(KeyCode.D))
        {
            Die();
            return;
        }

        // Constant forward movement
        if (isRunning)
        {
            rb.linearVelocity = new Vector2(runSpeed, rb.linearVelocity.y);
        }

        HandleEnergyRecovery();
        HandleJump();
        UpdateAnimation();
    }

    /// <summary>
    /// Recovers 1 energy every beat, whether the player is on the ground lane or up on the
    /// beat lines. This lives here (not on the beat-line objects) because the BeatLine script
    /// only exists in a couple of scenes - driving it from a per-beat timer on the player
    /// guarantees a single, steady source of recovery in every level so jumping stays
    /// sustainable. The recovery is on a beat clock rather than tied to the jump frame, so a
    /// jump's -1 still shows as a dip until the next beat tick.
    /// </summary>
    private void HandleEnergyRecovery()
    {
        float secondsPerBeat = AudioManager.instance != null ? AudioManager.instance.GetSecondsPerBeat() : 0.5f;

        energyRecoveryTimer += Time.deltaTime;
        if (energyRecoveryTimer >= secondsPerBeat)
        {
            // Only recover energy if the player is grounded (on ground lane or touching a block)
            if (groundDetector != null && groundDetector.IsGrounded())
            {
                RecoverEnergy();
            }
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
                    beatsToSkip = beatsToSkipAfterJump;
                    if (consumesEnergy) ConsumeEnergy();
                    isJumping = true;
                    animator.SetBool("IsJumping", true);
                    StartCoroutine(WaitForJumpAnimation()); 
                }
            }
            else
            {
                if (currentLane > minLanes)
                {
                    currentLane--;
                    UpdateLanePosition();
                    beatsToSkip = beatsToSkipAfterJump;
                    if (consumesEnergy) ConsumeEnergy();
                    isJumping = true;
                    animator.SetBool("IsJumping", true);
                    StartCoroutine(WaitForJumpAnimation()); 
                }
            }

        }

    // Nhảy xuống (S) — không set IsJumping
    if (Input.GetKeyDown(KeyCode.S))
    {
        if (!reversedGravity)
        {
            if (currentLane > 0)
            {
                currentLane--;
                UpdateLanePosition();
            }
        }
        else
        {
            if (currentLane < maxLanes)
            {
                currentLane++;
                UpdateLanePosition();
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
        beatsToSkip = beatsToSkipAfterJump; // Bypass the next beat after pad/orb jump
        ConsumeEnergy();
        animator.SetBool("IsPadding", true);
        StartCoroutine(ResetPaddingFlag());
    }
    private System.Collections.IEnumerator ResetPaddingFlag()
    {
        // Tự lấy độ dài của clip đang play trên layer 0
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        float clipLength = clips.Length > 0 ? clips[0].clip.length : 0.3f;

        yield return new WaitForSeconds(clipLength);
        animator.SetBool("IsPadding", false);
    }
    public void JumpPlayerToGround()
    {
        currentLane = !reversedGravity ? minLanes : maxLanes;
        UpdateLanePosition(fallDuration, fallEase);
    }

    // Normal lane change (jumping up): uses the standard transition feel.
    private void UpdateLanePosition() => UpdateLanePosition(transitionDuration, laneChangeEase);

    private void UpdateLanePosition(float duration, DG.Tweening.Ease ease)
    {
        float targetY = startPosition.y + (currentLane * laneHeight);

        transform.DOKill();

        transform.DOMoveY(targetY, duration)
                 .SetEase(ease); 

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
    }

    private System.Collections.IEnumerator WaitForJumpAnimation()
    {
        yield return null; 
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        float clipLength = clips.Length > 0 ? clips[0].clip.length : 0.3f;

        yield return new WaitForSeconds(clipLength);

        isJumping = false;
        animator.SetBool("IsJumping", false);
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
        if (isDead) return;
        isDead = true;

        transform.DOKill();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        animator.SetBool("IsDying", true);

        StartCoroutine(DieSequence());
    }

    private System.Collections.IEnumerator DieSequence()
    {
        // Chờ animation IsDying chạy xong
        yield return null; // chờ 1 frame để Animator cập nhật state mới
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        float clipLength = clips.Length > 0 ? clips[0].clip.length : 0.5f;

        yield return new WaitForSeconds(clipLength);

        if (OnPlayerDeath != null)
            OnPlayerDeath.Invoke();
        else
        {
            Debug.LogWarning("[PlayerController] No OnPlayerDeath listener. Reloading scene.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void UpdateAnimation()
    {
        animator.SetBool("IsRunning", isRunning);
        // Bỏ phần check IsOnGroundLane() ở đây, đã xử lý trong OnComplete
        animator.SetBool("IsJumping", isJumping);
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
        UpdateLanePosition(fallDuration, fallEase);
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
    public float GetRunSpeed() => runSpeed; 

    /// <summary>
    /// True when the player is logically resting on the ground lane (lane 0 normally,
    /// or the top lane under reversed gravity). This flips the instant a jump starts,
    /// unlike a world-position raycast which lags behind the lane-change tween. Used to
    /// gate energy recovery so the beat line on the jump beat can't refund the jump cost.
    /// </summary>
    public bool IsOnGroundLane() => !reversedGravity ? currentLane == minLanes : currentLane == maxLanes;

    public void SetRunning(bool running)
    {
        isRunning = running;
    }
}