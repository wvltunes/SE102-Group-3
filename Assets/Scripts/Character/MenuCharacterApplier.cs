// MenuCharacterApplier.cs
using UnityEngine;

public class MenuCharacterApplier : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    void Start()
    {
        var cm = CharacterManager.instance;
        if (cm == null) return;

        Animator anim = GetComponent<Animator>();
        if (anim != null && cm.selectedMenuAnimator != null)
            anim.runtimeAnimatorController = cm.selectedMenuAnimator;

        if (spriteRenderers.Length > 0 && cm.selectedSprite != null)
            spriteRenderers[0].sprite = cm.selectedSprite;
    }
}