using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a single beat event in a level
/// Contains timing, obstacle type, and which lane to spawn in
/// </summary>
[System.Serializable]
public class BeatEvent
{
    [Tooltip("Time in seconds (relative to music start) when this obstacle should appear")]
    public float timestamp;

    [Tooltip("Type of obstacle to spawn")]
    public ObstacleType type;

    [Tooltip("Lane to spawn in (0-3)")]
    [Range(0, 3)]
    public int lane;

    [Tooltip("Lane to jump over (only used for Jump obstacles)")]
    [Range(1, 3)]
    public int laneToJump;

    public BeatEvent(float timestamp, ObstacleType type, int lane, int laneToJump = 1)
    {
        this.timestamp = timestamp;
        this.type = type;
        this.lane = lane;
        this.laneToJump = laneToJump;
    }
}

/// <summary>
/// ScriptableObject that stores all beat events for a level
/// Can be created/edited in the Unity Inspector
/// </summary>
[CreateAssetMenu(fileName = "New Level Data", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    [Tooltip("Name of the level")]
    [SerializeField] private string levelName = "Level 1";

    [Tooltip("BPM of the level (for reference)")]
    [SerializeField] private float bpm = 160f;

    [Tooltip("List of beat events in this level")]
    [SerializeField] private List<BeatEvent> beatEvents = new List<BeatEvent>();

    public string GetLevelName() => levelName;
    public float GetBPM() => bpm;
    public List<BeatEvent> GetBeatEvents() => beatEvents;

    /// <summary>
    /// Get all beat events that should spawn at or after a specific time
    /// </summary>
    public List<BeatEvent> GetEventsAfter(float timestamp)
    {
        return beatEvents.FindAll(e => e.timestamp >= timestamp);
    }

    /// <summary>
    /// Get the next beat event after a specific time
    /// </summary>
    public BeatEvent GetNextEvent(float timestamp)
    {
        foreach (BeatEvent e in beatEvents)
        {
            if (e.timestamp > timestamp)
            {
                return e;
            }
        }
        return null;
    }

    /// <summary>
    /// Get beat event at specific index
    /// </summary>
    public BeatEvent GetEventAt(int index)
    {
        if (index >= 0 && index < beatEvents.Count)
        {
            return beatEvents[index];
        }
        return null;
    }

    public int GetEventCount() => beatEvents.Count;

#if UNITY_EDITOR
    /// <summary>
    /// Editor-only helper to add beat events
    /// </summary>
    public void AddBeatEvent(float timestamp, ObstacleType type, int lane)
    {
        beatEvents.Add(new BeatEvent(timestamp, type, lane));
        // Sort by timestamp for easier editing
        beatEvents.Sort((a, b) => a.timestamp.CompareTo(b.timestamp));
    }

    /// <summary>
    /// Editor-only helper to remove beat event at index
    /// </summary>
    public void RemoveBeatEvent(int index)
    {
        if (index >= 0 && index < beatEvents.Count)
        {
            beatEvents.RemoveAt(index);
        }
    }

    /// <summary>
    /// Editor-only helper to clear all events
    /// </summary>
    public void ClearBeatEvents()
    {
        beatEvents.Clear();
    }
#endif
}
