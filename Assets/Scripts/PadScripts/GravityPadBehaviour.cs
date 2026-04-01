using UnityEngine;

public class GravityPadBehaviour : MonoBehaviour
{
    private bool isTriggered = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!isTriggered)
            {
                PlayerController playerController = other.GetComponent<PlayerController>();
                playerController.ToggleReverseGravity();
                isTriggered = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isTriggered = false;
        }
    }
}
