using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;
    private float currentBPM = 120f;

    // Build index of the scene the current track belongs to. Used to restart
    // that track from the beginning when the SAME scene is (re)loaded - e.g.
    // after the player dies and presses Retry.
    private int musicSceneBuildIndex = -1;

    void Awake()
    {
        // Singleton Pattern. If a persistent instance already exists, this is a
        // duplicate that shipped inside a (re)loaded scene - destroy it and bail
        // out so it never replaces the singleton or double-subscribes to events.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioManager must have an AudioSource component!");
        }

        // Restart the music when its scene is reloaded. This is the key to the
        // Retry fix: on game-over the persistent AudioSource is Paused, then the
        // scene reloads. The reloaded scene's AudioInitializer rides on this
        // SAME GameObject - which is now a duplicate that Awake() destroys above
        // - so its Start() never runs to restart playback. Handling sceneLoaded
        // here, on the surviving instance, guarantees the track plays again.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Only the persistent instance ever subscribes, so only it unsubscribes.
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Restart only when the SAME scene that owns the current track reloads
        // (the Retry case). This avoids blasting gameplay music into, say, the
        // main menu. On the very first load there is no track yet, so we skip
        // and let AudioInitializer.Start() kick off the first play.
        if (audioSource != null
            && audioSource.clip != null
            && scene.buildIndex == musicSceneBuildIndex
            && !audioSource.isPlaying)
        {
            audioSource.Stop();   // clear any Paused state + reset to position 0
            audioSource.Play();   // play from the beginning
            Debug.Log("[AudioManager] Restarted track after scene reload.");
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
            // Stop first to clear any leftover Paused state (e.g. from a
            // game-over Pause()) and reset playback to position 0. This
            // AudioSource survives DontDestroyOnLoad, so on a scene reload it
            // can still be internally paused; calling Play() on a paused source
            // does not reliably restart it and leaves the music silent.
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
            currentBPM = bpm;
            // Remember which scene this track belongs to so OnSceneLoaded can
            // restart it from the beginning when that scene is reloaded (Retry).
            musicSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
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
        // Clear any Paused state before playing so a previously paused source
        // (which persists across scene reloads) starts cleanly instead of
        // staying silent.
        audioSource.UnPause();
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
