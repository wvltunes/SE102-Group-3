using UnityEngine;

public class AudioStarter : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private float bpm = 120f;

    void Start()
    {
        if (musicClip == null) return;
        AudioManager.instance.Play(musicClip, bpm);
    }
}