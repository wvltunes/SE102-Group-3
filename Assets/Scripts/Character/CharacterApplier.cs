using UnityEngine;

public class CharacterApplier : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    void Start()
    {
        var cm = CharacterManager.instance;
        if (cm == null) return;

        Animator anim = GetComponent<Animator>();
        if (anim != null && cm.selectedAnimator != null)
            anim.runtimeAnimatorController = cm.selectedAnimator;

        if (spriteRenderers.Length > 0 && cm.selectedSprite != null)
            spriteRenderers[0].sprite = cm.selectedSprite;
    }
}