using UnityEngine;

public class PadVariables : MonoBehaviour
{
    [SerializeField] private int laneMultiplier = 1;
    private SpriteRenderer spriteRenderer;
    public Sprite SingleLanePadSprite;
    public Sprite DoubleLanePadSprite;
    public Sprite TripleLanePadSprite;

    void Start()
    {
        Transform transform = this.transform.Find("Sprite");
        JumpPadBehaviour behaviour = GetComponent<JumpPadBehaviour>();
        if (transform != null )
        {
            spriteRenderer = transform.GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null && transform)
        {
            switch (laneMultiplier)
            {
                case 1:
                    spriteRenderer.sprite = SingleLanePadSprite;
                    behaviour.setLaneToJump(1);
                    spriteRenderer.color = Color.pink;//temp color
                    break;
                case 2:
                    spriteRenderer.sprite = DoubleLanePadSprite;
                    behaviour.setLaneToJump(2);
                    spriteRenderer.color = Color.yellow;//temp color
                    break;
                case 3:
                    spriteRenderer.sprite = TripleLanePadSprite;
                    behaviour.setLaneToJump(3);
                    spriteRenderer.color = Color.orange;//temp color
                    break;
                default:
                    spriteRenderer.sprite = SingleLanePadSprite;
                    behaviour.setLaneToJump(1);
                    spriteRenderer.color = Color.pink;//temp color
                    break;
            }
        }
    }
    public void setLaneMultiplier (int newLaneMultiplier)
    {
        this.laneMultiplier = newLaneMultiplier;
    }
    public int getLaneMultiplier ()
    {
        return this.laneMultiplier;
    }

}
