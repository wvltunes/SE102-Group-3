using UnityEngine;

public class JumpOrbVariables : MonoBehaviour
{
    [SerializeField] private int laneMultiplier = 1;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite SingleLaneOrbSprite;
    [SerializeField] private Sprite DoubleLaneOrbSprite;
    [SerializeField] private Sprite TripleLaneOrbSprite;

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
                    spriteRenderer.sprite = SingleLaneOrbSprite;
                    behaviour.setLaneToJump(1);
                    break;
                case 2:
                    spriteRenderer.sprite = DoubleLaneOrbSprite;
                    behaviour.setLaneToJump(2);
                    break;
                case 3:
                    spriteRenderer.sprite = TripleLaneOrbSprite;
                    behaviour.setLaneToJump(3);
                    break;
                default:
                    spriteRenderer.sprite = SingleLaneOrbSprite;
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
