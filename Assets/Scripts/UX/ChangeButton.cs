using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    public Image image;

    public Sprite normalSprite;
    public Sprite hoverSprite;
    public Sprite clickedSprite; // có thể để trống (null)

    private bool isClicked = false;

    private void Start()
    {
        image.sprite = normalSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isClicked)
            image.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isClicked)
            image.sprite = normalSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isClicked = true;

        // nếu có sprite click thì dùng, không có thì giữ nguyên hover/normal
        if (clickedSprite != null)
            image.sprite = clickedSprite;
    }

    public void ResetButton()
    {
        isClicked = false;
        image.sprite = normalSprite;
    }
}