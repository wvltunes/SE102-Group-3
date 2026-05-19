using UnityEngine;

public class JumpToGroundOrbVariables : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite JumpToGroundOrbSprite;

    void Start()
    {
        Transform transform = this.transform.Find("Sprite");
        JumpOrbBehaviour behaviour = GetComponent<JumpOrbBehaviour>();
        if (transform != null )
        {
            spriteRenderer = transform.GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null && transform)
        {
            spriteRenderer.sprite = JumpToGroundOrbSprite;
        }
    }
}
