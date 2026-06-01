using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    public Sprite selectedSprite;
    public RuntimeAnimatorController selectedAnimator;
    public int selectedIndex = -1; // ← thiếu field này

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }
}