using UnityEngine;

public class GravityPadVariables : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite gravityPadSprite;
    
    void Start()
    {
        Transform transform = this.transform.Find("Sprite");
        if (transform != null)
        {
            spriteRenderer= transform.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = gravityPadSprite;
        }
        
    }

}
