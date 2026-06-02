using UnityEngine;

public class ParallaxBackgroundController : MonoBehaviour
{
    [SerializeField] private GameObject[] backgrounds;
    [SerializeField] private Transform backgroundParent;

    private GameObject currentBackground;

    private void Start()
    {
        SetBackground(0);
    }

    public void SetBackground(int index)
    {
        if (currentBackground != null)
        {
            Destroy(currentBackground);
        }

        currentBackground = Instantiate(
            backgrounds[index],
            backgroundParent
        );

        currentBackground.transform.localPosition = Vector3.zero;
    }
}