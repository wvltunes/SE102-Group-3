using UnityEngine;

public class MaterialParallax : MonoBehaviour
{
    private Material mat;
    private Vector2 offset;

    [SerializeField] private float speed = 0.1f;

    void Start()
    {
        mat = GetComponent<Renderer>().material;

        if (mat == null)
        {
            Debug.LogError("No material found on Renderer!");
        }
    }

    void Update()
    {
        offset.x += speed * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }
}