using UnityEngine;
using DG.Tweening;

public class PlayerBeatPulse : MonoBehaviour
{
    [SerializeField] private float punchScale = 0.1f;
    [SerializeField] private float duration = 0.1f;

    private PlayerController player;

    void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public void Pulse()
    {
        if (player != null && !player.IsOnGroundLane())
            return;

        transform.DOKill();

        transform.DOPunchScale(
            Vector3.one * punchScale,
            duration,
            vibrato: 8,
            elasticity: 0.8f
        );
    }
}