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
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        ParalaxScroll();
    }
    private void ParalaxScroll()
    {
        float Speed = gameSpeed * parallaxFactor;
        offset += Speed *Time.deltaTime;
        material.SetTextureOffset("_MainTex", Vector2.right*offset);
    }


}
