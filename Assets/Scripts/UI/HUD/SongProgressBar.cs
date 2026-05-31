using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD widget that shows how far through the song (and therefore the level) the
/// player is. Reads the playback position from the persistent <see cref="AudioManager"/>
/// every frame and writes a normalised 0..1 value to a filled Image and/or a Slider.
///
/// Assign whichever presentation you use (both are optional):
///   - Image with Image Type = Filled (Fill Method = Horizontal): drives fillAmount.
///   - Slider with Min Value 0 and Max Value 1: drives value.
/// </summary>
public class SongProgressBar : MonoBehaviour
{
    [Tooltip("Filled Image (Image Type = Filled, Fill Method = Horizontal). Optional.")]
    [SerializeField] private Image fillImage;

    [Tooltip("Slider with Min Value 0 and Max Value 1. Optional.")]
    [SerializeField] private Slider slider;

    private void Update()
    {
        AudioManager am = AudioManager.instance;
        if (am == null) return;

        float duration = am.GetSongDuration();

        // Guard against divide-by-zero before any clip is loaded (duration == 0).
        float progress = duration > 0f
            ? Mathf.Clamp01(am.GetCurrentTimeInSong() / duration)
            : 0f;

        if (fillImage != null)
        {
            fillImage.fillAmount = progress;
        }
        if (slider != null)
        {
            slider.value = progress;
        }
    }
}
