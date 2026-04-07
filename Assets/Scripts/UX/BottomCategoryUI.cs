using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EditorMode { Build, Edit, Delete }

public class BottomCategoryUI : MonoBehaviour
{
    [Header("Content")]
    public Transform contentParent;
    public GameObject itemPrefab;

    [Header("Category Buttons")]
    public Image blockButtonImage;
    public Image spikeButtonImage;
    public Image portalButtonImage;
    public Image orbButtonImage;
    public Image triggerButtonImage;

    [Header("Page Buttons")]
    public Button leftButton;
    public Button rightButton;

    [Header("Button Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    [Header("Sprites")]
    public List<Sprite> blockSprites;
    public List<Sprite> spikeSprites;
    public List<Sprite> portalSprites;
    public List<Sprite> orbSprites;
    public List<Sprite> triggerSprites;

    [Header("Page Setting")]
    public int itemsPerPage = 12;

    [Header("Mode Buttons")]
    public Image buildButtonImage;
    public Image editButtonImage;
    public Image deleteButtonImage;

    [Header("Mode Panels")]
    public GameObject categoryBar;
    public GameObject actionButtons;

    [Header("Mode Button Colors")]
    public Color modeNormalColor = Color.white;
    public Color modeSelectedColor = new Color(0.5f, 0.9f, 0.85f, 1f);

    private List<Sprite> currentSprites = new List<Sprite>();
    private int currentPage = 0;
    private EditorMode currentMode = EditorMode.Build;

    void Start()
    {
        SwitchToBuild();
    }

    // ==================== Mode Switching ====================

    public void SwitchToBuild()
    {
        currentMode = EditorMode.Build;
        UpdateSelectedMode(buildButtonImage);

        if (categoryBar != null) categoryBar.SetActive(true);
        if (actionButtons != null) actionButtons.SetActive(true);

        leftButton.gameObject.SetActive(true);
        rightButton.gameObject.SetActive(true);

        ShowBlock();
    }

    public void SwitchToEdit()
    {
        currentMode = EditorMode.Edit;
        UpdateSelectedMode(editButtonImage);

        if (categoryBar != null) categoryBar.SetActive(false);
        if (actionButtons != null) actionButtons.SetActive(true);

        ClearItems();
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
    }

    public void SwitchToDelete()
    {
        currentMode = EditorMode.Delete;
        UpdateSelectedMode(deleteButtonImage);

        if (categoryBar != null) categoryBar.SetActive(false);
        if (actionButtons != null) actionButtons.SetActive(false);

        ClearItems();
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
    }

    void UpdateSelectedMode(Image selectedButton)
    {
        if (buildButtonImage != null) buildButtonImage.color = modeNormalColor;
        if (editButtonImage != null) editButtonImage.color = modeNormalColor;
        if (deleteButtonImage != null) deleteButtonImage.color = modeNormalColor;

        if (selectedButton != null) selectedButton.color = modeSelectedColor;
    }

    // ==================== Category Switching ====================

    public void ShowBlock()
    {
        currentSprites = blockSprites;
        currentPage = 0;
        ShowCurrentPage();
        UpdateSelectedButton(blockButtonImage);
    }

    public void ShowSpike()
    {
        currentSprites = spikeSprites;
        currentPage = 0;
        ShowCurrentPage();
        UpdateSelectedButton(spikeButtonImage);
    }

    public void ShowPortal()
    {
        currentSprites = portalSprites;
        currentPage = 0;
        ShowCurrentPage();
        UpdateSelectedButton(portalButtonImage);
    }

    public void ShowOrb()
    {
        currentSprites = orbSprites;
        currentPage = 0;
        ShowCurrentPage();
        UpdateSelectedButton(orbButtonImage);
    }

    public void ShowTrigger()
    {
        currentSprites = triggerSprites;
        currentPage = 0;
        ShowCurrentPage();
        UpdateSelectedButton(triggerButtonImage);
    }

    // ==================== Pagination ====================

    public void NextPage()
    {
        int maxPage = Mathf.CeilToInt((float)currentSprites.Count / itemsPerPage) - 1;

        if (currentPage < maxPage)
        {
            currentPage++;
            ShowCurrentPage();
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowCurrentPage();
        }
    }

    void ShowCurrentPage()
    {
        ClearItems();

        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, currentSprites.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject item = Instantiate(itemPrefab, contentParent);
            Image icon = item.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = currentSprites[i];
        }

        UpdatePageButtons();
    }

    void ClearItems()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }
    }

    void UpdateSelectedButton(Image selectedButton)
    {
        blockButtonImage.color = normalColor;
        spikeButtonImage.color = normalColor;
        portalButtonImage.color = normalColor;
        orbButtonImage.color = normalColor;
        triggerButtonImage.color = normalColor;

        selectedButton.color = selectedColor;
    }

    void UpdatePageButtons()
    {
        int maxPage = Mathf.CeilToInt((float)currentSprites.Count / itemsPerPage) - 1;

        leftButton.gameObject.SetActive(true);
        rightButton.gameObject.SetActive(true);

        leftButton.interactable = currentPage > 0;
        rightButton.interactable = currentPage < maxPage;

        Image leftImg = leftButton.GetComponent<Image>();
        Image rightImg = rightButton.GetComponent<Image>();

        if (leftImg != null)
            leftImg.color = currentPage > 0 ? Color.white : new Color(1f, 1f, 1f, 0.35f);

        if (rightImg != null)
            rightImg.color = currentPage < maxPage ? Color.white : new Color(1f, 1f, 1f, 0.35f);
    }
}