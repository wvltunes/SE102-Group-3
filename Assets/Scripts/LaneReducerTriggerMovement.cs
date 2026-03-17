using UnityEngine;

public class LaneReducerTriggerMovement : MonoBehaviour
{
    [SerializeField] private float gameSpeed = 5f; // Same speed as Parallax script
    [SerializeField] private float triggerRangeX = 1f; // Range to detect player crossing trigger
    [SerializeField] private float resetPositionX = 20f; // Position to reset trigger when it goes off-screen
    [SerializeField] private float despawnX = -15f; // Position where trigger is considered off-screen
    
    private PlayerController playerController;
    private bool hasTriggered = false; // Flag to trigger only once per reset
    private Vector3 startPosition; // Starting position of the trigger

    void Start()
    {
        startPosition = transform.position;
        // Find the player in the scene
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        // Move trigger from right to left
        transform.position += Vector3.left * gameSpeed * Time.deltaTime;

        // Check if player X position is within trigger range (regardless of Y position)
        if (playerController != null && !hasTriggered)
        {
            float distanceX = Mathf.Abs(playerController.transform.position.x - transform.position.x);
            
            if (distanceX < triggerRangeX)
            {
                // Trigger the lane reducer
                playerController.ReduceLane();
                hasTriggered = true;
            }
        }

        // Reset position when it goes off-screen
        if (transform.position.x < despawnX)
        {
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        // Reset to starting position and reset trigger flag
        Vector3 newPosition = transform.position;
        newPosition.x = resetPositionX;
        transform.position = newPosition;
        hasTriggered = false; // Reset flag so trigger can be activated again
    }
}
