using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;
    private float currentBPM = 120f;

    void Awake()
    {
        // Singleton Pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioManager must have an AudioSource component!");
        }
    }

    /// <summary>
    /// Load AudioClip từ Resources folder
    /// </summary>
    public void LoadAudio(string clipPath)
    {
        AudioClip clip = Resources.Load<AudioClip>(clipPath);
        if (clip != null)
        {
            audioSource.clip = clip;
            Debug.Log($"Loaded audio: {clipPath}");
        }
        else
        {
            Debug.LogError($"Cannot load audio clip at: Resources/{clipPath}");
        }
    }

    /// <summary>
    /// Play âm thanh với BPM
    /// </summary>
    public void Play(AudioClip clip, float bpm)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            currentBPM = bpm;
            Debug.Log($"Playing audio with BPM: {bpm}");
        }
        else
        {
            Debug.LogError("AudioClip is null!");
        }
    }

    /// <summary>
    /// Play AudioClip hiện tại
    /// </summary>
    public void Play()
    {
        audioSource.Play();
    }

    /// <summary>
    /// Pause âm thanh
    /// </summary>
    public void Pause()
    {
        audioSource.Pause();
    }

    /// <summary>
    /// Stop và reset âm thanh
    /// </summary>
    public void Stop()
    {
        audioSource.Stop();
    }

    /// <summary>
    /// BPM của bài hát hiện tại
    /// </summary>
    public float CurrentBPM
    {
        get { return currentBPM; }
    }

    /// <summary>
    /// Kiểm tra âm thanh đang phát
    /// </summary>
    public bool IsPlaying
    {
        get { return audioSource.isPlaying; }
    }

    /// <summary>
    /// Lấy thời gian hiện tại của bài hát (giây)
    /// </summary>
    public float GetCurrentTimeInSong()
    {
        return audioSource.time;
    }

    /// <summary>
    /// Lấy độ dài bài hát (giây)
    /// </summary>
    public float GetSongDuration()
    {
        return audioSource.clip != null ? audioSource.clip.length : 0f;
    }

    /// <summary>
    /// Lấy thời gian của beat kế tiếp (giây)
    /// </summary>
    public float GetNextBeatTime()
    {
        float secondsPerBeat = 60f / currentBPM;
        float currentBeatNumber = GetCurrentTimeInSong() / secondsPerBeat;
        float nextBeatNumber = Mathf.Ceil(currentBeatNumber);
        return nextBeatNumber * secondsPerBeat;
    }

    /// <summary>
    /// Lấy số giây trên mỗi beat
    /// </summary>
    public float GetSecondsPerBeat()
    {
        return 60f / currentBPM;
    }

    /// <summary>
    /// Lấy beat số hiện tại
    /// </summary>
    public int GetCurrentBeat()
    {
        float secondsPerBeat = 60f / currentBPM;
        return Mathf.FloorToInt(GetCurrentTimeInSong() / secondsPerBeat);
    }

    /// <summary>
    /// Set BPM mới
    /// </summary>
    public void SetBPM(float bpm)
    {
        if (bpm > 0)
        {
            currentBPM = bpm;
            Debug.Log($"BPM set to: {bpm}");
        }
        else
        {
            Debug.LogError("BPM must be greater than 0!");
        }
    }

    /// <summary>
    /// Set vị trí phát lại (giây)
    /// </summary>
    public void SetTime(float time)
    {
        if (audioSource.clip != null && time >= 0 && time <= audioSource.clip.length)
        {
            audioSource.time = time;
        }
    }
}
