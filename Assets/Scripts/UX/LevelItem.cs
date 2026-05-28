using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{

    [Header("State")]
    public bool isUnlocked = false;

    private TMP_Text[] texts;

    void Awake()
    {
        texts = GetComponentsInChildren<TMP_Text>();
    }

    public void UpdateUI(bool isSelected)
    {
        if (!isUnlocked)
        {
            SetAlpha(0.2f);
        }
        else
        {
            SetAlpha(isSelected ? 1f : 0.2f);
        }
        transform.localScale = isSelected ? Vector3.one * 1.2f : Vector3.one;
    }

    void SetAlpha(float a)
    {
      
        if (texts != null)
        {
            foreach (TMP_Text txt in texts)
            {
                txt.alpha = a;
            }
        }
    }
}