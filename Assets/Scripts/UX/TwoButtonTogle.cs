using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TwoButtonToggle : MonoBehaviour
{
    [Header("UI")]
    public Button btnLeft;
    public Button btnRight;
    public TMP_Text statusText;

    [Header("Options - Điền trong Inspector")]
    public string[] options = { "OFF", "ON" };
    public int defaultIndex = 0;

    [Header("Mute Control (chỉ tick nếu là Mute row)")]
    public bool isMuteControl = false;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterSlider;

    private int currentIndex;

    void Start()
    {
        btnLeft.onClick.AddListener(Previous);
        btnRight.onClick.AddListener(Next);

        currentIndex = defaultIndex;
        UpdateUI();
    }

    public void Next()
    {
        currentIndex++;
        if (currentIndex >= options.Length) currentIndex = 0;
        UpdateUI();
    }

    public void Previous()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = options.Length - 1;
        UpdateUI();
    }

    public void SetByValue(string value)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i] == value)
            {
                currentIndex = i;
                UpdateUI();
                return;
            }
        }
    }

    public void SetByIndex(int index)
    {
        currentIndex = Mathf.Clamp(index, 0, options.Length - 1);
        UpdateUI();
    }

   
    public void SetOn() => SetByValue("ON");
    public void SetOff() => SetByValue("OFF");

    public string GetValue() => options[currentIndex];
    public int GetIndex() => currentIndex;

    void UpdateUI()
    {
        if (statusText != null)
            statusText.text = options[currentIndex];

        if (isMuteControl)
            HandleMute();
    }

    void HandleMute()
    {
        bool muted = options[currentIndex] == "ON";
        float val = muted ? 0f : 0.5f;

        if (musicSlider != null) musicSlider.value = val;
        if (sfxSlider != null) sfxSlider.value = val;
        if (masterSlider != null) masterSlider.value = val;
    }
}