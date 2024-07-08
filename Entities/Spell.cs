namespace COIS2020.priashabarua0778496.Assignment3;

using Microsoft.Xna.Framework;


public enum Element
{
    Fire,
    Water,
    Earth,
    Dark,
    Light,
    Wind,
    Ice,
    Lightning,
}


/// <summary>
/// A spell that a wizard can shoot. This is basically just a wrapper for a position with a sprite.
/// </summary>
public class Spell : CombatEntity
{
    // Static fields
    // --------------------------------

    public static readonly float Speed = 0.75f;


    // Instance fields
    // --------------------------------

    public Element Type { get; init; }


    // Constructors
    // --------------------------------

    public Spell(Element type)
    {
        Type = type;
    }

    public Spell(Element type, float x, float y) : base(x, y)
    {
        Type = type;
    }

    public Spell(Element type, Vector2 position) : base(position)
    {
        Type = type;
    }


    public override string SpriteName => GetElementSprite(Type) ?? base.SpriteName;

    public static string? GetElementSprite(Element element)
    {
        return element switch
        {
            Element.Fire => "UI/AttackTypes/Fire.Alt",
            Element.Water => "UI/AttackTypes/Water",
            Element.Earth => "UI/AttackTypes/Grass",
            Element.Dark => "UI/AttackTypes/Poison",
            Element.Light => "UI/StatusBar/TwoStars.2",
            Element.Ice => "UI/AttackTypes/Ice",
            Element.Wind => "UI/AttackTypes/Haze",
            Element.Lightning => "UI/AttackTypes/Lightning",
            _ => null,
        };
    }
}
