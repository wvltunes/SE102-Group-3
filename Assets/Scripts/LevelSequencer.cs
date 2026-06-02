using UnityEngine;

/// <summary>
/// Spawns obstacles based on LevelData and audio timing.
/// Converts beat timestamps into world positions relative to player.
/// Automatically extends the map/background and places LevelCompleteZone at level end.
/// </summary>
public class LevelSequencer : MonoBehaviour
{
    [SerializeField] private LevelData levelData;

    [Header("Spawn Offset (relative to player)")]
    // Keep this in sync with BpmSpawner.lookAheadOffset so that obstacles and beat
    // lines spawned on the same beat reach the player at the same time.
    [SerializeField] private float spawnOffsetSeconds = 3f; //Calculate offset based on BPM instead of hardcoding
    private float spawnOffsetX;
    
    [Header("Obstacle Prefabs")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private GameObject jumpOrbPrefab;
    [SerializeField] private GameObject gravityOrbPrefab;
    [SerializeField] private GameObject jumpPadPrefab;
    [SerializeField] private GameObject gravityPadPrefab;

    [Header("Level Completion")]
    [SerializeField] private GameObject levelCompleteZonePrefab;
    [Tooltip("Time in seconds after the last beat event to place the completion zone")]
    [SerializeField] private float completionZoneDelay = 2f;

    [Header("Lane Settings")]
    [SerializeField] private float laneHeight = 2f;
    [SerializeField] private float laneBaseY = 0f;

    [Header("Map Extension")]
    [SerializeField] private GameObject backgroundObject;
    [Tooltip("Scale to extend background in X direction")]
    [SerializeField] private float backgroundExtensionScale = 2f;

    private int eventIndex = 0;
    private PlayerController playerController;
    private float levelStartTime;
    private bool levelStarted = false;
    private bool completionZonePlaced = false;
    private float levelEndTime = 0f;
    private float levelBPM = 120f; // Default BPM if not set in LevelData
    private float levelSecondsPerEvent = 0.5f;

    void Start()
    {
        // Auto-find player
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("[LevelSequencer] Player not found!");
            return;
        }
        else
        {
            spawnOffsetX = playerController.GetRunSpeed() * spawnOffsetSeconds;
        }

        // Mark level start
        levelStartTime = Time.time;
        levelStarted = true;

        if (levelData == null)
        {
            Debug.LogWarning("[LevelSequencer] Level Data not assigned!");
            return;
        }
        else
        {
            levelData = Instantiate(levelData); // Create instance to avoid modifying original asset
            levelData.SortBeatEvent();
        }
        levelBPM = levelData.GetBPM();
        levelSecondsPerEvent = 60f / levelBPM;

        Debug.Log($"[LevelSequencer] Level started with {levelData.GetEventCount()} beat events");

        // Calculate level end time (last event + completion delay)
        if (levelData.GetEventCount() > 0)
        {
            BeatEvent lastEvent = levelData.GetEventAt(levelData.GetEventCount() - 1);
            float lastEventTime = lastEvent.beatIndex * levelSecondsPerEvent;
            if (lastEvent != null)
            {
                levelEndTime = lastEventTime + completionZoneDelay;
                Debug.Log($"[LevelSequencer] Level end time set to: {levelEndTime}s (last event at {lastEventTime}s + {completionZoneDelay}s delay)");
            }
        }

        // Extend background/map if reference is provided
        if (backgroundObject != null)
        {
            ExtendBackground();
        }
    }

    void Update()
    {
        if (!levelStarted || levelData == null)
            return;

        // Get elapsed time since level start
        float elapsedTime = Time.time - levelStartTime;

        // Spawn obstacles while there are events remaining
        if (eventIndex < levelData.GetEventCount())
        {
            BeatEvent nextEvent = levelData.GetEventAt(eventIndex);
            if (nextEvent == null)
                return;

            // Check if it's time to spawn
            if (elapsedTime >= nextEvent.beatIndex * levelSecondsPerEvent)
            {
                SpawnObstacle(nextEvent);
                eventIndex++;
            }
        }

        // Place completion zone when level end time is reached
        if (!completionZonePlaced && levelEndTime > 0f && elapsedTime >= levelEndTime)
        {
            PlaceLevelCompleteZone();
        }
    }

    private void SpawnObstacle(BeatEvent beatEvent)
    {
        // Spawn relative to player's current position
        float playerX = playerController.transform.position.x;
        float worldX = playerX + spawnOffsetX;
        
        // Calculate lane Y position
        float worldY = laneBaseY + (beatEvent.lane * laneHeight);
        
        Vector3 spawnPosition = new Vector3(worldX, worldY, 0);

        // Get correct prefab based on type
        GameObject prefab = GetObstaclePrefab(beatEvent.type);
        if (prefab == null)
        {
            Debug.LogWarning($"[LevelSequencer] No prefab found for obstacle type: {beatEvent.type}");
            return;
        }

        // Spawn obstacle
        GameObject obstacle = Instantiate(prefab, spawnPosition, Quaternion.identity);

        if (beatEvent.type == ObstacleType.JumpOrb)
        {
            // Set laneToJump for orbs
            JumpOrbVariables orbController = obstacle.GetComponent<JumpOrbVariables>();
            if (orbController != null)
            {
                orbController.setLaneMultiplier(beatEvent.laneToJump);
            }
        }

        if (beatEvent.type == ObstacleType.JumpPad)
        {
            // Set laneToJump for jump pads
            JumpPadVariables padController = obstacle.GetComponent<JumpPadVariables>();
            if (padController != null)
            {
                padController.setLaneMultiplier(beatEvent.laneToJump);
            }
        }
        Debug.Log($"[LevelSequencer] Spawned {beatEvent.type} at x={worldX} (offset +{spawnOffsetX} from player), lane={beatEvent.lane}");
    }

    private GameObject GetObstaclePrefab(ObstacleType type)
    {
        return type switch
        {
            ObstacleType.Block => blockPrefab,
            ObstacleType.Enemy => enemyPrefab,
            ObstacleType.Spike => spikePrefab,
            ObstacleType.JumpOrb => jumpOrbPrefab,
            ObstacleType.GravityOrb => gravityOrbPrefab,
            ObstacleType.JumpPad => jumpPadPrefab,
            ObstacleType.GravityPad => gravityPadPrefab,
            _ => null
        };
    }

    /// <summary>
    /// Public method to reset sequencer (for level restart)
    /// </summary>
    public void ResetSequencer()
    {
        eventIndex = 0;
        levelStartTime = Time.time;
    }

    /// <summary>
    /// Get current spawn progress
    /// </summary>
    public float GetProgress()
    {
        if (levelData == null)
            return 0f;
        return (float)eventIndex / levelData.GetEventCount();
    }

    /// <summary>
    /// Extends the background object to cover the full level length
    /// </summary>
    private void ExtendBackground()
    {
        if (backgroundObject == null)
            return;

        RectTransform rectTransform = backgroundObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Scale background horizontally
            Vector3 newScale = rectTransform.localScale;
            newScale.x *= backgroundExtensionScale;
            rectTransform.localScale = newScale;
            Debug.Log($"[LevelSequencer] Extended background X scale by {backgroundExtensionScale}x");
        }
        else
        {
            // Try SpriteRenderer scale instead
            SpriteRenderer spriteRenderer = backgroundObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Vector3 newScale = backgroundObject.transform.localScale;
                newScale.x *= backgroundExtensionScale;
                backgroundObject.transform.localScale = newScale;
                Debug.Log($"[LevelSequencer] Extended background sprite X scale by {backgroundExtensionScale}x");
            }
        }
    }

    /// <summary>
    /// Places the LevelCompleteZone at the end of the level
    /// </summary>
    private void PlaceLevelCompleteZone()
    {
        if (completionZonePlaced)
            return;

        completionZonePlaced = true;

        // Calculate position based on level end time
        float worldX = playerController.transform.position.x + spawnOffsetX;
        float worldY = laneBaseY; // Place at base lane
        Vector3 spawnPosition = new Vector3(worldX, worldY, 0);

        // Instantiate completion zone prefab
        if (levelCompleteZonePrefab != null)
        {
            GameObject completionZone = Instantiate(levelCompleteZonePrefab, spawnPosition, Quaternion.identity);

            // Get LevelCompleteZone component and set position
            LevelCompleteZone zoneScript = completionZone.GetComponent<LevelCompleteZone>();
            if (zoneScript != null)
            {
                zoneScript.SetZonePosition(spawnPosition);
                zoneScript.SetColliderSize(2f, 8f);
            }

            Debug.Log($"[LevelSequencer] Placed LevelCompleteZone at position ({worldX}, {worldY}, 0) at time {levelEndTime}s");
        }
        else
        {
            // Fallback: create completion zone from scratch if no prefab is assigned
            CreateLevelCompleteZone(spawnPosition);
        }
    }

    /// <summary>
    /// Creates a LevelCompleteZone from scratch if no prefab is provided
    /// </summary>
    private void CreateLevelCompleteZone(Vector3 position)
    {
        GameObject completionZone = new GameObject("LevelCompleteZone");
        completionZone.transform.position = position;

        // Add BoxCollider2D as trigger
        BoxCollider2D collider = completionZone.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(2f, 8f); // Wide vertical trigger to catch player

        // Add LevelCompleteZone script
        LevelCompleteZone completeScript = completionZone.AddComponent<LevelCompleteZone>();

        // Set position and collider via script methods
        completeScript.SetZonePosition(position);
        completeScript.SetColliderSize(2f, 8f);

        Debug.Log($"[LevelSequencer] Created LevelCompleteZone at {position}");
    }
}
