using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    public Sprite selectedSprite;
    public RuntimeAnimatorController selectedAnimator;      // animator trong game
    public RuntimeAnimatorController selectedMenuAnimator;  // animator ở main menu
    public int selectedIndex = -1;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }
}