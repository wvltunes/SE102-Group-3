using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;

public class TestOrbSpawner : MonoBehaviour
{

    public GameObject jumpOrbToSpawnPrefab;
    public GameObject gravityOrbToSpawnPrefab;

    public bool randomSpawn; 
    public float bpm = 120;
    public Vector2 spawnPoint;
    private Coroutine spawnCoroutine;
    public int orbLaneMultiplierToSpawn;
    private Vector2 lane1Spawn = new Vector2(7, -2f);
    private Vector2 lane2Spawn = new Vector2(7, 0f);
    private Vector2 lane3Spawn = new Vector2(7, 2f);
    private Vector2 lane4Spawn = new Vector2(7, 4f);
    
    void Start()
    {
        if (randomSpawn || spawnPoint == null)
        {
            RestartSpawner();
        }
        else
        {
            Instantiate(jumpOrbToSpawnPrefab, spawnPoint, Quaternion.identity);
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
            float secondsPerOrb = 240f / bpm;
            int objectTypeToSpawn = Random.Range(0, 2); //0 for jump pads, 1 for gravity pads, 2 for orbs
            switch (objectTypeToSpawn)
            {
                case 0:
                    SpawnRandomJumpOrb(); break;
                case 1:
                    SpawnRandomGravityOrb(); break;
                default:
                    SpawnRandomJumpOrb(); break;
            }
            
            yield return new WaitForSeconds(secondsPerOrb);
        }
    }
    private void SpawnRandomGravityOrb()
    {
        int laneToSpawn = Random.Range(1, 4);
        GameObject newPad;
        switch (laneToSpawn)
        {
            case 1:
                newPad = Instantiate(gravityOrbToSpawnPrefab, lane1Spawn, Quaternion.identity);
                break;
            case 2:
                newPad = Instantiate(gravityOrbToSpawnPrefab, lane2Spawn, Quaternion.identity);
                break;
            case 3:
                newPad = Instantiate(gravityOrbToSpawnPrefab, lane3Spawn, Quaternion.identity);
                break;
            case 4:
                newPad = Instantiate(gravityOrbToSpawnPrefab, lane4Spawn, Quaternion.identity);
                break;
            default:
                newPad = Instantiate(gravityOrbToSpawnPrefab, lane1Spawn, Quaternion.identity);
                break;
        }
        GravityPadVariables padVariables = newPad.GetComponent<GravityPadVariables>();
    }
    private void SpawnRandomJumpOrb()
    {
        int laneToSpawn = Random.Range(1, 4);
        GameObject newPad;
        switch (laneToSpawn)
        {
            case 1:
                newPad = Instantiate(jumpOrbToSpawnPrefab, lane1Spawn, Quaternion.identity);
                break;
            case 2:
                newPad = Instantiate(jumpOrbToSpawnPrefab, lane2Spawn, Quaternion.identity);
                break;
            case 3:
                newPad = Instantiate(jumpOrbToSpawnPrefab, lane3Spawn, Quaternion.identity);
                break;
            case 4:
                newPad = Instantiate(jumpOrbToSpawnPrefab, lane4Spawn, Quaternion.identity);
                break;
            default:
                newPad = Instantiate(jumpOrbToSpawnPrefab, lane1Spawn, Quaternion.identity);
                break;
        }
        JumpOrbVariables padVariables = newPad.GetComponent<JumpOrbVariables>();
        padVariables.setLaneMultiplier(Random.Range(1, 4));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
