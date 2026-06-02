using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BpmSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;

    [Header("Look-Ahead Settings")]
    // Keep this in sync with LevelSequencer.spawnOffsetX so that beat lines and
    // obstacles spawned on the same beat reach the player at the same time.
    [SerializeField] private int lookAheadOffsetBeats = 3; //Calculate offset based on BPM instead of hardcoding

    private Transform playerTransform;
    PlayerController playerController;
    private Coroutine spawnCoroutine; // Reference to the running loop

    void Start()
    {
        // Auto-find Player
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerTransform = playerController.transform;
        }
        else
        {
            Debug.LogError("[BpmSpawner] Player not found in scene!");
        }

        RestartSpawner();
    }

    public void RestartSpawner()
    {
        // If a loop is already running, kill it first
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        // Start a fresh loop and save it to our reference
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Wait for AudioManager to be initialized
        while (AudioManager.instance == null)
        {
            yield return null;
        }

        while (true)
        {
            // Get BPM from AudioManager instead of hardcoding
            float secondsPerBeat = AudioManager.instance.GetSecondsPerBeat();
            
            // Spawn beat line at player.x + lookAheadOffset
            if (playerTransform != null)
            {
                Vector3 spawnPosition = new Vector3(
                    playerTransform.position.x + playerController.GetRunSpeed() * lookAheadOffsetBeats * secondsPerBeat,
                    prefabToSpawn.transform.position.y,
                    0
                );
                Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            }
            else
            {
                // Fallback to original behavior if player not found
                Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(secondsPerBeat);
        }
    }
}