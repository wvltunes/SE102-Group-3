using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public AudioMixer mixer;

    public string exposedName = "MusicVolume";

    public float step = 0.1f;

    void Start()
    {
        float saved = PlayerPrefs.GetFloat(exposedName, 1f);

        slider.value = saved;

        Apply(saved);

        slider.onValueChanged.AddListener(Apply);
    }

    public void Increase()
    {
        slider.value += step;
    }

    public void Decrease()
    {
        slider.value -= step;
    }

    void Apply(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);

        mixer.SetFloat(
            exposedName,
            Mathf.Log10(value) * 20
        );

        PlayerPrefs.SetFloat(exposedName, value);
    }
}