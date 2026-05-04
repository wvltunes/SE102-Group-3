using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MenuButton : MonoBehaviour
{
    public Image bg;
    public Sprite normalSprite;
    public Sprite selectedSprite;
    public Canvas overlayCanvas;

    public SettingRow[] panelRows;               // ✅ Kéo rows của panel này vào
    public SettingsMenuController settingsMenu;  // ✅ Kéo SettingsMenuController vào


    public string titleText;        
    public TMP_Text panelTitleText; 

    public GameObject contentPanel;

    [Header("Fill Settings")]
    public float normalFill = 0.79f;
    public float selectedFill = 1.0f;
    public float rotationAngle = 7f;

    private Transform originalParent;
    private int originalSiblingIndex;
    private bool isSelected = false;
    private GameObject placeholder;

   
    private Vector2 savedAnchoredPos;
    private Vector2 savedSizeDelta;
    private Vector2 savedAnchorMin;
    private Vector2 savedAnchorMax;
    private Vector2 savedPivot;

    void Start()
    {
        
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        if (bg != null)
        {
            bg.sprite = normalSprite;
            bg.fillAmount = normalFill;
        }

        
        if (contentPanel != null)
            contentPanel.SetActive(false);
    }

    public void Select()
    {
        if (isSelected) return;
        isSelected = true;

        if (panelTitleText != null)
            panelTitleText.text = titleText;

        if (contentPanel != null)
            contentPanel.SetActive(true);

         if (settingsMenu != null && panelRows != null)
            settingsMenu.SetRows(panelRows);


        if (bg != null)
        {
            bg.sprite = selectedSprite;
            bg.fillAmount = selectedFill;
            bg.transform.localRotation = Quaternion.Euler(0, 0, rotationAngle);
        }

        RectTransform rt = GetComponent<RectTransform>();
        savedAnchoredPos = rt.anchoredPosition;
        savedSizeDelta = rt.sizeDelta;
        savedAnchorMin = rt.anchorMin;
        savedAnchorMax = rt.anchorMax;
        savedPivot = rt.pivot;

        Vector3 worldPos = rt.position;

        DestroyPlaceholder();
        placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(originalParent, false);
        placeholder.transform.SetSiblingIndex(originalSiblingIndex);

        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = rt.rect.width;
        le.preferredHeight = rt.rect.height;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        transform.SetParent(overlayCanvas.transform, false);

        RectTransform overlayRect = overlayCanvas.GetComponent<RectTransform>();
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            overlayRect, screenPoint, null, out localPoint
        );

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = savedSizeDelta;
        rt.anchoredPosition = localPoint;
        rt.localScale = Vector3.one * 1.1f;
    }

    public void Deselect()
    {
        if (!isSelected) return;
        isSelected = false;

        if (contentPanel != null)
            contentPanel.SetActive(false);

        if (bg != null)
        {
            bg.sprite = normalSprite;
            bg.fillAmount = normalFill;
            bg.transform.localRotation = Quaternion.identity;
        }

        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);

        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = savedAnchorMin;
        rt.anchorMax = savedAnchorMax;
        rt.pivot = savedPivot;
        rt.sizeDelta = savedSizeDelta;
        rt.anchoredPosition = savedAnchoredPos;
        rt.localScale = Vector3.one;

        DestroyPlaceholder();
    }
    void DestroyPlaceholder()
    {
        if (placeholder != null)
        {
            Destroy(placeholder);
            placeholder = null;
        }
    }

    void OnDestroy()
    {
        DestroyPlaceholder();
    }
}