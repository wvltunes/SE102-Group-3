using UnityEngine;

public class GravityOrbVariables : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite gravityOrbSprite;
    
    void Start()
    {
        Transform transform = this.transform.Find("Sprite");
        if (transform != null)
        {
            spriteRenderer= transform.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = gravityOrbSprite;
        }
        
    }

}
