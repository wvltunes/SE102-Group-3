using UnityEngine;

public class UniversalKillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "LaneReducer":
                // ONLY destroy if it is a clone
                if (other.gameObject.name.Contains("(Clone)"))
                {
                    Destroy(other.gameObject);
                }

                break;

            case "Obstacle":
            case "Enemy":
            case "Pad":
                Destroy(other.gameObject);
                break;
            case "Projectile":
                // If these tags also have originals in the scene, 
                // you can wrap these in the (Clone) check too.
                Destroy(other.gameObject);
                break;

            case "Player":
                // Do nothing
                break;
        }
    }
}