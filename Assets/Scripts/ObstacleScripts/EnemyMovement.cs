using UnityEngine;

/// <summary>
/// Handles the leftward movement of Enemy objects, consistent with other game objects.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float gameSpeed = 5f;

    void Update()
    {
        // Move enemy from right to left, same pattern as other obstacles
        transform.position += Vector3.left * gameSpeed * Time.deltaTime;
    }
}
