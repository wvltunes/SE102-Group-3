using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;

public class TestObjectSpawner : MonoBehaviour
{

    public GameObject padToSpawnPrefab;
    public GameObject orbToSpawnPrefab;
    public bool randomSpawn; //not true random, use for testing
    public float bpm;
    public Vector2 spawnPoint;
    private Coroutine spawnCoroutine;
    public int padLaneMultiplierToSpawn;
    private Vector2 lane1Spawn = new Vector2 (7, -2.9f);
    private Vector2 lane2Spawn = new Vector2(7, -0.9f);
    private Vector2 lane3Spawn = new Vector2(7, 1.1f);
    private Vector2 lane4Spawn = new Vector2(7, 3.1f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (randomSpawn || spawnPoint == null)
        {
            RestartSpawner();
        }
        else
        {
            Instantiate(padToSpawnPrefab, spawnPoint, Quaternion.identity);
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
            //Spawn jump orbs


            //Spawn gravity pad


            //Spawn jump pads
            int laneToSpawn = Random.Range(1, 4);
            GameObject newPad;
            switch (laneToSpawn)
            {
                case 1:
                    newPad = Instantiate(padToSpawnPrefab, lane1Spawn, Quaternion.identity);
                    break;
                case 2:
                    newPad = Instantiate(padToSpawnPrefab, lane2Spawn, Quaternion.identity);
                    break;
                case 3:
                    newPad = Instantiate(padToSpawnPrefab, lane3Spawn, Quaternion.identity);
                    break;
                case 4:
                    newPad = Instantiate(padToSpawnPrefab, lane4Spawn, Quaternion.identity);
                    break;
                default:
                    newPad = Instantiate(padToSpawnPrefab, lane1Spawn, Quaternion.identity);
                    break;
            }
            PadVariables padVariables = newPad.GetComponent<PadVariables>();
            padVariables.setLaneMultiplier (Random.Range(1, 4));
            
            yield return new WaitForSeconds(secondsPerPad);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
