using UnityEngine;

public class DiscMenuController : MonoBehaviour
{
    public RectTransform segmentParent;
    public float rotateSpeed = 8f;

    private int currentIndex = 0;
    private float targetAngle;

    void Start()
    {
        Canvas.ForceUpdateCanvases();
        SetInitialRotation();
    }

    void Update()
    {
        SmoothRotate();
    }

    void SetInitialRotation()
    {
        float step = 360f / segmentParent.childCount;
        targetAngle = -currentIndex * step;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        Highlight();
    }

    void SmoothRotate()
    {
        float currentZ = transform.eulerAngles.z;

        float newZ = Mathf.LerpAngle(
            currentZ,
            targetAngle,
            1 - Mathf.Exp(-rotateSpeed * Time.deltaTime)
        );

        transform.rotation = Quaternion.Euler(0, 0, newZ);
    }

    public void Next()
    {
        Debug.Log("Next clicked");

        int count = segmentParent.childCount;
        currentIndex = (currentIndex + 1) % count;

        UpdateRotation();
    }

    public void Back()
    {
        Debug.Log("Back clicked");

        int count = segmentParent.childCount;
        currentIndex = (currentIndex - 1 + count) % count;

        UpdateRotation();
    }

    void UpdateRotation()
    {
        float step = 360f / segmentParent.childCount;
        targetAngle = -currentIndex * step;
        Highlight();
    }

    void Highlight()
    {
        for (int i = 0; i < segmentParent.childCount; i++)
        {
            Transform seg = segmentParent.GetChild(i);
            seg.localScale = (i == currentIndex) ? Vector3.one * 1.2f : Vector3.one;
        }
    }
}