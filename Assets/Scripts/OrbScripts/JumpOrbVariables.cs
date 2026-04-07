using UnityEngine;

public class JumpOrbVariables : MonoBehaviour
{
    [SerializeField] private int laneMultiplier = 1;
    private SpriteRenderer spriteRenderer;
    public Sprite SingleLanePadSprite;
    public Sprite DoubleLanePadSprite;
    public Sprite TripleLanePadSprite;

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
            switch (laneMultiplier)
            {
                case 1:
                    spriteRenderer.sprite = SingleLanePadSprite;
                    behaviour.setLaneToJump(1);
                    break;
                case 2:
                    spriteRenderer.sprite = DoubleLanePadSprite;
                    behaviour.setLaneToJump(2);
                    break;
                case 3:
                    spriteRenderer.sprite = TripleLanePadSprite;
                    behaviour.setLaneToJump(3);
                    break;
                default:
                    spriteRenderer.sprite = SingleLanePadSprite;
                    behaviour.setLaneToJump(1);
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
