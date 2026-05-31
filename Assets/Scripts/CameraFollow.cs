using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(5f, 0, -10f);
    
    [Header("Camera Bounds")]
    [SerializeField] private float minY = -2f;
    [SerializeField] private float maxY = 6f;
    
    private Transform player;

    void Start()
    {
        // Auto-find Player in scene
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            player = playerController.transform;
        }
        else
        {
            Debug.LogError("[CameraFollow] Player not found in scene!");
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Target position = player position + offset
        Vector3 targetPosition = player.position + offset;

        // Clamp Y position (prevent camera from going too high/low)
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        // Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
