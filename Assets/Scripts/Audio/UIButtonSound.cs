using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    public AudioClip clickSound;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        if (AudioManager.instance != null && clickSound != null)
        {
            AudioManager.instance.PlaySFX(clickSound);
        }
    }
}