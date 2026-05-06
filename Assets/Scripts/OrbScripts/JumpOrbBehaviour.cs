using UnityEngine;
using System.Collections;

public class JumpOrbBehaviour : MonoBehaviour
{
    [SerializeField] private int laneToJump = 1;

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

        // bắt input
        if (Input.GetMouseButtonDown(0))
        {
            inputBufferCounter = inputBufferTime;
        }

        if (inputBufferCounter > 0)
        {
            inputBufferCounter -= Time.deltaTime;
        }

        // kích hoạt
        if (playerInside && player != null && inputBufferCounter > 0)
        {
            player.JumpPlayer(laneToJump);
            isTriggered = true;

            StartCoroutine(ShrinkAndDestroy());
        }
    }

    // EFFECT 
    IEnumerator ShrinkAndDestroy()
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < shrinkDuration)
        {
            float t = time / shrinkDuration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }

  
    public int getLaneToJump() => laneToJump;
    public void setLaneToJump(int lane) => laneToJump = lane;
}