using UnityEngine;
using System.Collections;

public class SceneMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private float bpm = 120f;
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float targetVolume = 1f;

    void Start()
    {
        StartCoroutine(WaitThenPlay());
    }

    IEnumerator WaitThenPlay()
    {
        // Chờ 1 frame để AudioManager khởi tạo xong
        yield return null;

        if (AudioManager.instance == null)
        {
            Debug.LogError("KHÔNG tìm thấy AudioManager!");
            yield break;
        }

        if (musicClip == null)
        {
            Debug.LogError("Chưa kéo AudioClip vào Inspector!");
            yield break;
        }

        Debug.Log($"Bắt đầu phát: {musicClip.name}");
        AudioManager.instance.SetVolume(0f);
        AudioManager.instance.Play(musicClip, bpm);

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            AudioManager.instance.SetVolume(Mathf.Lerp(0f, targetVolume, elapsed / fadeInDuration));
            yield return null;
        }

        AudioManager.instance.SetVolume(targetVolume);
        Debug.Log("Fade in xong!");
    }
}