using UnityEngine;
using DG.Tweening; 
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float laneHeight = 2f;
    [SerializeField] private int maxLanes = 3;
    [SerializeField] private int minLanes = 0;

    [Header("DOTween Settings")]
    [SerializeField] private float transitionDuration = 0.1f;
    [SerializeField] private DG.Tweening.Ease laneChangeEase = DG.Tweening.Ease.OutQuad;

    private Rigidbody2D rb;
    private Animator animator;
    private int currentLane = 0;
    private Vector3 startPosition;
    private bool reversedGravity = false;

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
        HandleEnergyRecovery();
        HandleJump();
        UpdateAnimation();
    }

    private void HandleEnergyRecovery()
    {
        if (groundDetector != null && groundDetector.IsGrounded())
        {
            float secondsPerBeat = bpmSpawner != null ? (60.0f / bpmSpawner.bpm) : 0.5f;

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
                    if (consumesEnergy) ConsumeEnergy();
                }
            }
            else
            {
                if (currentLane > minLanes)
                {
                    currentLane--;
                    UpdateLanePosition();
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
                    if (consumesEnergy) ConsumeEnergy();
                }
            }
            else
            {
                if (currentLane < maxLanes)
                {
                    currentLane++;
                    UpdateLanePosition();
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

    private void UpdateAnimation()
    {
        bool IsRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool("IsJumping", false);
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
}