using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SliderClickControl : MonoBehaviour, IPointerDownHandler
{
    public Slider slider;
    public RectTransform fillArea;
    public TMP_Text valueText;

    public float step = 0.1f; 

    void Start()
    {
        slider.onValueChanged.AddListener(UpdateText);
        if (slider == null)
            slider = GetComponent<Slider>();
        UpdateText(slider.value);
    }

    void UpdateText(float value)
    {
        valueText.text = value.ToString("0.00");
    }
  
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransform rect = slider.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            float percent = Mathf.InverseLerp(
                rect.rect.xMin,
                rect.rect.xMax,
                localPoint.x
            );

            slider.value = percent * slider.maxValue;
        }
    }

   
    public void Decrease()
    {
        slider.value = Mathf.Clamp(slider.value - step, slider.minValue, slider.maxValue);
    }

    public void Increase()
    {
        slider.value = Mathf.Clamp(slider.value + step, slider.minValue, slider.maxValue);
    }
}