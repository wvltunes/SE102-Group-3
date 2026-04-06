using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    [Header("UI")]
    public GameObject lockIcon;

    [Header("State")]
    public bool isUnlocked = false;

    private TMP_Text[] texts;

    void Awake()
    {
     
        // Lấy tất cả text con (support nhiều text)
        texts = GetComponentsInChildren<TMP_Text>();

        // Auto tìm lock icon nếu chưa gán
        if (lockIcon == null)
        {
            Transform t = transform.Find("LockIcon");
            if (t != null) lockIcon = t.gameObject;
        }
    }

    public void UpdateUI(bool isSelected)
    {
        if (!isUnlocked)
        {
            if (lockIcon != null) lockIcon.SetActive(true);
            SetAlpha(0.2f);
        }
        else
        {
            if (lockIcon != null) lockIcon.SetActive(false);
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