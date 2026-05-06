using UnityEngine;

/// <summary>
/// Handles the leftward movement of Block objects, consistent with other game objects.
/// </summary>
public class BlockMovement : MonoBehaviour
{
    [SerializeField] private float gameSpeed = 5f;

    void Update()
    {
        // Move block from right to left, same pattern as other obstacles
        transform.position += Vector3.left * gameSpeed * Time.deltaTime;
    }
}
