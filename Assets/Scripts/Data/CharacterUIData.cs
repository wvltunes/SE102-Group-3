using UnityEngine;

[CreateAssetMenu(menuName = "Character UI/Data")]
public class CharacterUIData : ScriptableObject
{
    [Header("TEXT")]
    public string characterName;
    public string role;
    [TextArea] public string description;

    [Header("TRAITS")]
    public string trait1;
    public string trait2;
    public string trait3;

    [Header("SPRITES")]
    public Sprite background;
    public Sprite characterRender;

    public Sprite panelSprite;
    public Sprite tabActiveSprite;
    public Sprite tabInactiveSprite;
    public Sprite buttonSprite;

    [Header("THEME")]
    public Color mainColor;
    public Color glowColor;
    public Color textColor;

    [Header("STATS")]
    [Range(0, 100)] public int energy;
    [Range(0, 100)] public int speed;
    
}