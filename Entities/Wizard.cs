namespace COIS2020.priashabarua0778496.Assignment3;

using Microsoft.Xna.Framework;
using COIS2020.StarterCode.Assignment3;


public class Wizard : CombatEntity, IComparable<Wizard>, IEquatable<Wizard>
{
    private static readonly string[] names; // Names get filled from txt file automatically

    // Instance fields
    // --------------------------------

    public string Name { get; set; }

    public int SpellLevel { get; set; }
    public Element SpellType { get; set; }

    public int Energy { get; set; }
    public int MaxEnergy { get; protected set; }


    // Constructors
    // --------------------------------

    /// <summary>
    /// Creates a new wizard with a random name.
    /// </summary>
    public Wizard() : this(names[RNG.Next(names.Length)])
    { }

    /// <summary>
    /// Creates a new wizard at a specific position with a random name.
    /// </summary>
    public Wizard(float x, float y) : this(names[RNG.Next(names.Length)], x, y)
    { }

    /// <summary>
    /// Creates a new wizard at a specific position with a random name.
    /// </summary>
    public Wizard(Vector2 position) : this(names[RNG.Next(names.Length)], position)
    { }

    /// <summary>
    /// Creates a new wizard.
    /// </summary>
    public Wizard(string name)
    {
        Name = name;
        SpriteName = PickRandomSpriteName();

        Energy = MaxEnergy = RNG.Next(3, 11);   // Wizards have between 3-10 mana.
        SpellLevel = RNG.Next(1, 5);            // Spells are between levels 1 to 4.

        // Random spell type
        var elements = Enum.GetValues<Element>();
        SpellType = elements[RNG.Next(elements.Length)];

        RendererTintColor = CastleGameRenderer.GetRandomColor();
    }

    /// <summary>
    /// Creates a new wizard at the given position.
    /// </summary>
    public Wizard(string name, float x, float y) : this(name, new Vector2(x, y))
    { }

    /// <summary>
    /// Creates a new wizard at the given position.
    /// </summary>
    public Wizard(string name, Vector2 position) : this(name)
    {
        Position = position;
    }


    #region Provided code

    public int CompareTo(Wizard? other)
    {
        int otherLvl = other?.SpellLevel ?? int.MinValue;
        return SpellLevel.CompareTo(otherLvl) * -1; // -1 for reverse order; big -> small
    }

    public bool Equals(Wizard? other) => Equals((CombatEntity?)other); // Use base CombatEntity equality comparison
    public override bool Equals(object? obj) => Equals(obj as Wizard);
    public override int GetHashCode() => base.GetHashCode();

    public override string ToString()
    {
        #pragma warning disable format // Force VS not to mess up my indenting!
        string element;
        switch (SpellType)
        {
            // (find Unicode code-points for emojis on emojipedia.org under the Technical Information tab)
            case Element.Fire:      element = "\U0001F525"; break;      // "Fire" 🔥
            case Element.Water:     element = "\U0001F4A7"; break;      // "Droplet" 💧
            case Element.Earth:     element = "\U0001FAA8"; break;      // "Rock" 🪨
            case Element.Dark:      element = "\U0001F31C"; break;      // "Last Quarter Moon Face" 🌜
            case Element.Light:     element = "\U0001F31E"; break;      // "Sun with Face" 🌞
            case Element.Wind:      element = "\U0001F4A8"; break;      // "Dash Symbol" 💨
            case Element.Ice:       element = "\u2744\uFE0F"; break;    // "Snowflake" ❄
            case Element.Lightning: element = "\u26A1"; break;          // "High Voltage Sign" ⚡
            default:                element = "\u2753"; break;          // "Black Question Mark Ornament" ❓
        }
        #pragma warning restore format

        return $"{element} {Name} (lvl. {SpellLevel})";
    }

    // Static stuff
    // --------------------------------

    // Static constructor, runs once when this class is first used. Static constructors are a way to initialize `static`
    // fields when they might require more logic than a single assignment statement.
    static Wizard()
    {
        // Get output directory of program and find goblin names.
        var dllPath = Path.GetDirectoryName(typeof(Wizard).Assembly.Location);
        var txtPath = Path.Join(dllPath, "Resources/Assn1-Wizards.txt");

        // Add to a list, splitting at the 1st space and grabbing the second half (plus trimming extra whitespace).
        var nameList = new System.Collections.Generic.List<string>();
        foreach (string line in File.ReadLines(txtPath))
            nameList.Add(line.Split(null, 2)[1].Trim());

        // Save as an array, since it won't need to grow again.
        names = nameList.ToArray();
    }

    protected static string PickRandomSpriteName()
    {
        var person = RNG.Next(1, 14 + 1);                               // 14 skins to choose from; 1 to 14 inclusive
        var weapon = (RNG.NextDouble() >= 0.01) ? "Scepter" : "Gun";    // 1% chance for wizard to spawn with a gun lol
        return $"People/Skin{person}.{weapon}";
    }

    #endregion
}
