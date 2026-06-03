using UnityEngine;

/// <summary>
/// Central, scene-independent player for one-shot gameplay sound effects
/// (jump, death, orb collect, level-complete victory).
///
/// Why a dedicated manager instead of <see cref="AudioManager.PlaySFX"/>:
/// the music rides on the AudioManager's single AudioSource, which the
/// <see cref="GameManager"/> Pauses on game-over and level-complete.
/// PlayOneShot on a paused source is unreliable, so the death and victory
/// stingers could be swallowed. This manager owns its OWN AudioSource that is
/// never paused, guaranteeing the cue is heard at exactly those moments.
///
/// Zero setup required: it auto-bootstraps before the first scene loads and
/// survives scene changes (DontDestroyOnLoad). Clips are resolved in this order:
///   1. A real clip dropped into <c>Resources/SFX/{Jump,Die,Collect,Victory}</c>.
///   2. A procedural placeholder tone generated at runtime.
/// This satisfies the "use placeholder audio until UI/UX delivers the final SFX"
/// requirement - the team only has to drop files into Resources/SFX to upgrade.
///
/// SFX volume tracks the same PlayerPrefs key ("SFXVolume", linear 0..1) the
/// options slider writes, so the in-game SFX volume control affects these cues.
/// </summary>
public class SfxManager : MonoBehaviour
{
    public static SfxManager Instance { get; private set; }

    /// <summary>PlayerPrefs key the options SFX slider writes (linear 0..1).</summary>
    private const string SfxVolumeKey = "SFXVolume";

    private AudioSource source;

    private AudioClip jumpClip;
    private AudioClip dieClip;
    private AudioClip collectClip;
    private AudioClip victoryClip;

    /// <summary>
    /// Creates the singleton automatically before any scene loads, so every scene
    /// (including a level launched directly from the Editor) has SFX with no manual
    /// wiring. Guarded so a manually placed instance is never duplicated.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;
        var go = new GameObject("SfxManager");
        go.AddComponent<SfxManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;

        // Real clip if present in Resources/SFX, otherwise a generated placeholder.
        jumpClip = Resources.Load<AudioClip>("SFX/Jump") ?? GenerateBlip(520f, 880f, 0.12f);
        collectClip = Resources.Load<AudioClip>("SFX/Collect") ?? GenerateBlip(1046f, 1568f, 0.10f);
        dieClip = Resources.Load<AudioClip>("SFX/Die") ?? GenerateBlip(440f, 70f, 0.35f);
        victoryClip = Resources.Load<AudioClip>("SFX/Victory") ?? GenerateArpeggio();
    }

    // --- Public play helpers (call these from gameplay code) -------------------

    public void PlayJump() => Play(jumpClip);
    public void PlayDie() => Play(dieClip);
    public void PlayCollect() => Play(collectClip);
    public void PlayVictory() => Play(victoryClip);

    /// <summary>
    /// Plays a one-shot clip at the current SFX volume. Safe to call from any state
    /// (the dedicated source is never paused) and ignores null clips.
    /// </summary>
    public void Play(AudioClip clip)
    {
        if (clip == null || source == null) return;
        float volume = Mathf.Clamp01(PlayerPrefs.GetFloat(SfxVolumeKey, 1f));
        source.PlayOneShot(clip, volume);
    }

    // --- Procedural placeholder generation -------------------------------------

    /// <summary>
    /// Builds a short tone that sweeps from <paramref name="freqStart"/> to
    /// <paramref name="freqEnd"/> with an exponential volume decay - a simple,
    /// recognisable "blip" placeholder until real audio is supplied.
    /// </summary>
    private static AudioClip GenerateBlip(float freqStart, float freqEnd, float duration)
    {
        const int sampleRate = 44100;
        int sampleCount = Mathf.Max(1, Mathf.RoundToInt(sampleRate * duration));
        float[] data = new float[sampleCount];

        float phase = 0f;
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;                 // 0..1 over the clip
            float freq = Mathf.Lerp(freqStart, freqEnd, t);
            phase += 2f * Mathf.PI * freq / sampleRate;
            float envelope = Mathf.Exp(-4f * t);              // quick decay
            data[i] = Mathf.Sin(phase) * envelope * 0.5f;
        }

        AudioClip clip = AudioClip.Create("SFX_Blip", sampleCount, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    /// <summary>
    /// Builds a short ascending 4-note arpeggio (C5-E5-G5-C6) as a celebratory
    /// placeholder for the level-complete cue.
    /// </summary>
    private static AudioClip GenerateArpeggio()
    {
        const int sampleRate = 44100;
        float noteDuration = 0.12f;
        float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f };
        int noteSamples = Mathf.RoundToInt(sampleRate * noteDuration);
        int sampleCount = noteSamples * notes.Length;
        float[] data = new float[sampleCount];

        for (int n = 0; n < notes.Length; n++)
        {
            float phase = 0f;
            for (int i = 0; i < noteSamples; i++)
            {
                float t = (float)i / noteSamples;
                phase += 2f * Mathf.PI * notes[n] / sampleRate;
                float envelope = Mathf.Exp(-3f * t);
                data[n * noteSamples + i] = Mathf.Sin(phase) * envelope * 0.45f;
            }
        }

        AudioClip clip = AudioClip.Create("SFX_Victory", sampleCount, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
