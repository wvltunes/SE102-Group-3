using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;

public class JumpOrbSpawner : MonoBehaviour
{

    public GameObject orbToSpawnPrefab;
    public bool randomSpawn; //not true random, use for testing
    public float bpm = 120;
    public Vector2 spawnPoint;
    private Coroutine spawnCoroutine;
    public int orbLaneMultiplierToSpawn;
    private Vector2 lane1Spawn = new Vector2 (7, -2f);
    private Vector2 lane2Spawn = new Vector2(7, 0f);
    private Vector2 lane3Spawn = new Vector2(7, 2f);
    private Vector2 lane4Spawn = new Vector2(7, 4f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (randomSpawn || spawnPoint == null)
        {
            RestartSpawner();
        }
        else
        {
            Instantiate(orbToSpawnPrefab, spawnPoint, Quaternion.identity);
        }
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
            float secondsPerPad = 240f / bpm;
            int laneToSpawn = Random.Range(1, 4);
            GameObject newPad;
            switch (laneToSpawn)
            {
                case 1:
                    newPad = Instantiate(orbToSpawnPrefab, lane1Spawn, Quaternion.identity);
                    break;
                case 2:
                    newPad = Instantiate(orbToSpawnPrefab, lane2Spawn, Quaternion.identity);
                    break;
                case 3:
                    newPad = Instantiate(orbToSpawnPrefab, lane3Spawn, Quaternion.identity);
                    break;
                case 4:
                    newPad = Instantiate(orbToSpawnPrefab, lane4Spawn, Quaternion.identity);
                    break;
                default:
                    newPad = Instantiate(orbToSpawnPrefab, lane1Spawn, Quaternion.identity);
                    break;
            }
            JumpOrbVariables padVariables = newPad.GetComponent<JumpOrbVariables>();
            padVariables.setLaneMultiplier (Random.Range(1, 4));
            
            yield return new WaitForSeconds(secondsPerPad);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
