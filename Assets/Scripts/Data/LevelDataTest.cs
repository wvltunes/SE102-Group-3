using UnityEngine;

/// <summary>
/// Simple test script to verify LevelData loads correctly
/// Attach to any GameObject and set the Level Data reference in Inspector
/// </summary>
public class LevelDataTest : MonoBehaviour
{
    [SerializeField] private LevelData levelData;

    void Start()
    {
        if (levelData == null)
        {
            Debug.LogError("[LevelDataTest] Level Data not assigned!");
            return;
        }

        Debug.Log($"=== Level Data Test ===");
        Debug.Log($"Level Name: {levelData.GetLevelName()}");
        Debug.Log($"BPM: {levelData.GetBPM()}");
        Debug.Log($"Total Beat Events: {levelData.GetEventCount()}");

        // Log all beat events
        foreach (BeatEvent e in levelData.GetBeatEvents())
        {
            Debug.Log($"  Event: timestamp={e.timestamp}s, type={e.type}, lane={e.lane}");
        }

        Debug.Log($"=== Test Complete ===");
    }
}
