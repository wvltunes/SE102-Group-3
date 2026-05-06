using UnityEngine;
using System.Collections;

public class GravityOrbBehaviour : MonoBehaviour
{
    private bool playerInside = false;
    private PlayerController player;
    private bool isTriggered = false;

    [Header("Input Buffer")]
    [SerializeField] private float inputBufferTime = 0.12f;
    private float inputBufferCounter = 0f;

    [Header("Effect")]
    [SerializeField] private float shrinkDuration = 0.15f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.GetComponent<PlayerController>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            player = null;
            isTriggered = false;
        }
    }

    void Update()
    {
        if (isTriggered) return;

       
        if (Input.GetMouseButtonDown(0))
        {
            inputBufferCounter = inputBufferTime;
        }

    
        if (inputBufferCounter > 0)
        {
            inputBufferCounter -= Time.deltaTime;
        }

       
        if (playerInside && player != null && inputBufferCounter > 0)
        {
            player.ToggleReverseGravity();
            isTriggered = true;

            StartCoroutine(ShrinkAndDestroy());
        }
    }

   
    IEnumerator ShrinkAndDestroy()
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < shrinkDuration)
        {
            float t = time / shrinkDuration;

           
            t = 1 - Mathf.Pow(1 - t, 3);

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }
}