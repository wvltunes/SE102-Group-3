using UnityEngine;

/// <summary>
/// Handles the leftward movement of Enemy objects, consistent with other game objects.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float gameSpeed = 5f;

    void Update()
    {
        // Horizontal movement removed - obstacles now move with camera
    }
}
