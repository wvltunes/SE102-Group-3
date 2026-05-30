using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Route the kill through the player so the GameManager can handle
            // the game-over state instead of reloading the scene directly here.
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.Die();
            }
        }
    }
}
