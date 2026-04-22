using UnityEngine;

public class GravityOrbBehaviour : MonoBehaviour
{
    private bool isTriggered = false;
    private bool isColliding = false;
    private Collider2D collider2D;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.ToggleReverseGravity();
            isTriggered = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isColliding = false;
            isTriggered = false;
        }
        collider2D = null;
    }
}
