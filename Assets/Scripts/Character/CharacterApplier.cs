using UnityEngine;

public class CharacterApplier : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    void Start()
    {
        var cm = CharacterManager.instance;
        if (cm == null) return;

        // Make sure the chosen character's visuals are resolved from the persisted index
        // (+ Resources roster) when the select screen did not push live refs this session.
        cm.EnsureGameVisuals();

        Animator anim = GetComponent<Animator>();
        if (anim != null && cm.selectedAnimator != null)
            anim.runtimeAnimatorController = cm.selectedAnimator;

        if (spriteRenderers.Length > 0 && cm.selectedSprite != null)
            spriteRenderers[0].sprite = cm.selectedSprite;
    }
}