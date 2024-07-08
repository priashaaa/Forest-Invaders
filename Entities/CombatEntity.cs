namespace COIS2020.priashabarua0778496.Assignment3;

using Microsoft.Xna.Framework;


public class CombatEntity : IComparable<CombatEntity>, IEquatable<CombatEntity>
{
    private static uint nextID = 1; // Auto-incremented whenever a new entity is created
    public readonly static string DefaultSprite = "UI/Symbols/QuestionMark";

    // Instance fields
    // --------------------------------

    /// <summary>
    /// This entity's unique identifier.
    /// </summary>
    public uint ID { get; private set; }

    /// <summary>
    /// This entity's <i>(x, y)</i> position in 2D space.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// A string denoting which sprite this entity wants to be drawn with.
    /// </summary>
    ///
    /// <remarks>
    /// The names of the sprites are grouped based on which file from the original asset pack they came from. See the
    /// XML files in the <i>Resources</i> folder to determine the group/name combination required for a given sprite.
    /// Not all sprites from the asset pack are included in the map.
    /// </remarks>
    public virtual string SpriteName { get; protected set; }

    /// <summary>
    /// Applied as a tint to this entity's base sprite texture when drawing.
    /// </summary>
    public virtual Color RendererTintColor { get; set; }


    // Constructors
    // --------------------------------

    /// <summary>
    /// Creates a new entity at (0, 0).
    /// </summary>
    public CombatEntity()
    {
        ID = nextID++;
        Position = new Vector2(0, 0);
        SpriteName = DefaultSprite;
        RendererTintColor = Color.White;
    }

    /// <summary>
    /// Creates a new combat entity at the given position.
    /// </summary>
    public CombatEntity(float x, float y) : this(new Vector2(x, y))
    { }

    /// <summary>
    /// Creates a new combat entity at the given position.
    /// </summary>
    public CombatEntity(Vector2 position) : this()
    {
        Position = position;
    }


    // Methods
    // --------------------------------

    public int CompareTo(CombatEntity? other) => ID.CompareTo(other?.ID ?? int.MaxValue);


    /// <summary>
    /// Checks whether or not two entities are overlapping.
    /// </summary>
    /// <param name="a">The first entity.</param>
    /// <param name="b">The second entity.</param>
    /// <returns><c>true</c> if the entities' sprites are overlapping; <c>false</c> if not.</returns>
    public static bool Colliding(CombatEntity a, CombatEntity b)
    {
        static (Vector2, Vector2) Corners(Vector2 position)
        {
            var offset = Vector2.One / 2;
            var tl = position - offset;
            var br = position + offset;
            return (tl, br);
        }

        // Every entity in the game-world is a 1x1-unit square. So we just check like they're rectangles.
        ((float aLeft, float aTop), (float aRight, float aBottom)) = Corners(a.Position);
        ((float bLeft, float bTop), (float bRight, float bBottom)) = Corners(b.Position);
        bool h = aLeft < bRight && aRight > bLeft;
        bool v = aTop < bBottom && aBottom > bTop;
        return h && v;
    }

    /// <summary>
    /// Checks if this entity is currently overlapping with another.
    /// </summary>
    /// <param name="other">The entity to check against.</param>
    /// <returns><c>true</c> if the entities' sprites are overlapping; <c>false</c> if not.</returns>
    public bool Colliding(CombatEntity other) => Colliding(this, other);


    /// <summary>
    /// Computes the distance between this entity and some other entity.
    /// </summary>
    public float DistanceTo(CombatEntity other) => DistanceTo(other.Position);

    /// <summary>
    /// Computes the distance between this entity's position and some other point.
    /// </summary>
    public float DistanceTo(Vector2 point) => (point - Position).Length();


    /// <summary>
    /// Updates this entity's position.
    /// </summary>
    ///
    /// <param name="dx">An offset to add to this entity's x-position.</param>
    /// <param name="dy">An offset to add to this entity's y-position.</param>
    public void Move(float dx, float dy) => Position += new Vector2(dx, dy);

    /// <summary>
    /// Updates this entity's position.
    /// </summary>
    ///
    /// <param name="deltaPos">An offset vector to add to this entity's position.</param>
    public void Move(Vector2 deltaPos) => Position += deltaPos;

    /// <summary>
    /// Updates this entity's position by moving a specified amount in the given direction.
    /// </summary>
    /// <param name="direction">A unit-length, direction vector.</param>
    /// <param name="distance">How far in the given direction to move.</param>
    public void Move(Vector2 direction, float distance) => Move(Vector2.Normalize(direction) * distance);

    /// <summary>
    /// Updates this entity's position by moving a specified amount in the given direction.
    /// </summary>
    /// <param name="dirX">The X-component of a unit-length, direction vector.</param>
    /// <param name="dirY">The Y-component of a unit-length, direction vector.</param>
    /// <param name="distance">How far in the given direction to move.</param>
    public void Move(float dirX, float dirY, float distance) => Move(new Vector2(dirX, dirY), distance);


    /// <summary>
    /// Moves this entity towards some 2D position by at most <paramref name="speed"/> units.
    /// </summary>
    ///
    /// <param name="point">The point to move towards.</param>
    /// <param name="speed">The farthest this entity is allowed to move in a single frame.</param>
    public void MoveTowards(Vector2 point, float speed)
    {
        Vector2 ab = point - Position;                      // A to B = B - A.
        Move(ScaleVector(ab, size: speed, clamp: true));    // Scale before moving.
    }

    /// <summary>
    /// Moves this entity towards another entity by at most <paramref name="speed"/> units.
    /// </summary>
    ///
    /// <param name="other">The entity to move towards.</param>
    /// <param name="speed">The farthest this entity is allowed to move in a single frame.</param>
    public void MoveTowards(CombatEntity other, float speed) => MoveTowards(other.Position, speed);


    /// <summary>
    /// Moves this entity away from some 2D point.
    /// </summary>
    ///
    /// <param name="point">The point to move away from.</param>
    /// <param name="amount">How far to move away from the given point.</param>
    public void PushAwayFrom(Vector2 point, float amount)
    {
        Vector2 ba = Position - point;                      // "B to A" is the same direction as "A away from B".
        Move(ScaleVector(ba, size: amount, clamp: false));  // Scale before moving.
    }

    /// <summary>
    /// Moves this entity in the opposite direction from another.
    /// </summary>
    ///
    /// <param name="other">The entity to move away from.</param>
    /// <param name="amount">How far to move away from the given point.</param>
    public void PushAwayFrom(CombatEntity other, float amount) => PushAwayFrom(other.Position, amount);


    /// <summary>
    /// Scales a movement vector.
    /// </summary>
    ///
    /// <remarks>
    /// If <paramref name="clamp"/> is true, then <paramref name="vec"/> is scaled to be no longer than
    /// <paramref name="size"/>. Otherwise, <paramref name="vec"/> is scaled to have a length of exactly
    /// <paramref name="size"/>.
    /// </remarks>
    ///
    /// <param name="vec">The vector to scale.</param>
    /// <param name="size">The desired size.</param>
    /// <param name="clamp">Which mode to scale with.</param>
    private static Vector2 ScaleVector(Vector2 vec, float size, bool clamp)
    {
        float len = vec.Length();                       // Check length from (A->B).
        if (len <= 0.0000001f) return Vector2.Zero;     // If zero(ish), quit to avoid NaN.
        else if (clamp && len < size) return vec;       // If clamping, and if already small enough, just return.
        else return vec / len * size;                   // Otherwise, normalize and re-scale to desired size.
    }


    /// <summary>
    /// Ensures that this entity's position does not fall outside of a given range.
    /// </summary>
    ///
    /// <param name="xRange">A range of allowed x-values for this entity's position (min, max).</param>
    /// <param name="yRange">A range of allowed y-values for this entity's position (min, max).</param>
    public void ClampPosition((float, float) xRange, (float, float) yRange)
    {
        var (xMin, xMax) = xRange;
        var (yMin, yMax) = yRange;
        ClampPosition(xMin, xMax, yMin, yMax);
    }

    /// <summary>
    /// Ensures that this entity's position does not fall outside of a given range.
    /// </summary>
    ///
    /// <param name="xMin">The minimum allowed x-value for this entity's position.</param>
    /// <param name="xMax">The maximum allowed x-value for this entity's position.</param>
    /// <param name="yMin">The minimum allowed y-value for this entity's position.</param>
    /// <param name="yMax">The maximum allowed y-value for this entity's position.</param>
    public void ClampPosition(float xMin, float xMax, float yMin, float yMax)
    {
        float newX = Math.Clamp(Position.X, xMin, xMax);
        float newY = Math.Clamp(Position.Y, yMin, yMax);
        Position = new Vector2(newX, newY);
    }


    public bool Equals(CombatEntity? other) => other?.ID.Equals(ID) ?? false;
    public override bool Equals(object? obj) => Equals(obj as CombatEntity);
    public override int GetHashCode() => ID.GetHashCode(); // Hash as if entity was just an int
}
