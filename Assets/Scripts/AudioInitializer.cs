using UnityEngine;

public class AudioInitializer : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float bpm = 160f;
    [SerializeField] private bool playOnStart = true;
    [Tooltip("Thời gian cắt bỏ đoạn im lặng ở đầu bài nhạc (tính bằng giây)")]
    [SerializeField] private float skipSilenceTime = 0f;

    void Start()
    {
        if (playOnStart && AudioManager.instance != null)
        {
            if (audioClip != null)
            {
                // Phát nhạc với BPM đã set
                AudioManager.instance.Play(audioClip, bpm);
                if (skipSilenceTime > 0f)
                {
                    AudioManager.instance.SetTime(skipSilenceTime);
                }
                Debug.Log($"Started playing: {audioClip.name} with BPM: {bpm}, skipped {skipSilenceTime}s of silence.");
            }
            else
            {
                Debug.LogError($"Audio clip is not assigned in the Inspector");
            }
        }
    }

    /// <summary>
    /// Phát nhạc thủ công từ script khác
    /// </summary>
    public void PlayAudio(AudioClip clip, float newBpm)
    {
        if (clip != null)
        {
            AudioManager.instance.Play(clip, newBpm);
        }
        else
        {
            Debug.LogError($"Audio clip is not assigned");
        }
    }

    /// <summary>
    /// Dừng nhạc
    /// </summary>
    public void StopAudio()
    {
        AudioManager.instance.Stop();
    }
}
