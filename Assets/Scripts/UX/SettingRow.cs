using UnityEngine;
using UnityEngine.UI;

public class SettingRow : MonoBehaviour
{
    public GameObject bgNormal;
    public GameObject bgSelected;

    public Vector3 normalScale = Vector3.one;
    public Vector3 selectedScale = new Vector3(1.05f, 1.05f, 1f);

    public float smoothSpeed = 10f;

    private Vector3 targetScale;

    void Start()
    {
        targetScale = normalScale;
        transform.localScale = normalScale;

        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        // bật tắt layer
        bgNormal.SetActive(!isSelected);
        bgSelected.SetActive(isSelected);

        // scale popup
        targetScale = isSelected ? selectedScale : normalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * smoothSpeed
        );
    }
}