namespace COIS2020.priashabarua0778496.Assignment3;

using Microsoft.Xna.Framework;
using COIS2020.StarterCode.Assignment3;


public enum GoblinClan
{
    Cragmaw,
    BogBrotherhood,
    Boko,
}

public class Goblin : CombatEntity, IComparable<Goblin>, IEquatable<Goblin>
{
    private static readonly string[] names; // Names get filled from txt file automatically

    // Static fields
    // --------------------------------

    public static readonly float Speed = 0.75f;


    // Instance fields
    // --------------------------------

    public string Name { get; set; }
    public GoblinClan Clan { get; set; }
    public int AttackPower { get; set; }


    // Constructors
    // --------------------------------

    /// <summary>
    /// Creates a new goblin with a random name.
    /// </summary>
    public Goblin() : this(names[RNG.Next(names.Length)])
    { }

    /// <summary>
    /// Creates a new goblin at a specific position with a random name.
    /// </summary>
    public Goblin(float x, float y) : this(names[RNG.Next(names.Length)], x, y)
    { }

    /// <summary>
    /// Creates a new goblin at a specific position with a random name.
    /// </summary>
    public Goblin(Vector2 position) : this(names[RNG.Next(names.Length)], position)
    { }

    /// <summary>
    /// Creates a new goblin with a pre-seeded random number generator.
    /// </summary>
    public Goblin(string name)
    {
        Name = name;

        var clans = Enum.GetValues<GoblinClan>();
        Clan = clans[RNG.Next(clans.Length)];
        SpriteName = PickRandomSpriteName();
        RendererTintColor = CastleGameRenderer.GetRandomColor(60, 210);
    }

    /// <summary>
    /// Creates a new goblin at the given position.
    /// </summary>
    public Goblin(string name, float x, float y) : this(name, new Vector2(x, y))
    { }

    /// <summary>
    /// Creates a new goblin at the given position.
    /// </summary>
    public Goblin(string name, Vector2 position) : this(name)
    {
        Position = position;
    }


    #region Provided code

    public int CompareTo(Goblin? other)
    {
        int otherPower = other?.AttackPower ?? int.MinValue;
        return AttackPower.CompareTo(otherPower) * -1; // -1 for reverse order; big -> small
    }


    public bool Equals(Goblin? other) => Equals((CombatEntity?)other); // Use base CombatEntity equality comparison
    public override bool Equals(object? obj) => Equals(obj as Goblin);
    public override int GetHashCode() => base.GetHashCode();

    public override string ToString()
    {
        string clan;

        #pragma warning disable format // Force VS not to mess up my indenting!
        switch (Clan)
        {
            case GoblinClan.Boko:           clan = "\U0001F43D"; break;     // "Pig Nose" emoji 🐽
            case GoblinClan.BogBrotherhood: clan = "\U0001F438"; break;     // "Frog Face" 🐸
            case GoblinClan.Cragmaw:        clan = "\u26F0\uFE0F"; break;   // "Mountain" ⛰
            default:                        clan = "\u2753"; break;         // "Red Question Mark" ❓
        }
        #pragma warning restore format

        return $"{clan} {Name} (pow. {AttackPower})";
    }

    // Static stuff
    // --------------------------------

    // Static constructor; runs once when this class is first used. Static constructors are a way to initialize `static`
    // fields when they might require more logic than a single assignment statement (like reading a file).
    static Goblin()
    {
        // Get output directory of program and find goblin names.
        var dllPath = Path.GetDirectoryName(typeof(Goblin).Assembly.Location);
        var txtPath = Path.Join(dllPath, "Resources/Assn1-Goblins.txt");

        // Trim them as we read them into a list.
        var nameList = new System.Collections.Generic.List<string>();
        foreach (string line in File.ReadLines(txtPath))
            nameList.Add(line.Trim());

        // Save as an array, since it won't need to grow again.
        names = nameList.ToArray();
    }

    protected static string PickRandomSpriteName()
    {
        double rand = RNG.NextDouble();
        return rand switch
        {
            <= 0.50 => "Enemies/Goblin",            // 50% chance
            <= 0.80 => "Enemies/Goblin.Sword",      // 30% chance
            <= 0.95 => "Enemies/Goblin.Glaive",     // 15% chance
            <= 0.99 => "Enemies/Goblin.Scepter",    // 4% chance
            _ => "Enemies/Martian",                 // 1% chance :o
        };
    }

    #endregion
}
