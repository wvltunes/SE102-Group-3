using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Material material;
    [SerializeField] private float parallaxFactor= 0.1f;
    private float offset;
    public float gameSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }
        else
        {
            Debug.LogError("Renderer component not found on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (material != null)
        {
            ParalaxScroll();
        }
    }
    private void ParalaxScroll()
    {
        float Speed = gameSpeed * parallaxFactor;
        offset += Speed *Time.deltaTime;
        material.SetTextureOffset("_MainTex", Vector2.right*offset);
    }


}
