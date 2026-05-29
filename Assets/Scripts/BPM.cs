using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BpmSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;

    private Coroutine spawnCoroutine; // Reference to the running loop

    void Start()
    {
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
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(secondsPerBeat);
        }
    }
}