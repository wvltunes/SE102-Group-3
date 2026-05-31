using UnityEngine;

/// <summary>
/// Spawns obstacles based on LevelData and audio timing.
/// Converts beat timestamps into world positions relative to player.
/// </summary>
public class LevelSequencer : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    
    [Header("Spawn Offset (relative to player)")]
    [SerializeField] private float spawnOffsetX = 8f;
    
    [Header("Obstacle Prefabs")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private GameObject jumpOrbPrefab;
    [SerializeField] private GameObject gravityOrbPrefab;
    [SerializeField] private GameObject jumpPadPrefab;
    [SerializeField] private GameObject gravityPadPrefab;

    [Header("Lane Settings")]
    [SerializeField] private float laneHeight = 2f;
    [SerializeField] private float laneBaseY = 0f;

    private int eventIndex = 0;
    private PlayerController playerController;
    private float levelStartTime;
    private bool levelStarted = false;

    void Start()
    {
        // Auto-find player
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("[LevelSequencer] Player not found!");
            return;
        }

        // Mark level start
        levelStartTime = Time.time;
        levelStarted = true;

        if (levelData == null)
        {
            Debug.LogWarning("[LevelSequencer] Level Data not assigned!");
            return;
        }

        Debug.Log($"[LevelSequencer] Level started with {levelData.GetEventCount()} beat events");
    }

    void Update()
    {
        if (!levelStarted || levelData == null || eventIndex >= levelData.GetEventCount())
            return;

        // Get elapsed time since level start
        float elapsedTime = Time.time - levelStartTime;

        // Get next beat event
        BeatEvent nextEvent = levelData.GetEventAt(eventIndex);
        if (nextEvent == null)
            return;

        // Check if it's time to spawn
        if (elapsedTime >= nextEvent.timestamp)
        {
            SpawnObstacle(nextEvent);
            eventIndex++;
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
}
