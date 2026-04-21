using UnityEngine;

public class SpikeMovement : MonoBehaviour
{

    [SerializeField] private float gameSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Movement is temporary, might need fundamental changes to move the camera instead of the map
        transform.position += Vector3.left * gameSpeed * Time.deltaTime;
    }
}
