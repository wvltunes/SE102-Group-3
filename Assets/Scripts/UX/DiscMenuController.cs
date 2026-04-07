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

        int unlockedCount = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < segmentParent.childCount; i++)
        {
            LevelItem item = segmentParent.GetChild(i).GetComponent<LevelItem>();
            if (item != null)
            {
                item.isUnlocked = (i < unlockedCount);
            }
        }

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
        int count = segmentParent.childCount;
        int nextIndex = (currentIndex + 1) % count;

        LevelItem nextItem = segmentParent.GetChild(nextIndex).GetComponent<LevelItem>();

        if (!nextItem.isUnlocked)
        {
            HitEffect(); 
            return;
        }

        currentIndex = nextIndex;
        UpdateRotation();
    }

    public void Back()
    {
        int count = segmentParent.childCount;
        int nextIndex = (currentIndex - 1 + count) % count;

        LevelItem nextItem = segmentParent.GetChild(nextIndex).GetComponent<LevelItem>();

        if (!nextItem.isUnlocked)
        {
            HitEffect();
            return;
        }

        currentIndex = nextIndex;
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
            LevelItem item = segmentParent.GetChild(i).GetComponent<LevelItem>();
            if (item != null)
            {
                
                bool isSelected = (i == currentIndex);

                item.UpdateUI(isSelected);
            }
        }
    }

    void HitEffect()
    {
        StopAllCoroutines();
        StartCoroutine(BounceBack());
    }

    System.Collections.IEnumerator BounceBack()
    {
        float original = targetAngle;


        float bump = targetAngle - 10f;

        float t = 0;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            float z = Mathf.Lerp(original, bump, t / 0.2f);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }

        t = 0;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            float z = Mathf.Lerp(bump, original, t / 0.2f);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
    }
}