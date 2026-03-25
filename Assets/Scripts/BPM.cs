using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BpmSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float bpm = 120.0f;

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
        while (true)
        {
            float secondsPerBeat = 60.0f / bpm;
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(secondsPerBeat);
        }
    }
}