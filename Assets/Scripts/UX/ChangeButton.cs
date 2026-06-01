using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Image image;

    public Sprite normalSprite;
    public Sprite hoverSprite;

    private void Start()
    {
        image.sprite = normalSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = normalSprite;
    }
}