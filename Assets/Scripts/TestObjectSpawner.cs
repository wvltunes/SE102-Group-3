using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;

public class TestObjectSpawner : MonoBehaviour
{
    [SerializeField] bool Spikes = true;
    [SerializeField] bool Pads = true;
    [SerializeField] bool Orbs = true;

    public GameObject jumpPadToSpawnPrefab;
    public GameObject gravityPadToSpawnPrefab;
    public GameObject jumpOrbToSpawnPrefab;
    public GameObject spikeToSpawnPrefab;

    public bool randomSpawn; //not true random, use for testing
    public float bpm;
    public Vector2 spawnPoint;
    private Coroutine spawnCoroutine;
    public int padLaneMultiplierToSpawn;
    private Vector2 lane1Spawn = new Vector2 (7, -2.9f);
    private Vector2 lane2Spawn = new Vector2(7, -0.9f);
    private Vector2 lane3Spawn = new Vector2(7, 1.1f);
    private Vector2 lane4Spawn = new Vector2(7, 3.1f);
    private Vector2 lane1OrbSpawn = new Vector2(7, -2f);
    private Vector2 lane2OrbSpawn = new Vector2(7, 0f);
    private Vector2 lane3OrbSpawn = new Vector2(7, 2f);
    private Vector2 lane4OrbSpawn = new Vector2(7, 4f);
    private Vector2 lane1SpikeSpawn = new Vector2(7, -2.7f);
    private Vector2 spikeSpawnLaneIncrement = new Vector2(0, 2.0f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (randomSpawn || spawnPoint == null)
        {
            RestartSpawner();
        }
        else
        {
            Instantiate(jumpPadToSpawnPrefab, spawnPoint, Quaternion.identity);
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

            int objectTypeToSpawn = Random.Range(0,3); //0 for jump pads, 1 for gravity pads, 2 for orbs, 3 for spikes
            switch (objectTypeToSpawn)
            {
                case 0:
                    if (Pads)
                    { 
                        SpawnRandomJumpPads(); 
                    } 
                    else
                    {
                        goto Case1;
                    }
                    break;
                case 1:
                Case1:
                    if (Orbs)
                    {
                        SpawnRandomJumpOrb();
                    }
                    else
                    {
                        goto Case2;
                    }
                        break;
                case 2:
                Case2:
                    if (Pads)
                    {
                        SpawnRandomGravityPads();
                    }
                    else
                    {
                        goto Case3;
                    }
                        break;
                case 3:
                Case3:
                    if (Spikes)
                    {
                        SpawnRandomSpikes();
                    }
                    else
                    {

                    }
                        break;
            }
            //Spawn jump orbs




            //Spawn j
            
            
            yield return new WaitForSeconds(secondsPerPad);
        }
    }
    private void SpawnRandomJumpOrb()
    {
        int laneToSpawn = Random.Range(1, 4);
        GameObject newPad;
        switch (laneToSpawn)
        {
            case 1:
                newPad = Instantiate(jumpOrbToSpawnPrefab, lane1OrbSpawn, Quaternion.identity);
                break;
            case 2:
                newPad = Instantiate(jumpOrbToSpawnPrefab, lane2OrbSpawn, Quaternion.identity);
                break;
            case 3:
                newPad = Instantiate(jumpOrbToSpawnPrefab, lane3OrbSpawn, Quaternion.identity);
                break;
            case 4:
                newPad = Instantiate(jumpOrbToSpawnPrefab, lane4OrbSpawn, Quaternion.identity);
                break;
            default:
                newPad = Instantiate(jumpOrbToSpawnPrefab, lane1OrbSpawn, Quaternion.identity);
                break;
        }
        JumpOrbVariables padVariables = newPad.GetComponent<JumpOrbVariables>();
        padVariables.setLaneMultiplier(Random.Range(1, 4));
    }
    private void SpawnRandomGravityPads()
    {
        int laneToSpawn = Random.Range(1, 4);
        GameObject newPad;
        switch (laneToSpawn)
        {
            case 1:
                newPad = Instantiate(gravityPadToSpawnPrefab, lane1Spawn, Quaternion.identity);
                break;
            case 2:
                newPad = Instantiate(gravityPadToSpawnPrefab, lane2Spawn, Quaternion.identity);
                break;
            case 3:
                newPad = Instantiate(gravityPadToSpawnPrefab, lane3Spawn, Quaternion.identity);
                break;
            case 4:
                newPad = Instantiate(gravityPadToSpawnPrefab, lane4Spawn, Quaternion.identity);
                break;
            default:
                newPad = Instantiate(gravityPadToSpawnPrefab, lane1Spawn, Quaternion.identity);
                break;
        }
        GravityPadVariables padVariables = newPad.GetComponent<GravityPadVariables>();
    }
    private void SpawnRandomJumpPads ()
    {
        int laneToSpawn = Random.Range(1, 4);
        GameObject newPad;
        switch (laneToSpawn)
        {
            case 1:
                newPad = Instantiate(jumpPadToSpawnPrefab, lane1Spawn, Quaternion.identity);
                break;
            case 2:
                newPad = Instantiate(jumpPadToSpawnPrefab, lane2Spawn, Quaternion.identity);
                break;
            case 3:
                newPad = Instantiate(jumpPadToSpawnPrefab, lane3Spawn, Quaternion.identity);
                break;
            case 4:
                newPad = Instantiate(jumpPadToSpawnPrefab, lane4Spawn, Quaternion.identity);
                break;
            default:
                newPad = Instantiate(jumpPadToSpawnPrefab, lane1Spawn, Quaternion.identity);
                break;
        }
        JumpPadVariables padVariables = newPad.GetComponent<JumpPadVariables>();
        padVariables.setLaneMultiplier(Random.Range(1, 4));
    }

    private void SpawnRandomSpikes()
    {
        int laneToSpawn = Random.Range(1, 4);
        GameObject newSpike;
        switch (laneToSpawn)
        {
            case 1:
                newSpike = Instantiate(spikeToSpawnPrefab, lane1SpikeSpawn, Quaternion.identity);
                break;
            case 2:
                newSpike = Instantiate(spikeToSpawnPrefab, lane1SpikeSpawn + 1 * spikeSpawnLaneIncrement, Quaternion.identity);
                break;
            case 3:
                newSpike = Instantiate(spikeToSpawnPrefab, lane1SpikeSpawn + 2 * spikeSpawnLaneIncrement, Quaternion.identity);
                break;
            case 4:
                newSpike = Instantiate(spikeToSpawnPrefab, lane1SpikeSpawn + 4 * spikeSpawnLaneIncrement, Quaternion.identity);
                break;
            default:
                newSpike = Instantiate(spikeToSpawnPrefab, lane1Spawn, Quaternion.identity);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
