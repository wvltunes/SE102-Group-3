using UnityEngine;

public class JumpOrbBehaviour : MonoBehaviour
{
    [SerializeField] private int laneToJump = 1;
    private bool isTriggered = false;
    private bool isPlayerInZone = false;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.JumpPlayer(laneToJump);
            isTriggered = true;
            //isPlayerInZone = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isTriggered = false;
            isPlayerInZone = false;
        }
    }
    public int getLaneToJump () 
    { 
        return laneToJump; 
    }
    public void setLaneToJump (int laneToJump)
    {
        this.laneToJump = laneToJump;
    }
}
