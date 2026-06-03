using UnityEngine;
using TMPro;

public class SongProgressUI : MonoBehaviour
{
    [SerializeField] private TMP_Text progressText;

    private void Update()
    {
        if (AudioManager.instance == null) return;

        float current = AudioManager.instance.GetCurrentTimeInSong();
        float duration = AudioManager.instance.GetSongDuration();

        if (duration <= 0) return;

        float progress = current / duration;

        int percent = Mathf.RoundToInt(progress * 100f);

        progressText.text = percent + "%";
    }
}