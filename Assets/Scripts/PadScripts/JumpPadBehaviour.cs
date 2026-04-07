using UnityEngine;

public class JumpPadBehaviour : MonoBehaviour
{
    [SerializeField] private int laneToJump = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.JumpPlayer(laneToJump);
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
