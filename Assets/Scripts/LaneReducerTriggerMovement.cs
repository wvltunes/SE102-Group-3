using UnityEngine;

public class LaneReducerTriggerMovement : MonoBehaviour
{
    [SerializeField] private float gameSpeed = 5f; 
    [SerializeField] private float triggerRangeX = 1f;
    [SerializeField] private bool isEnabled = true;
    
    private PlayerController playerController;
    private bool hasTriggered = false; 

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        // Move trigger from right to left
        transform.position += Vector3.left * gameSpeed * Time.deltaTime;

        // Trigger logic
        if (playerController != null && !hasTriggered)
        {
            float distanceX = Mathf.Abs(playerController.transform.position.x - transform.position.x);
            
            if (distanceX < triggerRangeX && this.isEnabled)
            {
                playerController.ReduceLane();
                hasTriggered = true;
            }
        }
        // Note: Destruction is now handled by the UniversalKillZone object in the scene
    }
}