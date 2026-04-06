using TMPro;
using UnityEngine;

public class SegmentUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI songText;
    public TextMeshProUGUI diffText;

    public void Setup(string level, string song, string diff)
    {
        levelText.text = level;
        songText.text = song;
        diffText.text = diff;
    }
}