using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
// Runs before default-order scripts (e.g. LevelSequencer) so the per-character run
// speed is applied in Start() before LevelSequencer caches its spawn offset from it -
// otherwise obstacle spawn distance would be computed from the wrong speed and drift
// off the beat for the whole level.
[DefaultExecutionOrder(-50)]
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
    private bool isRunning = true;
    private bool isJumping = false;

    [Header("Character Selection")]
    [Tooltip("Apply the chosen character's sprite and animator from CharacterManager " +
             "when the level starts.")]
    [SerializeField] private bool applyCharacterVisuals = true;
    [Tooltip("Apply the chosen character's max-energy stat. Timing-safe - energy does " +
             "not affect obstacle/music sync.")]
    [SerializeField] private bool applyCharacterEnergy = true;
    [Tooltip("Also vary RUN SPEED per character. OFF by default: the shipping levels " +
             "place obstacles by hand and tune them to a single run speed, so changing " +
             "it makes those obstacles reach the player off the beat. Only enable if a " +
             "level is authored to be speed-independent (e.g. obstacles spawned relative " +
             "to the player by LevelSequencer, where spawn distance scales with speed).")]
    [SerializeField] private bool applyCharacterSpeed = false;
    [Tooltip("Run speed when a character's speed stat is at minimum (0). Only used when " +
             "'Apply Character Speed' is enabled.")]
    [SerializeField] private float minRunSpeed = 5f;
    [Tooltip("Run speed when a character's speed stat is at maximum (1).")]
    [SerializeField] private float maxRunSpeed = 8f;
    [Tooltip("Max energy used when a character's energy stat is at minimum (0). Keep " +
             "the range within the number of energy-bar images on the HUD (the design " +
             "caps energy at 4 units) so a high-energy character's bar never overflows.")]
    [SerializeField] private int minMaxEnergy = 2;
    [Tooltip("Max energy used when a character's energy stat is at maximum (1).")]
    [SerializeField] private int maxMaxEnergy = 4;

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

        // Read the character chosen on the selection screen (saved index in
        // PlayerPrefs, mirrored on CharacterManager) and apply its visuals/stats
        // before energy is initialised so currentEnergy starts from the right max.
        ApplySelectedCharacter();

        currentEnergy = maxEnergy;
    }

    /// <summary>
    /// Applies the selected character to this player: sprite + animator controller,
    /// and (optionally) the per-character speed/energy stats.
    ///
    /// Stats come from the <see cref="CharacterUIData"/> roster entry for the saved
    /// index when one is assigned; otherwise the normalized 0..1 values pushed from
    /// the select screen are used. A negative value means "stat not set", in which
    /// case the inspector defaults are kept. No-op when no <see cref="CharacterManager"/>
    /// exists (e.g. launching a level directly without going through the menu).
    /// </summary>
    private void ApplySelectedCharacter()
    {
        CharacterManager cm = CharacterManager.instance;
        if (cm == null) return;

        // Visuals chosen on the select screen.
        if (applyCharacterVisuals)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null && cm.selectedSprite != null)
                sr.sprite = cm.selectedSprite;

            if (animator != null && cm.selectedAnimator != null)
                animator.runtimeAnimatorController = cm.selectedAnimator;
        }

        // Resolve stats: roster CharacterUIData first, then the runtime 0..1 values.
        // A negative value means "stat not set", so the inspector defaults are kept.
        float speed01 = -1f, energy01 = -1f;
        CharacterUIData data = cm.GetSelectedData();
        if (data != null)
        {
            speed01 = Mathf.Clamp01(data.speed / 100f);
            energy01 = Mathf.Clamp01(data.energy / 100f);
        }
        else
        {
            speed01 = cm.selectedSpeed01;
            energy01 = cm.selectedEnergy01;
        }

        // Speed is opt-in: hand-placed obstacle levels are tuned to one run speed.
        if (applyCharacterSpeed && speed01 >= 0f)
            runSpeed = Mathf.Lerp(minRunSpeed, maxRunSpeed, speed01);

        if (applyCharacterEnergy && energy01 >= 0f)
            maxEnergy = Mathf.Max(1, Mathf.RoundToInt(Mathf.Lerp(minMaxEnergy, maxMaxEnergy, energy01)));
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

#if UNITY_EDITOR
        // Debug (Editor only): Press 'D' to trigger death. Compiled out of builds so a
        // player can never accidentally kill themselves with a stray key press.
        if (Input.GetKeyDown(KeyCode.D))
        {
            Die();
            return;
        }
#endif

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
                    beatsToSkip = 1;
                    if (consumesEnergy) ConsumeEnergy();
                    isJumping = true;
                    animator.SetBool("IsJumping", true);
                    SfxManager.Instance?.PlayJump();
                    StartCoroutine(WaitForJumpAnimation());
                }
            }
            else
            {
                if (currentLane > minLanes)
                {
                    currentLane--;
                    UpdateLanePosition();
                    beatsToSkip = 1;
                    if (consumesEnergy) ConsumeEnergy();
                    isJumping = true;
                    animator.SetBool("IsJumping", true);
                    SfxManager.Instance?.PlayJump();
                    StartCoroutine(WaitForJumpAnimation());
                }
            }

        }

    // Nhảy xuống (S) — không set IsJumping
    if (Input.GetKeyDown(KeyCode.S))
    {
        if (consumesEnergy && currentEnergy <= 0) return;

        if (!reversedGravity)
        {
            if (currentLane > 0)
            {
                currentLane--;
                UpdateLanePosition();
                beatsToSkip = 1;
                if (consumesEnergy) ConsumeEnergy();
            }
        }
        else
        {
            if (currentLane < maxLanes)
            {
                currentLane++;
                UpdateLanePosition();
                beatsToSkip = 1;
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

        // Death stinger. Played from the dedicated SFX source so it is still heard
        // when the GameManager pauses the music a moment later on game-over.
        SfxManager.Instance?.PlayDie();

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